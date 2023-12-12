using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces;
using Wallet.Interfaces.BtcBased;
using Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased;
using Wallet.Wallets;

namespace WalletTests.WalletTesting.BtcBasedWalletTesting
{
    public class BtcBasedWalletTesting
    {
        private readonly IBtcBasedNode NowNode = new NowNodeBtcBasedNode("https://btcbook-testnet.nownodes.io/api/v2/utxo/", "https://btcbook-testnet.nownodes.io/api/v2/sendtx/", "347a90ff-48f0-40a7-afd1-4404235f811f");

        [Fact]
        public async void TestingBroadcastingTransactionsToOneAddress_OnSuccess_ExpectedTrue()
        {
            IWallet wallet = new BtcBasedWallet("holle197", Wallet.Enums.Networks.BtcTestnet, NBitcoin.ScriptPubKeyType.Legacy, NowNode);
            var res = await wallet.BroadcastTransactionAsync("tb1q3m7yz726mcm3rspdg7zr76y6cn30hdeu469yu6", 0.00001m, 0.00001m);

            Assert.True(!string.IsNullOrEmpty(res));
        }

        [Fact]
        public async void TestingBroadcastingTransactionsToMultipleAddresses_OnSuccess_ExpectedTrue()
        {
            IWallet wallet = new BtcBasedWallet("holle197", Wallet.Enums.Networks.BtcTestnet, NBitcoin.ScriptPubKeyType.Legacy, NowNode);
            var destinationAddresses = new List<string>() { "tb1q3m7yz726mcm3rspdg7zr76y6cn30hdeu469yu6", "tb1qvkuc7hmldy4dnkmsv8s08mmnkzth5r4fulwgft" };
            var amounts = new List<decimal>() { 0.0001m, 0.0002m };
            var res = await wallet.BroadcastTransactionAsync(destinationAddresses, amounts, 0.0001m);

            Assert.True(!string.IsNullOrEmpty(res));
        }
    }
}
