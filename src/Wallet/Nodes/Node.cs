using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Interfaces;

namespace Wallet.Nodes
{
    public abstract class Node : INode
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _fetcherUrl;
        protected readonly string _pusherUrl;
        protected readonly string? _apiKey;

        public Node(string fetcherUrl,string pusherUrl,string? apiKey)
        {
            this._httpClient = new HttpClient();
            this._fetcherUrl = fetcherUrl;
            this._pusherUrl = pusherUrl;
            if(apiKey is not null)
            {
                this._apiKey = apiKey;
                _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            }
        }

        public abstract Task<decimal> GetAddressBalanceAsync(string address);
        public abstract Task<string> BroadcastTransactionAsync(string hexTx); 


    }
}
