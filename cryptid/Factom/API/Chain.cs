#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Cryptid.Utils;
using Newtonsoft.Json;
using RestSharp;

#endregion

namespace Cryptid.Factom.API {
    public class Chain {
        private const string ServerHost = "localhost";
        private const int ServerPort = 8088;
        private const int ServerPortMd = 8089;

        private readonly RestClient _client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");
        private readonly RestClient _clientMd = new RestClient("http://" + ServerHost + ":" + ServerPortMd + "/v1/");

        /// <summary>
        ///     Returns a chaintype object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public ChainType NewChain(DataStructs.EntryData entry) {
            var c = new ChainType {FirstEntry = entry};
            var chainHash = new List<byte>();
            if (entry.ExtIDs != null) {
                foreach (var extId in entry.ExtIDs) {
                    var h = SHA256.Create().ComputeHash(extId);
                    chainHash.AddRange(h);
                }
            }
            c.ChainId = SHA256.Create().ComputeHash(chainHash.ToArray());
            c.FirstEntry.ChainId = c.ChainId;
            return c;
        }

        /// <summary>
        ///     First method to add a chain to factom.
        /// </summary>
        /// <param name="c">ChainType</param>
        /// <param name="name">Name of Entry  Credit wallet</param>
        /// <returns>ChainId</returns>
        public byte[] CommitChain(ChainType c, string name) {
            var byteList = new List<byte>();

            //1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(Times.MilliTime());

            var entry = c.FirstEntry;

            // 32 Byte ChainId Hash
            //byte[] chainIDHash = Encoding.ASCII.GetBytes(c.ChainId);
            var chainIdHash = c.ChainId;
            chainIdHash = SHA256.Create().ComputeHash(chainIdHash);
            chainIdHash = SHA256.Create().ComputeHash(chainIdHash);
            byteList.AddRange(chainIdHash);

            // 32 byte Weld; sha256(sha256(EntryHash + ChainId))
            var cid = c.ChainId;
            var s = Entries.HashEntry(c.FirstEntry);
            var weld = new byte[cid.Length + s.Length];
            s.CopyTo(weld, 0);
            cid.CopyTo(weld, s.Length);
            weld = SHA256.Create().ComputeHash(weld);
            weld = SHA256.Create().ComputeHash(weld);
            byteList.AddRange(weld);

            // 32 byte Entry Hash of the First Entry
            byteList.AddRange(Entries.HashEntry(c.FirstEntry));

            // 1 byte number of Entry Credits to pay
            var cost = (sbyte) (Entries.EntryCost(entry) + 10); // TODO: check errors
            byteList.Add(BitConverter.GetBytes(cost)[0]);

            var com = new WalletCommit {Message = Arrays.ByteArrayToHex(byteList.ToArray())};

            var json = JsonConvert.SerializeObject(com);

            var req = new RestRequest("/commit-chain/" + name, Method.POST);
            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            req.AddUrlSegment("name", name);
            var resp = _clientMd.Execute(req);

            Console.WriteLine("CommitChain Resp = " + resp.StatusCode); // TODO: Remove
            Console.WriteLine("Message= " + com.Message); // TODO: Remove

            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Chain Commit Failed. Message: " + resp.ErrorMessage);
            }
            return Entries.ChainIdOfFirstEntry(c.FirstEntry);
        }

        /// <summary>
        ///     Second step in committing a new chain. Only run this if CommitChain was successful.
        /// </summary>
        /// <param name="c">ChainType</param>
        /// <returns></returns>
        public bool RevealChain(ChainType c) {
            var r = new Reveal();
            var b = Entries.MarshalBinary(c.FirstEntry);
            r.Entry = Arrays.ByteArrayToHex(b);

            var json = JsonConvert.SerializeObject(r);

            var req = new RestRequest("/reveal-chain/", Method.POST);
            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            var resp = _client.Execute(req);
            Console.WriteLine("RevealChain Resp = " + resp.StatusCode); //TODO: Remove

            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Chain Reveal Failed. Message: " + resp.ErrorMessage);
            }
            return true;
        }

        public class ChainType {
            public byte[] ChainId { get; set; }
            public DataStructs.EntryData FirstEntry { get; set; }
        }

        private class WalletCommit {
            public string Message { get; set; }
        }

        private class Reveal {
            public string Entry { get; set; }
        }
    }
}