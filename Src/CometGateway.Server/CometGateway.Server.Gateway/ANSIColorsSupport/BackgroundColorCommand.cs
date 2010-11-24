using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class BackgroundColorCommand : IANSICommand
    {
        private Color color;

        public BackgroundColorCommand(Color color)
        {
            this.color = color;
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetBackgroundColor(color);
        }

        public override bool Equals(object obj)
        {
            return (obj is BackgroundColorCommand) &&
                   (obj as BackgroundColorCommand).color == color;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
    }
}
