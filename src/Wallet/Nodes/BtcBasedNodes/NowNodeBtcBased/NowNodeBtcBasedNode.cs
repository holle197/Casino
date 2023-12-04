using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.ErrorHandling;
using Wallet.Interfaces.BtcBased;
using Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased.Data;

namespace Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased
{
    public class NowNodeBtcBasedNode : Node, IBtcBasedNode
    {
        public NowNodeBtcBasedNode(string fetcherUrl, string pusherUrl, string? apiKey) : base(fetcherUrl, pusherUrl, apiKey)
        {
        }

        public async override Task<string> BroadcastTransactionAsync(string hexTx)
        {
            try
            {
                var apiCall = await base._httpClient.GetAsync(base._pusherUrl + hexTx);
                var asStr = await apiCall.Content.ReadAsStringAsync();
                var hash = JsonConvert.DeserializeObject<NowNodeBtcBasedTransactionResult>(asStr);

                return hash.result;
            }
            catch (Exception e)
            {

                throw new NodeException("Node Cannot Broadcast Transactions.\n"+e.Message);
            }
        }


        public override async Task<decimal> GetAddressBalanceAsync(string address)
        {
            var utxos = await GetUnspentTransactionsOutputsAsync(address);
            //Any() is used insted of Count() coz of performance
            if (utxos is null || !utxos.Any()) return 0m;
            return ConvertUtxosToBalance(utxos);
        }

        public async Task<IEnumerable<IUnspentTransactionOutput>?> GetUnspentTransactionsOutputsAsync(string address)
        {
            try
            {
                var apiCall = await base._httpClient.GetAsync(_fetcherUrl + address);
                var asString = await apiCall.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<NowNodeUnspentTransactionOutput>>(asString);
            }
            catch (Exception e)
            {

                throw new NodeException("Node Cannot Fetch Address Data (UTxOs).\n" + e.Message);
            }
        }

        private static decimal ConvertUtxosToBalance(IEnumerable<IUnspentTransactionOutput> utxos)
        {
            var res = 0m;
            foreach (var i in utxos)
            {
                res += i.GetValue();
            }
            return res;
        }

    }
}
