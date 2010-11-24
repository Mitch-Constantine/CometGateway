using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CometGateway.Server.Gateway.Infrastructure;
using CometGateway.Server.Gateway.Telnet;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    public class ANSICommandsDecoder : ProtocolLayer<IANSICommand[], string, string, string>
    {
        private IANSICommandsStateMachine stateMachine;
        public ANSICommandsDecoder(IRawStringConnection underlyingConnection, IANSICommandsStateMachine stateMachine) :
            base(underlyingConnection)
        {
            this.stateMachine = stateMachine;
        }

        internal override string ConvertToInternalFormat(string data)
        {
            return data;
        }


        internal override IANSICommand[] ConvertFromInternalFormat(string data)
        {
            return data
                    .Select(c => stateMachine.Decode(c))
                    .Where(command => command != null)
                    .ToArray();
        }
    }
}
