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


        private void TxInputValidation(BtcBasedTransactionModel txModel)
        {
            if (txModel.Amount < 0.00000001m) throw new WalletException("The smallest transfer amount must be greater or equal to 0.00000001");
            if (txModel.Fee < 0.00000001m) throw new WalletException("The smallest fee must be greater or equal to 0.00000001");
            if (txModel.DestinationAddr == Key.GetAddress(AddressType, Network)) throw new WalletException("You cannot send a transaction to your own address");

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
