using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CometGateway.Server.Gateway.ANSIColorsSupport;
using System.Drawing;

namespace CometGateway.Server.Gateway.Tests.ANSIColorsSupport
{
    [TestClass]
    public class ANSICommandToHTMLConverterTest
    {
        [TestMethod]
        public void CharsConvertedToHtmlEncodedForm()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new[] { 
                        new CharHTMLCommand {Char = 'X'},
                        new CharHTMLCommand {Char = '<'}
                    }
                );

            Assert.AreEqual("<pre>X&lt;", actualHtml);
        }

        [TestMethod]
        public void CRLFGetsCorrectlyInserted()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand {Char = 'X'},
                        new NewLineHTMLCommand(),
                        new CharHTMLCommand {Char = 'Y'}
                    }
                );

            Assert.AreEqual("<pre>X</pre><pre>Y", actualHtml);
        }
        [TestMethod]
        public void EmptyLineGetsNBSP()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new NewLineHTMLCommand(),
                        new NewLineHTMLCommand(),
                        new NewLineHTMLCommand(),
                        new CharHTMLCommand {Char = 'Y'}
                    }
                );

            Assert.AreEqual("<pre>&nbsp;</pre><pre>&nbsp;</pre><pre>&nbsp;</pre><pre>Y", actualHtml);
        }

        [TestMethod]
        public void TextColorGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new ForegroundColorCommand(Color.Red),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='color:#8B0000'>cd", actualHtml);
        }

        [TestMethod]
        public void TextColorGetsAppliedAtStartOfLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new ForegroundColorCommand(Color.Red),
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre><font style='color:#8B0000'>abcd", actualHtml);
        }
    
        [TestMethod]
        public void FontInteractsCorrectlyWithLineEnd()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new ForegroundColorCommand(Color.Red),
                        new CharHTMLCommand { Char = 'a' },
                        new ForegroundColorCommand(Color.Blue),
                        new CharHTMLCommand { Char = 'b' },
                        new ForegroundColorCommand(Color.FromArgb(0, 255, 0)),
                        new CharHTMLCommand {Char = 'c'},
                        new NewLineHTMLCommand(),
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            string expectedHtml = "<pre><font style='color:#8B0000'>a</font>" +
                                        "<font style='color:#00008B'>b</font>" +
                                        "<font style='color:#008B00'>c</font></pre>" +
                                        "<pre><font style='color:#008B00'>d";
            Assert.AreEqual(expectedHtml, actualHtml);
        }

        [TestMethod]
        public void TextBackgroundGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new BackgroundColorCommand(Color.Red),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='background-color:#8B0000'>cd", actualHtml);
        }

        [TestMethod]
        public void BoldGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new BackgroundColorCommand(Color.Red),
                        new SetBoldCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='color:#FFFFFF;background-color:#FF0000'>cd", actualHtml);
        }


        [TestMethod]
        public void ItalicGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new SetItalicCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='font-style:italic'>cd", actualHtml);
        }

        [TestMethod]
        public void UnderlineGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new SetUnderlineCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='text-decoration:underline'>cd", actualHtml);
        }

        [TestMethod]
        public void ReverseVideoGetsAppliedMidline()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new SetReverseVideoCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='color:#000000;background-color:#8B8B8B'>cd", actualHtml);
        }

        [TestMethod]
        public void StrikethroughGetsAppliedMidLine()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new SetStrikethroughCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='text-decoration:line-through'>cd", actualHtml);
        }

        [TestMethod]
        public void StrikethroughCombinesWithLinethrough()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new SetUnderlineCommand(true),
                        new SetStrikethroughCommand(true),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );

            Assert.AreEqual("<pre>ab<font style='text-decoration:line-through underline'>cd", actualHtml);
        }

        [TestMethod]
        public void ResetRevertsToDefaultFonts()
        {
            var decoder = new ANSICommandToHTMLConverter(null);
            string actualHtml = decoder.ConvertFromInternalFormat(
                    new IANSICommand[] { 
                        new ForegroundColorCommand(Color.Red),
                        new CharHTMLCommand { Char = 'a' },
                        new CharHTMLCommand { Char = 'b' },
                        new ResetCommand(),
                        new CharHTMLCommand {Char = 'c'},
                        new CharHTMLCommand { Char = 'd' }
                    }
                );
            Assert.AreEqual("<pre><font style='color:#8B0000'>ab</font>cd", actualHtml);
        }
    }
}
