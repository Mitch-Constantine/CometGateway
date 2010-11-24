using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.Telnet
{
    public interface ITelnetStateMachine
    {
        void HandleIncomingByte(
            byte incomingData,
            out byte[] sendBack,
            out bool isRealData
        );
    }
}
