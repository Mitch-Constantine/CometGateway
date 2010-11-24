using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
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
