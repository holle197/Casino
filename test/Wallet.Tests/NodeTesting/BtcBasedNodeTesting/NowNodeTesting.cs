using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces.BtcBased;
using Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased;

namespace WalletTests.NodeTesting.BtcBasedNodeTesting
{
    public class NowNodeTesting
    {
        private readonly IBtcBasedNode node = new NowNodeBtcBasedNode("https://btcbook-testnet.nownodes.io/api/v2/utxo/", "https://btcbook-testnet.nownodes.io/api/v2/sendtx/", "347a90ff-48f0-40a7-afd1-4404235f811f");
        [Fact]
        public async void TestingUtxosFetcher_OnSuccess_ExpectedTrue()
        {
            var res = await node.GetAddressBalanceAsync("mfecaPyYZDQLP8kCFAAxBTAYeVy6eDn8Ks");
            Assert.True(res > 0m);
        }
    }
}
