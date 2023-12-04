using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.ErrorHandling
{
    public class WalletException : Exception
    {
        public WalletException(string msg) : base(msg)
        {

        }
    }
}
