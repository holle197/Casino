using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces.BtcBased;

namespace Wallet.Transactions.BtcBasedTransactions
{
    internal class BtcBasedRawTransaction
    {
        /// <summary>
        /// Creating and signing new transaction
        /// </summary>
        /// <param name="txModel"></param>
        /// <returns>hexadecimal string of newly created and signed transaction ready to be broadcasted</returns>
        public static string CreateAndSignTransaction(BtcBasedTransactionModel txModel)
        {
            var bestUtxosForTx = PickBestUtxos(txModel.Utxos, txModel.GetTotalAmount());
            var transaction = Transaction.Create(txModel.Network);
            GenerateOutpoints(transaction, bestUtxosForTx);
            GenerateMoney(transaction, bestUtxosForTx, txModel);
            SignTransaction(transaction, bestUtxosForTx, txModel);
            return transaction.ToHex();
        }

        //method loop through all unsorted ienumerable of utxos and picking while amount < utxo.value
        //this is not optimised for economical use
        //later we need to decide what will be better for transaction: use much smaller utxos or big one
        //possible implementing search algorithm to match what is the best for incomming tx
        //example : 2utxos of 1btc when we sending is better to use if we sending 2btc than 1 utxos of 2.1btc => 0.1btc will be
        //added to our utxo pool 
        private static IEnumerable<IUnspentTransactionOutput> PickBestUtxos(IEnumerable<IUnspentTransactionOutput> utxos, decimal totalAmount)
        {
            decimal total = 0;
            List<IUnspentTransactionOutput> bestUtxos = new();
            foreach (var utxo in utxos)
            {
                if (total >= totalAmount) break;
                total += Convert.ToDecimal(utxo.GetValue());
                bestUtxos.Add(utxo);
            }
            return bestUtxos;
        }

        private static void GenerateOutpoints(Transaction tx, IEnumerable<IUnspentTransactionOutput> utxos)
        {
            foreach (var utxo in utxos)
            {
                OutPoint otp = OutPoint.Parse(utxo.GetTransactionId() + ":" + utxo.GetOutputNo());
                tx.Inputs.Add(otp);
            }
        }

        private static void GenerateMoney(Transaction tx, IEnumerable<IUnspentTransactionOutput> bestUtxosForTx, BtcBasedTransactionModel txModel)
        {
            decimal rest = TotalBalanceInBestUtxos(bestUtxosForTx) - txModel.GetTotalAmount();
            var moneyToSend = new Money(txModel.Amount, MoneyUnit.BTC);

            if (rest > 0)
            {
                var restMoney = new Money(rest, MoneyUnit.BTC);
                tx.Outputs.Add(moneyToSend, txModel.DestinationAddr.ScriptPubKey);
                tx.Outputs.Add(restMoney, txModel.Address.ScriptPubKey);
                SignInputs(tx, bestUtxosForTx, txModel);
                return;
            }

            tx.Outputs.Add(moneyToSend, txModel.DestinationAddr.ScriptPubKey);
            SignInputs(tx, bestUtxosForTx, txModel);
        }

        //used to check if wallet need to generate REST money to be return to the address
        private static decimal TotalBalanceInBestUtxos(IEnumerable<IUnspentTransactionOutput> utxos)
        {
            decimal total = 0;
            foreach (var utxo in utxos)
            {
                total += Convert.ToDecimal(utxo.GetValue());
            }
            return total;
        }

        private static void SignInputs(Transaction tx, IEnumerable<IUnspentTransactionOutput> utxos, BtcBasedTransactionModel txModel)
        {
            for (int i = 0; i < utxos.Count(); i++)
            {
                tx.Inputs[i].ScriptSig = txModel.Address.ScriptPubKey;
            }
        }

        private static void SignTransaction(Transaction tx, IEnumerable<IUnspentTransactionOutput> utxos, BtcBasedTransactionModel txModel)
        {
            List<ICoin> coins = new();
            foreach (var utxo in utxos)
            {
                var txInString = uint256.Parse(utxo.GetTransactionId());
                var coin = new Coin(txInString, utxo.GetOutputNo(), new Money(Convert.ToDecimal(utxo.GetValue()), MoneyUnit.BTC),
                                    txModel.Address.ScriptPubKey);
                coins.Add(coin);
            }
            tx.Sign(txModel.Key.GetWif(txModel.Network), coins);
        }
    }
}
