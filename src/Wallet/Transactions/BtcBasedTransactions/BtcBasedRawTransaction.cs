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


        private static IEnumerable<IUnspentTransactionOutput> PickBestUtxos(IEnumerable<IUnspentTransactionOutput> utxos, decimal totalAmount)
        {
            var sortedUtxos = utxos.OrderBy(utxo => utxo.GetValue()).ToList();
            decimal currentTotal = 0;
            List<IUnspentTransactionOutput> favorableUtxos = new List<IUnspentTransactionOutput>();

            foreach (var utxo in sortedUtxos)
            {
                if (currentTotal >= totalAmount) break;

                if (currentTotal + Convert.ToDecimal(utxo.GetValue()) <= totalAmount)
                {
                    currentTotal += Convert.ToDecimal(utxo.GetValue());
                    favorableUtxos.Add(utxo);
                }
                else
                {
                   
                }
            }
            return favorableUtxos;
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
            var moneyToSend = CreateMoney(txModel.Amounts);

            if (rest > 0)
            {
                var restMoney = new Money(rest, MoneyUnit.BTC);
                AddMoney(moneyToSend, txModel.DestinationAddresses, tx);
                tx.Outputs.Add(restMoney, txModel.Address.ScriptPubKey);
                SignInputs(tx, bestUtxosForTx, txModel);
                return;
            }

            AddMoney(moneyToSend, txModel.DestinationAddresses, tx);
            SignInputs(tx, bestUtxosForTx, txModel);
        }

        private static List<Money> CreateMoney(List<decimal> amounts)
        {
            var money = new List<Money>();
            if (amounts is null || amounts.Count == 0) throw new WalletException("Insufficient Balance.");
            foreach (var i in amounts)
            {
                money.Add(new Money(i, MoneyUnit.BTC));
            }
            return money;
        }
        private static void AddMoney(List<Money> money,List<BitcoinAddress> destinationsAddresses,Transaction tx)
        {
            if (money.Count != destinationsAddresses.Count) throw new WalletException("Equality Error.");
            for (int i = 0; i < money.Count; i++)
            {
                tx.Outputs.Add(money[i], destinationsAddresses[i].ScriptPubKey);
            }
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
