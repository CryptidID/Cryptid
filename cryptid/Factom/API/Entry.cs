using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using cryptid.Factom.API;
using Cryptid.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace Cryptid.Factom.API
{
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    public class Entry {
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
        public EntryBlockData GetEntryBlockByKeyMR(ChainHeadData chainHead) {
            return GetEntryBlockByKeyMR(chainHead.ChainHead);
        }

        /// <summary>
        /// Returns an EntryBlockData
        /// </summary>
        /// <param name="hash">String of KeyMr</param>
        /// <returns>EntryBlockData</returns>
        public EntryBlockData GetEntryBlockByKeyMR(string keyMR) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.GET);
            req.AddUrlSegment("hash", keyMR);

            IRestResponse resp = client.Execute(req);
            EntryBlockData entryBlock = JsonConvert.DeserializeObject<EntryBlockData>(resp.Content);
            return entryBlock;
        }

        /// <summary>
        /// Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a string
        /// </summary>
        /// <param name="hash">Entry hash as EntryBlockData.entry</param>
        /// <returns></returns>
        public EntryData GetEntryData(EntryBlockData.EntryData entry) {
            return GetEntryData(entry.EntryHash);
        }

        /// <summary>
        /// Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a string
        /// </summary>
        /// <param name="entryHash">Entryhash of entry</param>
        /// <returns></returns>
        public EntryData GetEntryData(string entryHash) {
            var req = new RestRequest("/entry-by-hash/{hash}", Method.GET);
            req.AddUrlSegment("hash", entryHash);

            IRestResponse resp = client.Execute(req);
            EntryData entryType = JsonConvert.DeserializeObject<EntryData>(resp.Content);
            ;
            return entryType;
        }

        /// <summary>
        /// Returns all the entries in an Entryblock. Type of entry has timestamp and entryhash value
        /// </summary>
        /// <param name="chainHead">ChainHeadData type</param>
        /// <returns></returns>
        public List<EntryBlockData.EntryData> GetAllChainEntries(ChainHeadData chainHead) {
            EntryBlockData block = GetEntryBlockByKeyMR(chainHead);
            EntryBlockData blockPointer = block;
            List<EntryBlockData.EntryData> dataList = new List<EntryBlockData.EntryData>();

            while (blockPointer.Header.PrevKeyMr != ZeroHash) {
                dataList.AddRange(blockPointer.EntryList); // Add all entries in current MR
                blockPointer = GetEntryBlockByKeyMR(blockPointer.Header.PrevKeyMr);
            }
            dataList.AddRange(blockPointer.EntryList);
            return dataList;
        }

        /// <summary>
        /// Returns all the entries in an Entryblock. Type of entry has timestamp and entryhash value
        /// </summary>
        /// <param name="chainHeadID">ChainID as string</param>
        /// <returns></returns>
        public List<EntryBlockData.EntryData> GetAllChainEntries(string chainHeadID) {
            ChainHeadData chainHead = GetChainHead(chainHeadID);
            return GetAllChainEntries(chainHead);
        }

        private class WallerCommit {
            public string Message { get; set; }
        }

        public bool CommitEntry(EntryData entry, string name) {
            List<byte> byteList = new List<byte>();

            // 1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(Times.MilliTime());

            // 32 byte Entry Hash
            byteList.AddRange(Entries.HashEntry(entry));

            // 1 byte number of entry credits to pay
            sbyte cost = Entries.EntryCost(entry); // TODO: check errors
            byteList.Add(BitConverter.GetBytes(cost)[0]);

            WallerCommit com = new WallerCommit();
            com.Message = Arrays.ByteArrayToHex(byteList.ToArray()); //Hex encoded string on bytelist

           var json = JsonConvert.SerializeObject(com);

           Console.WriteLine("CE Json = " + json);
           var byteJson = Encoding.ASCII.GetBytes(json);
           Console.WriteLine("Byte to json = " + Encoding.ASCII.GetString(byteJson));

           var req = new RestRequest("/commit-entry/{name}", Method.POST);
           req.AddUrlSegment("name", name);
           req.AddJsonBody(json);
           
           //req.AddParameter("Application/Json", json, ParameterType.RequestBody);
           //req.AddBody("application/json", json);
           IRestResponse resp = client.Execute(req);
           Console.WriteLine("CommitEntry Resp = " + resp.Content);
           return true; // TODO: This, true for success, false=failed
        }

        private class Reveal {
            public string Entry { get; set; }
        }

        public bool RevealEntry(EntryData entry) {
            Reveal rev = new Reveal();
            byte[] marshaledEntry = Entries.MarshalBinary(entry);
            rev.Entry = Arrays.ByteArrayToHex(marshaledEntry);
            var req = new RestRequest("/reveal-entry/", Method.POST);
            var json = JsonConvert.SerializeObject(rev);
            Console.WriteLine("RE Json = " + json);
            var byteJson = Encoding.ASCII.GetBytes(json);
            req.AddParameter("application/json", byteJson, ParameterType.RequestBody);
            IRestResponse resp = client.Execute(req);
            Console.WriteLine("RevealEntry Resp = " + resp.Content);
            return true;//TODO: This, true for success, false=failed
        }
    }
}