using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public class ForegroundColorCommand : IANSICommand
    {
        private Color color;

        public ForegroundColorCommand(Color color)
        {
            this.color = color;
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetForegroundColor(color);
        }

        public override bool Equals(object obj)
        {
            var otherCommand = obj as ForegroundColorCommand;
            return otherCommand != null &&
                    otherCommand.color == color;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
    }
}
