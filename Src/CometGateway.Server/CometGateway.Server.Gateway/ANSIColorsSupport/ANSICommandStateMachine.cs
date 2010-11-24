using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public class ANSICommandStateMachine : IANSICommandsStateMachine
    {
        string commandBuffer = "";

        public IANSICommand Decode(char received)
        {
            const char ESC = '\x11';
            if (received == ESC || !String.IsNullOrEmpty(commandBuffer) )
            {
                commandBuffer += received;
                var decodedCommand = DecodeCommand(commandBuffer);
                if (decodedCommand != null)
                    commandBuffer = "";

                int maxCommandLength = GetCommandMapping().Keys.Max(k => k.Length);
                if (decodedCommand == null && commandBuffer.Length == maxCommandLength)
                {
                    commandBuffer = "";
                    return null;
                }
                
                return decodedCommand;
            }

            return
                received == '\n' ? (IANSICommand)new NewLineHTMLCommand() :
                received == '\r'? null :
                new CharHTMLCommand() { Char = received };
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
            const string prefix = "\x11" + "[";
            const string suffix = "m";

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
                pair => prefix + pair.Key + suffix, 
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
