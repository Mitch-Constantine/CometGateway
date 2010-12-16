using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public class ANSICommandStateMachine : IANSICommandsStateMachine
    {
        StringBuilder commandBuffer = new StringBuilder();

        public IANSICommand[] Decode(char received)
        {
            const char ESC = '\x1B';

            if (received == ESC)
            {
                commandBuffer.Append( received );
                return new IANSICommand[]{};
            }

            if (commandBuffer.Length > 0)
            {
                bool storeOpenBracket = 
                    received == '[' && 
                    commandBuffer.Length == 1;
                bool storeCommand = 
                        commandBuffer.Length >= 2 &&
                        Char.IsDigit(received) || received == ';';
                
                if (storeOpenBracket || storeCommand)
                {
                    commandBuffer.Append( received );
                    return new IANSICommand[] { };
                }

                if (commandBuffer.Length > 2 && received == 'm')
                {
                    var commands = commandBuffer.ToString().Substring(2);
                    var arrCommands = commands.Split(';');

                    commandBuffer = new StringBuilder();
                    return (from cmd in arrCommands
                            let decoded = DecodeCommand(cmd)
                            where decoded != null
                            select decoded)
                                .ToArray();
                }

                var charCommands =
                    (from c in commandBuffer.ToString()
                        select new CharHTMLCommand {Char = c })
                        .ToArray();
                commandBuffer = new StringBuilder();
                return charCommands;
            }
            return
                received == '\n' ? new IANSICommand[] { new NewLineHTMLCommand() }:
                received == '\r'? new IANSICommand[] {} :
                new IANSICommand[] { new CharHTMLCommand() { Char = received } };
        }

        private IANSICommand DecodeCommand(string command)
        {
            IDictionary<string, IANSICommand> commandMapping = GetCommandMapping();
            return commandMapping.ContainsKey(command) ? 
                        commandMapping[command] : 
                        null;
        }

        private IEnumerable<KeyValuePair<string, IANSICommand>> OnOffCommands(
            string codeOn, 
            string codeOff, 
            Func<bool, IANSICommand> commandGenerator
        )
        {
            return new[]
            {
                new KeyValuePair<string, IANSICommand>(codeOn, commandGenerator(true)),
                new KeyValuePair<string, IANSICommand>(codeOff, commandGenerator(false))
            };
        }
        
        private IDictionary<string, IANSICommand> GetCommandMapping()
        {
            IEnumerable<KeyValuePair<string, IANSICommand>>
                setForegroundCommands = ColorBasedCommands(
                        "3", 
                        (color) =>new ForegroundColorCommand(color),
                        Color.White
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setBackgroundCommands = ColorBasedCommands(
                    "4",
                    (color) => new BackgroundColorCommand(color),
                    Color.Black
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setBoldCommands = OnOffCommands(
                    "1", "22",
                    (enabled) => new SetBoldCommand(enabled)
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setItalicCommands = OnOffCommands(
                    "3", "23",
                    (enabled) => new SetItalicCommand(enabled)
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setUnderlineCommands = OnOffCommands(
                    "4", "24",
                    (enabled) => new SetUnderlineCommand(enabled)
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setReverseVideoCommands = OnOffCommands(
                    "7", "27",
                    (enabled) => new SetReverseVideoCommand(enabled)
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                setStrikethroughCommands = OnOffCommands(
                    "9", "29",
                    (enabled) => new SetStrikethroughCommand(enabled)
                );

            IEnumerable<KeyValuePair<string, IANSICommand>>
                resetCommand =
                    new[] {
                        new KeyValuePair<string, IANSICommand>(
                                "0", new ResetCommand()
                        )
                    };
            
            var simplifiedCommands =
                setForegroundCommands
                    .Union(setBackgroundCommands)
                    .Union(setBoldCommands)
                    .Union(setItalicCommands)
                    .Union(setUnderlineCommands)
                    .Union(setStrikethroughCommands)
                    .Union(setReverseVideoCommands)
                    .Union(resetCommand);

            return simplifiedCommands.ToDictionary(
                pair => pair.Key, 
                pair => pair.Value
            );  
        }

        private static IEnumerable<KeyValuePair<string, IANSICommand>> ColorBasedCommands(
            string prefix,
            Func<Color, IANSICommand> commandGenerator,
            Color defaultColor
        )
        {
            var colorCodes =
                (from c in Enumerable.Range('0', 8)
                 select (char)c)
                    .ToArray();

            return (from color in colorCodes
                    let decodedColor = DecodeColor(color)
                    select new KeyValuePair<string, IANSICommand>(
                           prefix + color, commandGenerator(decodedColor)
                        ))
                   .Union(new [] {new KeyValuePair<string, IANSICommand>(
                       prefix + "9", commandGenerator(defaultColor)
                   )});
        }

        internal static Color DecodeColor(char ansiDigitCode)
        {
            // 0 - black
            // 1 - red
            // 2 - green
            // 3 - yellow
            // 4 - blue
            // 5 - magenta
            // 6 - cyan
            // 7 - white
            Dictionary<int, Color> colors = new Dictionary<int, Color>
            {
                {'0', Color.Black},
                {'1', Color.Red},
                {'2', Color.Lime},
                {'3', Color.Yellow},
                {'4', Color.Blue},
                {'5', Color.Magenta},
                {'6', Color.Cyan},
                {'7', Color.White}
            };

            return colors[ansiDigitCode];
        }
    }
}
