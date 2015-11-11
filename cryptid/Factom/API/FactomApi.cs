using System;
using System.Collections.Generic;
using cryptid.Factom.API;
using Newtonsoft.Json;
using RestSharp;

namespace Cryptid.Factom.API
{
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    public class FactomApi
    {
        private const String ServerHost = "localhost";
        private const int ServerPort = 8088;
        private const string ZeroHash = "0000000000000000000000000000000000000000000000000000000000000000";

        private RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");

        /// <summary>
        /// Takes in an entry chain hash and returns Key MR of the first entry.
        /// </summary>
        /// <param name="hash">ChainID</param>
        /// <returns>KeyMR of first entry (last in list)</returns>
        public ChainHeadData GetChainHead(String hash) {
            var req = new RestRequest("/chain-head/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash);

            IRestResponse resp = client.Execute(req);

            ChainHeadData chainHead = JsonConvert.DeserializeObject<ChainHeadData>(resp.Content);
            return chainHead;
        }

        /// <summary>
        /// Returns an EntryBlockData
        /// </summary>
        /// <param name="hash">Chainhead</param>
        /// <returns>EntryBlockData</returns>
        public EntryBlockData GetEntryBlockByKeyMR(ChainHeadData keyMR) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.GET);
            req.AddUrlSegment("hash", keyMR.ChainHead);

            IRestResponse resp = client.Execute(req);
            EntryBlockData entryBlock = JsonConvert.DeserializeObject<EntryBlockData>(resp.Content);
            return entryBlock;
        }

        /// <summary>
        /// Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a string
        /// </summary>
        /// <param name="hash">Entry hash</param>
        /// <returns></returns>
        public EntryData GetEntryData(EntryBlockData.EntryData entry) {
            var req = new RestRequest("/entry-by-hash/{hash}", Method.GET);
            req.AddUrlSegment("hash", entry.EntryHash);

            IRestResponse resp = client.Execute(req);
            EntryData entryType = JsonConvert.DeserializeObject<EntryData>(resp.Content); ;
            return entryType;
        }

        public List<EntryData> GetAllChainEntries(ChainHeadData chainHead) {
            EntryBlockData block = GetEntryBlockByKeyMR(chainHead);
            EntryBlockData nextBlock = block;
            List<EntryBlockData.EntryData> dataList = new List<EntryBlockData.EntryData>();
            while (nextBlock.Header.PrevKeyMr != ZeroHash) {
                
            }
            return null;
        }

    }
}