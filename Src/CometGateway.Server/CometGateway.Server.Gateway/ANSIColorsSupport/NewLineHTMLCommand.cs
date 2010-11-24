using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class NewLineHTMLCommand : IANSICommand
    {
        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.AcceptNewLine();
        }

        public override bool Equals(object obj)
        {
            return obj is NewLineHTMLCommand;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
