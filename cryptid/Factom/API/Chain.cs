using System;
using RestSharp;

namespace Cryptid.Factom.API
{
    public class Chain {
        public class ChainHeadData {
            public string ChainHead { get; set; }
        }

        public class EntryBlockData {
            public class HeaderData {
                public int BlockSequenceNumber { get; set; }
                public string ChainID { get; set; }
                public string PrevKeyMr { get; set; }
                public int Timestamp { get; set; }
            }

            public class EntryData {
                public string EntryHash { get; set; }
                public int Timestamp { get; set; }
            }

            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }
        }

        private const String ServerHost = "localhost";
        private const int ServerPort = 8088;

        private RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort);

        ///<summary>
        /// Returns Key MR of the first entry in the block
        ///</summary>
        public ChainHeadData GetChainHead(String hash) {
            var req = new RestRequest("/chain-head/{hash}", Method.POST);
            req.AddUrlSegment("hash", hash);

            RestResponse<ChainHeadData> resp = client.Execute<ChainHeadData>(req) as RestResponse<ChainHeadData>;
            return resp.Data;
        }

        public EntryBlockData GetEntryBlockByKeyMR(String hash) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.POST);
            req.AddUrlSegment("hash", hash);

            RestResponse<EntryBlockData> resp = client.Execute<EntryBlockData>(req) as RestResponse<EntryBlockData>;
            return resp.Data;
        }

    }
}
