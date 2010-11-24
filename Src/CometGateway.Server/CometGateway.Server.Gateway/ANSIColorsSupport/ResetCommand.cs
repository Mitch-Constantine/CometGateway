using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class ResetCommand : IANSICommand
    {
        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.Reset();
        }

        public override bool Equals(object obj)
        {
            return obj is ResetCommand;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
