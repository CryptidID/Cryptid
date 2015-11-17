using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cryptid.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace cryptid.Factom.API
{
    using EntryData = DataStructs.EntryData;
    public class Chain
    {
        private const String ServerHost = "localhost";
        private const int ServerPort = 8088;
        private const int ServerPortMD = 8089;
        private const string ZeroHash = "0000000000000000000000000000000000000000000000000000000000000000";

        private RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");
        private RestClient clientMD = new RestClient("http://" + ServerHost + ":" + ServerPortMD + "/v1/");

        public class ChainType {
            public string ChainId { get; set; }
            public EntryData FirstEntry { get; set; }
        }

        public ChainType NewChain(EntryData entry) {
            ChainType c = new ChainType();
            c.FirstEntry = entry;
            List<byte> chainHash = new List<byte>();
            if (entry.ExtIDs != null) {
                foreach (string extId in entry.ExtIDs) {
                    var h = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(extId));
                    chainHash.AddRange(h);
                }
            }
            c.ChainId = Arrays.ByteArrayToHex(SHA256.Create().ComputeHash(chainHash.ToArray()));
            c.FirstEntry.ChainID = c.ChainId;
            return c;
        }

        public class WalletCommit  {
            public string Message { get; set; }
        }
        public bool CommitChain(ChainType c, string name) {
            List<byte> byteList = new List<byte>();

            //1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(Times.MilliTime());

            var entry = c.FirstEntry;

            // 32 Byte ChainID Hash
            //byte[] chainIDHash = Encoding.ASCII.GetBytes(c.ChainId);
            byte[] chainIDHash = Strings.DecodeHexIntoBytes(c.ChainId);
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            byteList.AddRange(chainIDHash);

            // 32 byte Weld; sha256(sha256(EntryHash + ChainID))
            byte[] cid = Strings.DecodeHexIntoBytes(c.ChainId);
            byte[] s = Entries.HashEntry(c.FirstEntry);
            byte[] weld = new byte[cid.Length + s.Length];
            s.CopyTo(weld, 0); cid.CopyTo(weld, s.Length);
            weld = SHA256.Create().ComputeHash(weld);
            weld = SHA256.Create().ComputeHash(weld);
            byteList.AddRange(weld);

            // 32 byte Entry Hash of the First Entry
            byteList.AddRange(Entries.HashEntry(c.FirstEntry));

            // 1 byte number of Entry Credits to pay
            sbyte cost = (sbyte)(Entries.EntryCost(entry) + 10); // TODO: check errors
            byteList.Add(BitConverter.GetBytes(cost)[0]);

            var com = new WalletCommit();
            com.Message = Arrays.ByteArrayToHex(byteList.ToArray());
            
            var json = JsonConvert.SerializeObject(com);

            

            var req = new RestRequest("/commit-chain/" + name, Method.POST);
            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            req.AddUrlSegment("name", name);
            IRestResponse resp = clientMD.Execute(req);
            Console.WriteLine("CommitChain Resp = " + resp.StatusCode);

            Console.WriteLine("Message= " + com.Message);

            return true; //TODO: False for fail
        }

        public class Reveal {
            public string Entry { get; set; }
        }

        public bool RevealChain(ChainType c) {
            Reveal r = new Reveal();
            var b = Entries.MarshalBinary(c.FirstEntry);
            r.Entry = Arrays.ByteArrayToHex(b);

            var json = JsonConvert.SerializeObject(r);
            var byteJson = Encoding.ASCII.GetBytes(json);

            var req = new RestRequest("/reveal-chain/", Method.POST);
            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse resp = client.Execute(req);
            Console.WriteLine("RevealChain Resp = " + resp.StatusCode);
            return true;
        }
    }
}
