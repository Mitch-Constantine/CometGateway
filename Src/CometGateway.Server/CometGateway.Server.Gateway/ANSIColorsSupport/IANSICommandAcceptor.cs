using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public interface IANSICommandAcceptor
    {
        void AcceptChar(char c);
        void AcceptNewLine();

        void SetBackgroundColor(Color color);
        void SetForegroundColor(Color color);
        void Reset();

        void SetBold(bool enabled);
        void SetItalic(bool enabled);
        void SetReverseVideo(bool enabled);
        void SetStrikethrough(bool enabled);
        void SetUnderline(bool enabled);
    }
}
