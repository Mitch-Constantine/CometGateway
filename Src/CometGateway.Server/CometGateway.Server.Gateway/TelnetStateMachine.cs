using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public class TelnetStateMachine : ITelnetStateMachine
    {
        const int IAC = 255;
        const int SB = 250;
        const int SE = 240;
        const int WILL = 251;
        const int WONT = 252;
        const int DO = 253;
        const int DONT = 254;

        const int ANY = -1;
        const int BYTE = -2;

        List<byte> currentCommand = new List<byte>();

        public byte[] Translate(byte incomingData)
        {
            if (incomingData != IAC &&
                !currentCommand.Any())
                return new[] { incomingData };

            currentCommand.Add(incomingData);
            byte[] commandRecognized = RecognizeCommand();
            if (commandRecognized != null)
            {
                currentCommand = new List<Byte>();
                return commandRecognized;
            }
            else
            {
                return new byte[] { };
            }
        }

        private byte[] RecognizeCommand()
        {
            return MapCommands(
                new[] { IAC, IAC }, new[] { IAC },
                new[] { IAC, DO, BYTE }, new[] { IAC, WONT, BYTE },
                new[] { IAC, DONT, BYTE }, new [] { IAC, WONT, BYTE },
                new [] { IAC, SB, ANY, IAC, SE }, new int []{},
                new [] { IAC, BYTE }, new int[] {BYTE}
            );
        }

        private byte[] MapCommands(params int[][] commandDescription )
        {
            for (int paramCrt = 0; paramCrt < commandDescription.Length; paramCrt += 2)
            {
                int[] commandTemplate = commandDescription[paramCrt];
                int[] answerTemplate = commandDescription[paramCrt + 1];
                byte [] answer;
                bool isMatch;
                MatchCommand(
                    commandTemplate, 
                    answerTemplate,
                    out isMatch,
                    out answer
                );

                if (answer != null)
                {
                    return answer;
                }
                if (isMatch)
                    return null;
            }

            return null;
        }

        private void MatchCommand(
            int[] commandTemplate, 
            int[] answerTemplate,
            out bool isMatch,
            out byte[] answer
        )
        {
            byte[] sequencePlaceholder;
            byte? bytePlaceholder;
            bool fullMatch;
            IsMatch(commandTemplate, out isMatch, out bytePlaceholder, out sequencePlaceholder, out fullMatch);

            if (fullMatch)
            {
                answer = ReplaceInTemplate(answerTemplate, bytePlaceholder, sequencePlaceholder);
            }
            else
            {
                answer = null;
            }
        }

        private void IsMatch(
            int[] commandTemplate, 
            out bool isMatch, 
            out byte? bytePlaceholder,
            out byte[] sequencePlaceholder, 
            out bool fullMatch
        )
        {
            int commandByteIndex = 0;
            sequencePlaceholder = null;
            bytePlaceholder = null;

            int ndxByteCrt;
            isMatch = true;
            int? anyTokenStartOffset = null;
            int anyTokenSequenceLength = 0;
            for (ndxByteCrt = 0; ndxByteCrt < commandTemplate.Length; ndxByteCrt++)
            {
                if (commandByteIndex >= currentCommand.Count)
                    break;

                if (commandTemplate[ndxByteCrt] == currentCommand[commandByteIndex])
                {
                    commandByteIndex++;
                }
                else if (commandTemplate[ndxByteCrt] == BYTE)
                {
                    bytePlaceholder = currentCommand[commandByteIndex];
                    commandByteIndex++;
                }
                else if (commandTemplate[ndxByteCrt] == ANY)
                {
                    anyTokenStartOffset = commandByteIndex;
                    int offsetFromEnd = commandTemplate.Length - ndxByteCrt;
                    commandByteIndex = currentCommand.Count - offsetFromEnd;
                    anyTokenStartOffset = ndxByteCrt;
                    anyTokenSequenceLength = currentCommand.Count - commandTemplate.Length - 1;
                }
                else
                {
                    isMatch = (anyTokenStartOffset != null);
                    break;
                }
            }

            fullMatch = isMatch && ndxByteCrt == commandTemplate.Length;
            if (fullMatch && anyTokenStartOffset != null)
            {
                sequencePlaceholder =
                    currentCommand
                        .Skip(anyTokenStartOffset.Value)
                        .Take(anyTokenSequenceLength)
                        .Select(n => (byte)n)
                        .ToArray();
            }

        }

        private static byte[] ReplaceInTemplate(int[] answerTemplate, byte? bytePlaceholder, byte[] sequencePlaceholder)
        {
            return answerTemplate
                        .SelectMany(
                            (templateToken) =>
                                    templateToken == BYTE ?
                                        new byte[] { bytePlaceholder.Value } :
                                    templateToken == ANY ?
                                            sequencePlaceholder :
                                    new byte[] { (byte)templateToken }
                        )
                        .ToArray();
        }

    }
}
