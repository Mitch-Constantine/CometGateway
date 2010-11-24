using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class SetReverseVideoCommand : IANSICommand
    {
        private bool enabled;
        public SetReverseVideoCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is SetReverseVideoCommand &&
                   (obj as SetReverseVideoCommand).enabled == enabled;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode();
        }

        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.SetReverseVideo(enabled);
        }
    }
}
