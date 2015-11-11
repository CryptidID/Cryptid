using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;
using Cryptid.Utils;
using Newtonsoft.Json;
using RestSharp;

/// <summary>
/// DEPRECIATED
/// </summary>
namespace Cryptid.Factom.API {
/*

        private const String ServerHost = "localhost";
        private const int ServerPort = 8088;

        private RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");

        private struct RevealRequest {
            string Entry;

            public RevealRequest(string Entry) {
                this.Entry = Entry;
            }
        }

        public String RevealChain(EntryData entry) {
            //http://localhost:8088/v1/reveal-chain/?
            //byte[] entryBytes = Serialization.GetBytes<EntryData>(entry);
            byte[] entryBytes = Serialization.SerializeMessage<EntryData>(entry);

            string entryHex = BitConverter.ToString(entryBytes);
            entryHex = entryHex.Replace("-", "");
            Console.WriteLine(entryHex);
            RevealRequest entryStruct = new RevealRequest(entryHex);

            var req = new RestRequest("/reveal-chain/", Method.POST);
            req.AddParameter("application/json", entryStruct, ParameterType.RequestBody);

            IRestResponse resp = client.Execute(req);
            return resp.Content;

        }


        /// <summary>
        /// Takes in an entry chain hash and returns Key MR of the first entry.
        /// </summary>
        /// <param name="hash">ChainID</param>
        /// <returns></returns>
        public ChainHeadData GetChainHead(String hash) {
            var req = new RestRequest("/chain-head/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash);

            IRestResponse resp = client.Execute(req);

            return JsonConvert.DeserializeObject<ChainHeadData>(resp.Content);
        }

        /// <summary>
        /// Returns an EntryBlockData
        /// </summary>
        /// <param name="hash"> Chainhead Hash</param>
        /// <returns></returns>
        public EntryBlockData GetEntryBlockByKeyMR(ChainHeadData hash) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash.ChainHead);

            IRestResponse resp = client.Execute(req);
            return JsonConvert.DeserializeObject<EntryBlockData>(resp.Content);
        }

        public string GetEntryBlockByKeyMRJson(String hash) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash);

            IRestResponse resp = client.Execute(req);
            return resp.Content;
        }

        /// <summary>
        /// Returns the raw data of entry hash. Reccomend using GetEntryJson instead
        /// </summary>
        /// <param name="hash">Entry hash</param>
        /// <returns></returns>
        public string GetEntryRawData(string hash) {
            var req = new RestRequest("/get-raw-data/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash);

            IRestResponse resp = client.Execute(req);
            return resp.Content;
        }

        /// <summary>
        /// Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a string
        /// </summary>
        /// <param name="hash">Entry hash</param>
        /// <returns></returns>
        public EntryData GetEntryData(string hash) {
            var req = new RestRequest("/entry-by-hash/{hash}", Method.GET);
            req.AddUrlSegment("hash", hash);

            IRestResponse resp = client.Execute(req);
            return JsonConvert.DeserializeObject<EntryData>(resp.Content); ;
        }

    }
    */
}
