using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces.BtcBased;

namespace Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased.Data
{
    public class NowNodeUnspentTransactionOutput : IUnspentTransactionOutput
    {
#pragma warning disable IDE1006
        public string txid { get; set; } = string.Empty;
        public uint vout { get; set; }
        public string value { get; set; } = string.Empty;
#pragma warning restore IDE1006

        public uint GetOutputNo()
        {
            return vout;
        }

        public string GetTransactionId()
        {
            return txid;
        }

        public decimal GetValue()
        {
            //convert string  to decimal with 8 decimal points
            return Math.Round(Convert.ToDecimal(value) / 100000000, 8);
        }
    }
}
