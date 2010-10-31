using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway
{
    public class MessageTypeAttribute : Attribute
    {
        public string TypeCode { get; set; }

        public MessageTypeAttribute(string typeCode)
        {
            TypeCode = typeCode;
        }
    }
}
