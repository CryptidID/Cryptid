using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cryptid.Factom.API
{
    public class Chain
    {
        private class ChainHeadDto
        {
            public string ChainHead { get; set; }
        }

        public string Server = "http://localhost:8088";

        /*
         * Returns Key MR of the first entry in the block
         */
        public string GetChainHead(string hash) {
                var request = (HttpWebRequest)WebRequest.Create(Server + "/v1/chain-head/" + hash);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
         }

        public string GetEntryBlockByKeyMR(string key)
        {
            var request = (HttpWebRequest)WebRequest.Create(Server + "/v1/entry-block-by-keymr/" + key);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ChainHeadDto>(responseString);
            return result.ChainHead;
        }

    }
}
