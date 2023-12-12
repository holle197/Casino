using NBitcoin;
using NBitcoin.Altcoins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Enums;
using Wallet.ErrorHandling;
using Wallet.Interfaces;
using Wallet.Interfaces.BtcBased;
using Wallet.Transactions.BtcBasedTransactions;

namespace Wallet.Wallets
{
    public sealed class BtcBasedWallet : IWallet
    {
        private readonly IBtcBasedNode _node;
        private Network Network { get; }
        private Key Key { get; }
        private ScriptPubKeyType AddressType { get; }
        private Networks Networks { get; }

        public BtcBasedWallet(string secret, Networks network, ScriptPubKeyType addrTypee, IBtcBasedNode node)
        {
            this.Network = SetNetwork(network);
            this.Key = new Key(Wallet.PrivateKeyGenerator.HASH.SHA256Hashing.GenerateHashBytes(secret));
            this.AddressType = addrTypee;
            this.Networks = network;
            this._node = node;
        }

        public string GetAddress()
        {
            return Key.GetAddress(AddressType, Network).ToString();
        }
        public string GetWIF()
        {
            return Key.GetWif(Network).ToString();
        }

        public Networks GetNetwork()
        {
            return this.Networks;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            return await _node.GetAddressBalanceAsync(GetAddress());
        }
        public async Task<string> BroadcastTransactionAsync(string destinationAddress, decimal amount, decimal fee)
        {
            var exceptionMsg = "Insufficient Funds.";
            var utxos = await _node.GetUnspentTransactionsOutputsAsync(GetAddress());
            if (utxos is null || !utxos.Any()) throw new WalletException(exceptionMsg);
            var txModel = new BtcBasedTransactionModel(Network, Key, GetAddress(), AddressType, utxos, destinationAddress, amount, fee);
            TxInputValidation(txModel);

            if (!HaveEnoughFunds(txModel)) throw new WalletException(exceptionMsg);

            var singedTransaction = BtcBasedRawTransaction.CreateAndSignTransaction(txModel);

            return await _node.BroadcastTransactionAsync(singedTransaction);
        }

        public async Task<string> BroadcastTransactionAsync(List<string> destinationAddress, List<decimal> amount, decimal fee)
        {
            var exceptionMsg = "Insufficient Funds.";
            var utxos = await _node.GetUnspentTransactionsOutputsAsync(GetAddress());
            if (utxos is null || !utxos.Any()) throw new WalletException(exceptionMsg);
            var txModel = new BtcBasedTransactionModel(Network, Key, GetAddress(), AddressType, utxos, destinationAddress, amount, fee);
            TxInputValidation(txModel);

            if (!HaveEnoughFunds(txModel)) throw new WalletException(exceptionMsg);

            var singedTransaction = BtcBasedRawTransaction.CreateAndSignTransaction(txModel);

            return await _node.BroadcastTransactionAsync(singedTransaction);
        }


        private void TxInputValidation(BtcBasedTransactionModel txModel)
        {
            CheckAmounts(txModel);
            if (txModel.Fee < 0.00000001m) throw new WalletException("The smallest fee must be greater or equal to 0.00000001");
            CheckDestinationsAddresses(txModel);
        }

        private static void CheckAmounts(BtcBasedTransactionModel txModel)
        {
            if (txModel.Amounts.Count == 0) throw new WalletException("Destination Address Are Not Found.");
            foreach (var i in txModel.Amounts)
            {
                if (i <= 0m) throw new WalletException("Amount Cannot Be Less Or Equal To 0.");
            }
        }
        private void CheckDestinationsAddresses(BtcBasedTransactionModel txModel)
        {
            if (txModel.DestinationAddresses.Count == 0) throw new WalletException("Destination Address Not Found");
            foreach (var i in txModel.DestinationAddresses)
            {
                if (i == Key.GetAddress(this.AddressType, this.Network)) throw new WalletException("Destination Address Cannot Be The Same As Origin.");
            }
        }

        //Checks the account for sufficient balances for the next transaction
        private static bool HaveEnoughFunds(BtcBasedTransactionModel txModel)
        {
            decimal total = 0m;
            foreach (var utxo in txModel.Utxos)
            {
                total += Convert.ToDecimal(utxo.GetValue());
            }
            return total >= txModel.GetTotalAmount();
        }

        private static Network SetNetwork(Networks network)
        {
            return network switch
            {
                Networks.BtcMainnet => Bitcoin.Instance.Mainnet,
                Networks.LtcMainnet => Litecoin.Instance.Mainnet,
                Networks.DogeMainnet => Dogecoin.Instance.Mainnet,
                Networks.BtcTestnet => Bitcoin.Instance.Testnet,
                Networks.LtcTestnet => Litecoin.Instance.Testnet,
                Networks.DogeTestnet => Dogecoin.Instance.Testnet,

                _ => throw new WalletException("Invalid Network")
            };
        }


    }
}
