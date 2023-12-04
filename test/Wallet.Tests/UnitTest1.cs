using Wallet.Interfaces;
using Wallet.Interfaces.BtcBased;
using Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased;
using Wallet.Wallets;

namespace WalletTests;

public class UnitTest1
{
    private readonly IBtcBasedNode node = new NowNodeBtcBasedNode("https://btcbook-testnet.nownodes.io/api/v2/utxo/", "https://btcbook-testnet.nownodes.io/api/v2/sendtx/", "347a90ff-48f0-40a7-afd1-4404235f811f");
    [Fact]
    public async void Test1()
    {
        var res = await node.GetAddressBalanceAsync("mfecaPyYZDQLP8kCFAAxBTAYeVy6eDn8Ks");
        Assert.True(res > 0m);

    }

    [Fact]
    public async void Test2()
    {
        IWallet wallet = new BtcBasedWallet("holle197", Wallet.Enums.Networks.BtcTestnet, NBitcoin.ScriptPubKeyType.Legacy, node);
        var res = await wallet.BroadcastTransactionAsync("tb1q3m7yz726mcm3rspdg7zr76y6cn30hdeu469yu6", 0.00001m, 0.00001m);

        Assert.True(!string.IsNullOrEmpty(res));
    }
}