using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CometGateway.Server.Gateway.ANSIColorsSupport;
using Rhino.Mocks;
using System.Drawing;

namespace CometGateway.Server.Gateway.Tests.ANSIColorsSupport
{
    [TestClass]
    public class ANSICommandsDecoderTest
    {
        const string ESC = "\x11";
        [TestMethod]
        public void CharDecodesToCharCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("abc");
            CollectionAssert.AreEqual(
                    new[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new CharHTMLCommand() {Char = 'b'},
                        new CharHTMLCommand() {Char = 'c'}
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void LfIsDecodedToNewLine()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a\n");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new NewLineHTMLCommand()
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void ResetCodeIsDecodedToResetCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[0m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new ResetCommand()
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void CRIsIgnored()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a\r");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'}
                    },
                    actualCommands
                );
        }


        [TestMethod]
        public void ForegroundCodeDecodedsToForegroundCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a" + ESC + "[36mb" );
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new ForegroundColorCommand(Color.Cyan),
                        new CharHTMLCommand() {Char = 'b'}
                    },
                    actualCommands
                );            
        }

        [TestMethod]
        public void BackgroundCodeDecodedsToBackgroundCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a" + ESC + "[46mb");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new BackgroundColorCommand(Color.Cyan),
                        new CharHTMLCommand() {Char = 'b'}
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void DefaultBackgroundCodeDecodedsToDefaultBackgroundCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a" + ESC + "[49mb");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new BackgroundColorCommand(Color.Black),
                        new CharHTMLCommand() {Char = 'b'}
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void DefaultForegroundCodeDecodedsToDefaultForegroundCommand()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a" + ESC + "[39mb");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new ForegroundColorCommand(Color.White),
                        new CharHTMLCommand() {Char = 'b'}
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void BogusCommandsAreIgnored()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat("a" + ESC + "123b4");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new CharHTMLCommand() {Char = 'a'},
                        new CharHTMLCommand() {Char = 'b'}
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void BoldCodesAreConvertedToBoldCommands()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[1m" + ESC + "[22m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new SetBoldCommand(true),
                        new SetBoldCommand(false)
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void ItalicCodesAreConvertedToItalicCommands()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[3m" + ESC + "[23m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new SetItalicCommand(true),
                        new SetItalicCommand(false)
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void UnderlineCodesAreConvertedToUnderlineCommands()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[4m" + ESC + "[24m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new SetUnderlineCommand(true),
                        new SetUnderlineCommand(false)
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void StrikethroughCommandsAreConvertedToStrikethroughCodes()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[9m" + ESC + "[29m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new SetStrikethroughCommand(true),
                        new SetStrikethroughCommand(false)
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void ReverseVideoCommandsAreConvertedToReverseVideoCodes()
        {
            var decoder = new ANSICommandsDecoder(null, new ANSICommandStateMachine());
            IANSICommand[] actualCommands = decoder.ConvertFromInternalFormat(ESC + "[7m" + ESC + "[27m");
            CollectionAssert.AreEqual(
                    new IANSICommand[] 
                    { 
                        new SetReverseVideoCommand(true),
                        new SetReverseVideoCommand(false)
                    },
                    actualCommands
                );
        }

        [TestMethod]
        public void ColorsDecodeCorrectly()
        {
            // 0 - black
            // 1 - red
            // 2 - green
            // 3 - yellow
            // 4 - blue
            // 5 - magenta
            // 6 - cyan
            // 7 - white
            Assert.AreEqual(Color.Black, ANSICommandStateMachine.DecodeColor('0'));
            Assert.AreEqual(Color.Red, ANSICommandStateMachine.DecodeColor('1'));
            Assert.AreEqual(Color.Lime, ANSICommandStateMachine.DecodeColor('2'));
            Assert.AreEqual(Color.Yellow, ANSICommandStateMachine.DecodeColor('3'));
            Assert.AreEqual(Color.Blue, ANSICommandStateMachine.DecodeColor('4'));
            Assert.AreEqual(Color.Magenta, ANSICommandStateMachine.DecodeColor('5'));
            Assert.AreEqual(Color.Cyan, ANSICommandStateMachine.DecodeColor('6'));
            Assert.AreEqual(Color.White, ANSICommandStateMachine.DecodeColor('7'));
        }
    }
}
