using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CometGateway.Server.Gateway.Infrastructure;
using CometGateway.Server.Gateway.Telnet;

namespace CometGateway.Server.Gateway.Telnet
{
    public class BytesToStringConversionLayer : ProtocolLayer<string, string, byte[], byte[]>, IRawStringConnection
    {
        public BytesToStringConversionLayer(ITelnetConnection connection)
            : base(connection)
        {
        }

        internal override byte[] ConvertToInternalFormat(string data)
        {
            return ASCIIEncoding.ASCII.GetBytes(data);          
        }

        internal override string ConvertFromInternalFormat(byte[] data)
        {
            return ASCIIEncoding.ASCII.GetString(data);
        }
    }
}
