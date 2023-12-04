using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Interfaces
{
    public interface INode
    {
        Task<decimal> GetAddressBalanceAsync(string address);
        Task<string> BroadcastTransactionAsync(string txHex);
    }
}
