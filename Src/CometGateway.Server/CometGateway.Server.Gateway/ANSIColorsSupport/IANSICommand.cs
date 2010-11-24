using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public interface IANSICommand
    {
        void ApplyTo(IANSICommandAcceptor acceptor);
    }
}
