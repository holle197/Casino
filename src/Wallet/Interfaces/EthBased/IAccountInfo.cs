﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Interfaces.EthBased
{
    internal class IAccountInfo
    {
        public decimal Balance { get; set; }
        public uint TransactionsSentCount { get; set; }
    }
}
