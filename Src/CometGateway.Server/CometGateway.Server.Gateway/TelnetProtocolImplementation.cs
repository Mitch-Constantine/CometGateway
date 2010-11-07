using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public class TelnetProtocolImplementation : ProtocolLayer<byte[], byte[]>, ITelnetConnection
    {
        private ITelnetStateMachine TelnetStateMachine { get; set; }
        
        public TelnetProtocolImplementation(
            IConnection<byte[]> connection, 
            ITelnetStateMachine telnetStateMachine
        ) :
            base(connection)
        {
            TelnetStateMachine = telnetStateMachine;            
        }
        
        internal override byte[] ConvertToInternalFormat(byte[] data)
        {
            return data;
        }

        internal override byte[] ConvertFromInternalFormat(byte[] data)
        {
            return data
                    .SelectMany(byteCrt => TelnetStateMachine.Translate(byteCrt))
                    .ToArray();
        }
    }
}
