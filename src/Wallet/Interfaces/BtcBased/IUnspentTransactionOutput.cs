using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Interfaces.BtcBased
{
    public interface IUnspentTransactionOutput
    {
        //hexadecimal id of transaction
        string GetTransactionId();
        //amount
        public decimal GetValue();
        //number of position
        public uint GetOutputNo();
    }
}
