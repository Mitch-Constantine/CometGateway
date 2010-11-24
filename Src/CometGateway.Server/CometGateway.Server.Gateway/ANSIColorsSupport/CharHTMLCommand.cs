using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CometGateway.Server.Gateway.ANSIColorsSupport
{
    internal class CharHTMLCommand : IANSICommand
    {
        public char Char { get; set; }


        public void ApplyTo(IANSICommandAcceptor acceptor)
        {
            acceptor.AcceptChar(Char);
        }

        public override bool Equals(object obj)
        {
            var otherCharCommand = this as CharHTMLCommand;
            if (otherCharCommand == null)
                return false;
            return Char == otherCharCommand.Char;
        }

        public override int GetHashCode()
        {
            return Char.GetHashCode();
        }
    }
}
