using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class SetStrikethroughCommand : IANSICommand
    {
        private bool enabled;

        public SetStrikethroughCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is SetStrikethroughCommand &&
                   (obj as SetStrikethroughCommand).enabled == enabled;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode();
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetStrikethrough(enabled);
        }
    }
}
