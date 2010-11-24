using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class SetBoldCommand : IANSICommand
    {
        private bool enabled;
        public SetBoldCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is SetBoldCommand &&
                   (obj as SetBoldCommand).enabled == enabled;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode();
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetBold(enabled);
        }
    }
}
