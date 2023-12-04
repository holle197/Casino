using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Interfaces.BtcBased
{
    public interface IBtcBasedNode : INode
    {
        //this method is used to prepare transactions for signing and broadcasting
        Task<IEnumerable<IUnspentTransactionOutput>?> GetUnspentTransactionsOutputsAsync(string address);
    }
}
