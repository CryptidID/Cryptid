using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cryptid.Factom.API;
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

        private RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");
        private RestClient clientMD = new RestClient("http://" + ServerHost + ":" + ServerPortMD + "/v1/");

        public class ChainType {
            public byte[] ChainId { get; set; }
            public EntryData FirstEntry { get; set; }
        }

        /// <summary>
        /// Returns a chaintype object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public ChainType NewChain(EntryData entry) {
            ChainType c = new ChainType();
            c.FirstEntry = entry;
            List<byte> chainHash = new List<byte>();
            if (entry.ExtIDs != null) {
                foreach (byte[] extId in entry.ExtIDs) {
                    var h = SHA256.Create().ComputeHash(extId);
                    chainHash.AddRange(h);
                }
            }
            c.ChainId = SHA256.Create().ComputeHash(chainHash.ToArray());
            c.FirstEntry.ChainId = c.ChainId;
            return c;
        }

        /// <summary>
        /// Used to send json object as POST data
        /// </summary>
        private class WalletCommit  {
            public string Message { get; set; }
        }

        /// <summary>
        /// First method to add a chain to factom.
        /// </summary>
        /// <param name="c">Chain to be added</param>
        /// <param name="name">Name of Entry  Credit wallet</param>
        /// <returns>ChainID of chain added</returns>
        public byte[] CommitChain(ChainType c, string name) {
            List<byte> byteList = new List<byte>();

            //1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(Times.MilliTime());

            var entry = c.FirstEntry;

            // 32 Byte ChainID Hash
            //byte[] chainIDHash = Encoding.ASCII.GetBytes(c.ChainId);
            byte[] chainIDHash = c.ChainId;
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            byteList.AddRange(chainIDHash);

            // 32 byte Weld; sha256(sha256(EntryHash + ChainID))
            byte[] cid = c.ChainId;
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

            Console.WriteLine("CommitChain Resp = " + resp.StatusCode);// TODO: Remove
            Console.WriteLine("Message= " + com.Message); // TODO: Remove

            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Chain Commit Failed. Message: " + resp.ErrorMessage);
            }
            return Entries.ChainIdOfFirstEntry(c.FirstEntry);
        }

        /// <summary>
        /// Used to serialize json as POST data
        /// </summary>
        private class Reveal {
            public string Entry { get; set; }
        }

        /// <summary>
        /// Second step in committing a new chain. Only run this if CommitChain was successful.
        /// </summary>
        /// <param name="c">Chain to be added</param>
        /// <returns>Boolean true/false for success/failure</returns>
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
            Console.WriteLine("RevealChain Resp = " + resp.StatusCode); //TODO: Remove

            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Chain Reveal Failed. Message: " + resp.ErrorMessage);
            }
            return true;
        }
    }
}
