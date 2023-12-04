using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Enums
{
    public enum Networks
    {
        //btc-based
        BtcMainnet,
        LtcMainnet,
        DogeMainnet,

        //eth-based
        EthMainnet,
        EtcMainnet,

        //testnets
        BtcTestnet,
        LtcTestnet,
        DogeTestnet,

        EthTetnet,
        EtcTestnet,
    }
}
