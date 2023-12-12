// See https://aka.ms/new-console-template for more information
using Wallet.Interfaces;
using Wallet.Interfaces.BtcBased;
using Wallet.Nodes.BtcBasedNodes.NowNodeBtcBased;
using Wallet.Wallets;

 IBtcBasedNode node = new NowNodeBtcBasedNode("https://btcbook-testnet.nownodes.io/api/v2/utxo/", "https://btcbook-testnet.nownodes.io/api/v2/sendtx/", "347a90ff-48f0-40a7-afd1-4404235f811f");
IWallet wallet = new BtcBasedWallet("holle197", Wallet.Enums.Networks.BtcTestnet, NBitcoin.ScriptPubKeyType.Legacy, node);
var destinationAddresses = new List<string>() { "tb1q3m7yz726mcm3rspdg7zr76y6cn30hdeu469yu6", "tb1qvkuc7hmldy4dnkmsv8s08mmnkzth5r4fulwgft" };
var amounts = new List<decimal>() { 0.0001m, 0.0002m };
var res = await wallet.BroadcastTransactionAsync(destinationAddresses, amounts, 0.0001m);
Console.WriteLine(res);