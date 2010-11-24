using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class SetItalicCommand : IANSICommand
    {
        private bool enabled;
        public SetItalicCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is SetItalicCommand &&
                   (obj as SetItalicCommand).enabled == enabled;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode();
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetItalic(enabled);
        }
    }
}
