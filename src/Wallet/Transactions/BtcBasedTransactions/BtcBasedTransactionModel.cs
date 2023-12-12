using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.ErrorHandling;
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
        public List<BitcoinAddress> DestinationAddresses { get; set; } = new();
        public List<decimal> Amounts { get; set; } = new();
        public decimal Fee { get; set; }

        public BtcBasedTransactionModel(Network network, Key key, string address, ScriptPubKeyType addrType, IEnumerable<IUnspentTransactionOutput> utxos, string DestinationAddr, decimal amount, decimal fee)
        {
            this.Network = network;
            this.Key = key;
            this.Address = BitcoinAddress.Create(address, Network);
            this.AddrType = addrType;
            this.Utxos = utxos;
            try
            {
                this.DestinationAddresses.Add(BitcoinAddress.Create(DestinationAddr, Network));

            }
            catch (Exception)
            {

                throw new WalletException("Invalid Address.");
            }
            this.Amounts.Add(amount);
            this.Fee = fee;
            CheckEquality();
            CheckValidation();
        }

        public BtcBasedTransactionModel(Network network, Key key, string address, ScriptPubKeyType addrType, IEnumerable<IUnspentTransactionOutput> utxos, List<string> destinationAddress, List<decimal> amount, decimal fee)
        {
            this.Network = network;
            this.Key = key;
            this.Address = BitcoinAddress.Create(address, Network);
            this.AddrType = addrType;
            this.Utxos = utxos;
            this.DestinationAddresses = CreateDestinationsAddresses(destinationAddress);
            this.Amounts = amount;
            this.Fee = fee;
            CheckEquality();
            CheckValidation();
        }

        public decimal GetTotalAmount()
        {
            var total = 0m;
            foreach (var i in this.Amounts)
            {
                total += i;
            }
            return total + this.Fee;
        }

        private List<BitcoinAddress> CreateDestinationsAddresses(List<string> destiantionAddresses)
        {
            var addresses = new List<BitcoinAddress>();
            foreach (var i in destiantionAddresses)
            {
                try
                {
                    addresses.Add(BitcoinAddress.Create(i, this.Network));
                }
                catch (Exception)
                {

                    throw new WalletException("Invalid Address");
                }
            }
            return addresses;

        }

        //Check equality of number of addresses and number of amount
        private void CheckEquality()
        {
            if (this.Amounts.Count == this.DestinationAddresses.Count) return;

            throw new WalletException("Number Of Addresses And Number Of Amounts Are Not Equal.");
        }

        //Check validation of amounts.Amount cannot be 0 or negative
        //Every blockchain have own rule for minimum amount for sending (btc 8 decimals,ethereum 0)
        private void CheckValidation()
        {
            foreach (var i in this.Amounts)
            {
                if (i <= 0m) throw new WalletException("Amount Cannot Be 0.");
            }
        }


    }
}
