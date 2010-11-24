using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public class TelnetConnection : ProtocolLayer<byte[], byte[], byte[], byte[]>, ITelnetConnection
    {
        private ITelnetStateMachine TelnetStateMachine { get; set; }
        
        public TelnetConnection(
            ISocketConnection connection, 
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
            List<byte> sendBack;
            List<byte> actualData;

            Process(data, out sendBack, out actualData);
            
            if (sendBack.Any())
                Send(sendBack.ToArray());
            return actualData.ToArray();
        }

        internal void Process(byte[] data, out List<byte> sendBack, out List<byte> actualData)
        {
            sendBack = new List<byte>();
            actualData = new List<byte>();

            foreach (var byteCrt in data)
            {
                byte[] sendBackCrt;
                bool isActualData;
                TelnetStateMachine.HandleIncomingByte(
                    byteCrt,
                    out sendBackCrt,
                    out isActualData
                );
                sendBack.AddRange(sendBackCrt);
                if (isActualData)
                    actualData.Add(byteCrt);
            }
        }
    }
}
