using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces.BtcBased;

namespace Wallet.Transactions.BtcBasedTransactions
{
    internal class BtcBasedTransactionModel
    {
        public Network Network { get; set; }
        public Key Key { get; set; }
        public BitcoinAddress Address { get; set; }
        public ScriptPubKeyType AddrType { get; set; }
        public IEnumerable<IUnspentTransactionOutput> Utxos { get; set; }
        public BitcoinAddress DestinationAddr { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }

        public BtcBasedTransactionModel(Network network, Key key, string address, ScriptPubKeyType addrType, IEnumerable<IUnspentTransactionOutput> utxos, string DestinationAddr, decimal amount, decimal fee)
        {
            this.Network = network;
            this.Key = key;
            this.Address = BitcoinAddress.Create(address, Network);
            this.AddrType = addrType;
            this.Utxos = utxos;
            this.DestinationAddr = BitcoinAddress.Create(DestinationAddr, Network);
            this.Amount = amount;
            this.Fee = fee;
        }

        public decimal GetTotalAmount()
        {
            return Amount + Fee;
        }
    }
}
