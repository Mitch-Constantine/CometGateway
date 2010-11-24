using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class SetUnderlineCommand : IANSICommand
    {
        private bool enabled;
        public SetUnderlineCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is SetUnderlineCommand &&
                   (obj as SetUnderlineCommand).enabled == enabled;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode();
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetUnderline(enabled);
        }
    }
}
