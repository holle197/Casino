using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.ErrorHandling
{
    public class NodeException : Exception
    {
        public NodeException(string msg) : base(msg)
        {

        }
    }
}
