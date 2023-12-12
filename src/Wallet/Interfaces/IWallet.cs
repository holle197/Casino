using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Enums;

namespace Wallet.Interfaces
{
    public interface IWallet 
    {
        Task<decimal> GetBalanceAsync();
        Task<string> BroadcastTransactionAsync(string destinationAddress,decimal amount,decimal fee);
        Task<string> BroadcastTransactionAsync(List<string> destinationAddress, List<decimal> amount, decimal fee);

        string GetAddress();
        //returns string of wallet import format - private key
        string GetWIF();
        Networks GetNetwork();
        
    }
}
