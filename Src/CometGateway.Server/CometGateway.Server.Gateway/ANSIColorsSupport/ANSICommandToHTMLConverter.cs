using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CometGateway.Server.Gateway;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public class ANSICommandToHTMLConverter : 
        ProtocolLayer<string, string, IANSICommand[], string>,
        IANSICommandAcceptor,
        IHTMLConnection
    {
        string htmlBuffer;
        bool afterNewLine = true;
        Color color;
        Color backgroundColor;
        bool afterFontChange;
        bool fontTagGenerated;
        bool boldEnabled;
        bool italicEnabled;
        bool underlineEnabled;
        bool reverseVideoEnabled;
        bool strikethroughEnabled;

        public ANSICommandToHTMLConverter(
            IConnection<IANSICommand[], string> underlyingConnection) :
            base(underlyingConnection)
        {
            SetDefaultStyling();
        }         

        internal override string ConvertToInternalFormat(string data)
        {
            return data;
        }

        internal override string ConvertFromInternalFormat(IANSICommand[] data)
        {
            foreach (IANSICommand command in data)
                command.ApplyTo(this);
            string htmlBufferCopy = htmlBuffer;
            htmlBuffer = "";
            return htmlBufferCopy;
        }

        void IANSICommandAcceptor.AcceptChar(char c)
        {
            if (afterNewLine)
            {
                htmlBuffer += "<pre>";
                if (GetStyles().Any())
                { 
                    htmlBuffer += FormattedTag("font");
                    fontTagGenerated = true;
                } 
                afterFontChange = false;
            }
            if (afterFontChange)
            {
                if (fontTagGenerated)
                { 
                    htmlBuffer += "</font>";
                    fontTagGenerated = false;
                }
                if (GetStyles().Any())
                {
                    fontTagGenerated = true;
                    htmlBuffer += FormattedTag("font");
                } 
            } 
            htmlBuffer += HttpUtility.HtmlEncode(c);
            afterNewLine = false;
            afterFontChange = false;
        }

        void IANSICommandAcceptor.AcceptNewLine()
        {
            if (fontTagGenerated) 
                htmlBuffer += "</font>";
            if (afterNewLine)
                htmlBuffer += FormattedTag("pre") + "&nbsp;";
            htmlBuffer += "</pre>";
            afterFontChange = false;
            afterNewLine = true;
            fontTagGenerated = false;
        }

        public void SetBackgroundColor(Color color)
        {
            backgroundColor = color;
            afterFontChange = true;
        }

        public void SetForegroundColor(Color color)
        {
            this.color = color;
            afterFontChange = true;
        }

        public void Reset()
        {
            SetDefaultStyling();
        }

        public void SetBold(bool enabled)
        {
            boldEnabled = enabled;
            afterFontChange = true;
        }

        public void SetItalic(bool enabled)
        {
            italicEnabled = enabled;
            afterFontChange = true;
        }

        public void SetReverseVideo(bool enabled)
        {
            reverseVideoEnabled = enabled;
            afterFontChange = true;
        }

        public void SetStrikethrough(bool enabled)
        {
            strikethroughEnabled = enabled;
            afterFontChange = true;
        }

        public void SetUnderline(bool enabled)
        {
            underlineEnabled = enabled;
            afterFontChange = true;
        }

        static Color ApplyBold(Color color, bool bold)
        {
            return Color.FromArgb(
                    ApplyBold(color.R, bold),
                    ApplyBold(color.G, bold),
                    ApplyBold(color.B, bold)
                );
        }

        private static int ApplyBold(byte color, bool bold)
        {
            return bold ? color : color * 0x8b / 0xff;
        }

        private string FormattedTag(string tag)
        {
            var styles = GetStyles();

            string tagBody = tag;
            if (styles.Any())
            {
                var arrStrStyles = 
                    (from style in styles
                     select String.Format("{0}:{1}", style.Item1, style.Item2))
                     .ToArray();
                string styleAttribute =
                    String.Format("style='{0}'",
                        String.Join(";", arrStrStyles)
                    );

                tagBody += " " + styleAttribute;
            }
            return "<" + tagBody + ">";
        }

        private List<Tuple<string, string>> GetStyles()
        {
            var styles = new List<Tuple<string, string>>();

            Color foregroundColor = ApplyBold(color, boldEnabled);
            Color actualBackgroundColor = ApplyBold(this.backgroundColor, boldEnabled);
            if (reverseVideoEnabled)
            {
                Color temp = foregroundColor;
                foregroundColor = actualBackgroundColor;
                actualBackgroundColor = temp;
            }
            Color defaultColor = ApplyBold(Color.White, false);
            if (foregroundColor.ToArgb() != defaultColor.ToArgb())
            {
                styles.Add(new Tuple<string, string>("color", FormatHtml(foregroundColor)));
            }
            Color defaultBackground = Color.Black; // Bold black = black
            if (actualBackgroundColor.ToArgb() != defaultBackground.ToArgb())
            {
                styles.Add(new Tuple<string, string>("background-color", FormatHtml(actualBackgroundColor)));
            }
            if (italicEnabled)
            {
                styles.Add(new Tuple<string, string>("font-style", "italic"));
            }
            if (underlineEnabled || strikethroughEnabled)
            {
                var textDecorations = new List<string>();
                if (strikethroughEnabled)
                    textDecorations.Add("line-through");
                if (underlineEnabled)
                    textDecorations.Add("underline");
                string strTextDecorations = String.Join(" ", textDecorations.ToArray());
                styles.Add(new Tuple<string, string>("text-decoration", strTextDecorations));
            }
            return styles;
        }

        private string FormatHtml(Color color)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        private void SetDefaultStyling()
        {
            color = Color.White;
            backgroundColor = Color.Black;
            afterFontChange = false;
            boldEnabled = false;
            italicEnabled = false;
            underlineEnabled = false;
            reverseVideoEnabled = false;
            strikethroughEnabled = false;
            afterFontChange = true;
        }
    }
}
