using System;
using System.Collections.Generic;
using System.Net;
using cryptid.Factom.API;
using Cryptid.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace Cryptid.Factom.API {
    public class Entry {
        private const string ServerHost = "localhost";
        private const int ServerPort = 8088;
        private const int ServerPortMD = 8089;

        private static readonly byte[] ZeroHash =
            Strings.DecodeHexIntoBytes("0000000000000000000000000000000000000000000000000000000000000000");


        private readonly RestClient client = new RestClient("http://" + ServerHost + ":" + ServerPort + "/v1/");
        private readonly RestClient clientMD = new RestClient("http://" + ServerHost + ":" + ServerPortMD + "/v1/");

        public DataStructs.EntryData NewEntry(byte[] content, byte[][] extIds, string chainId) {
            var entry = new DataStructs.EntryData();
            entry.Content = content;
            entry.ExtIDs = extIds;
            return entry;
        }

        /// <summary>
        ///     Takes in an entry chain hash and returns Key MR of the first entry.
        /// </summary>
        /// <param name="hash">ChainID</param>
        /// <returns>KeyMR of first entry (last in list)</returns>
        public DataStructs.ChainHeadData GetChainHead(byte[] hash) {
            var hashString = Arrays.ByteArrayToHex(hash);
            var req = new RestRequest("/chain-head/{hash}", Method.GET);
            req.AddUrlSegment("hash", hashString);
            var resp = client.Execute(req);

            var chainHead = JsonConvert.DeserializeObject<DataStructs.ChainHeadDataStringFormat>(resp.Content);
            return DataStructs.ConvertStringFormatToByteFormat(chainHead);
        }

        /// <summary>
        ///     Returns an EntryBlockData
        /// </summary>
        /// <param name="hash">Chainhead</param>
        /// <returns>EntryBlockData</returns>
        public DataStructs.EntryBlockData GetEntryBlockByKeyMR(DataStructs.ChainHeadData chainHead) {
            return GetEntryBlockByKeyMR(chainHead.ChainHead);
        }


        /// <summary>
        ///     Returns an EntryBlockData
        /// </summary>
        /// <param name="hash">String of KeyMr</param>
        /// <returns>EntryBlockData</returns>
        public DataStructs.EntryBlockData GetEntryBlockByKeyMR(byte[] keyMR) {
            var req = new RestRequest("/entry-block-by-keymr/{hash}", Method.GET);
            var keyMRString = Arrays.ByteArrayToHex(keyMR);
            req.AddUrlSegment("hash", keyMRString);

            var resp = client.Execute(req);
            if (resp.Content == "EBlock not found") {
                throw new Exception("EBlock not Found, Zerohash looked up");
            }
            var entryBlock = JsonConvert.DeserializeObject<DataStructs.EntryBlockDataStringFormat>(resp.Content);

            return DataStructs.ConvertStringFormatToByteFormat(entryBlock);
        }

        /// <summary>
        ///     Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a
        ///     string
        /// </summary>
        /// <param name="hash">Entry hash as EntryBlockData.entry</param>
        /// <returns></returns>
        public DataStructs.EntryData GetEntryData(DataStructs.EntryBlockData.EntryData entry) {
            return GetEntryData(entry.EntryHash);
        }

        /// <summary>
        ///     Returns the data in an entry hash in a easier to use format. ToString will convert the Content from hex into a
        ///     string
        /// </summary>
        /// <param name="entryHash">Entryhash of entry</param>
        /// <returns></returns>
        public DataStructs.EntryData GetEntryData(byte[] entryHash) {
            var req = new RestRequest("/entry-by-hash/{hash}", Method.GET);
            req.AddUrlSegment("hash", Arrays.ByteArrayToHex(entryHash));

            var resp = client.Execute(req);
            var entryType = JsonConvert.DeserializeObject<DataStructs.EntryDataStringFormat>(resp.Content);
            return DataStructs.ConvertStringFormatToByteFormat(entryType);
        }

        /// <summary>
        ///     Returns all the entries in an Entryblock. Type of entry has timestamp and entryhash value
        /// </summary>
        /// <param name="chainHead">ChainHeadData type</param>
        /// <returns></returns>
        public List<DataStructs.EntryBlockData.EntryData> GetAllChainEntries(DataStructs.ChainHeadData chainHead) {
            var block = GetEntryBlockByKeyMR(chainHead);
            var blockPointer = block;
            var dataList = new List<DataStructs.EntryBlockData.EntryData>();

            while (!Bytes.Equality(blockPointer.Header.PrevKeyMr, ZeroHash)) {
                dataList.AddRange(blockPointer.EntryList); // Add all entries in current MR
                blockPointer = GetEntryBlockByKeyMR(blockPointer.Header.PrevKeyMr);
            }
            dataList.AddRange(blockPointer.EntryList);
            return dataList;
        }

        /// <summary>
        ///     Returns all the entries in an Entryblock. Type of entry has timestamp and entryhash value
        /// </summary>
        /// <param name="chainHeadID">ChainID as string</param>
        /// <returns></returns>
        public List<DataStructs.EntryBlockData.EntryData> GetAllChainEntries(byte[] chainHeadID) {
            var chainHead = GetChainHead(chainHeadID);
            return GetAllChainEntries(chainHead);
        }

        public byte[] CommitEntry(DataStructs.EntryData entry, string name) {
            var byteList = new List<byte>();

            // 1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(Times.MilliTime());

            // 32 byte Entry Hash
            byteList.AddRange(Entries.HashEntry(entry));

            // 1 byte number of entry credits to pay
            var cost = Entries.EntryCost(entry); // TODO: check errors
            byteList.Add(BitConverter.GetBytes(cost)[0]);

            var com = new WallerCommit();
            com.Message = Arrays.ByteArrayToHex(byteList.ToArray()); //Hex encoded string on bytelist

            var json = JsonConvert.SerializeObject(com);

            Console.WriteLine("CE Json = " + json); //TODO: Remove

            var req = new RestRequest("/commit-entry/{name}", Method.POST);
            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            req.AddUrlSegment("name", name);
            var resp = clientMD.Execute(req);
            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Entry Commit Failed. Message: " + resp.ErrorMessage);
            }
            Console.WriteLine("CommitEntry Resp = " + resp.StatusCode + "|" + resp.StatusCode);
            return Entries.ChainIdOfFirstEntry(entry);
        }

        public bool RevealEntry(DataStructs.EntryData entry) {
            var rev = new Reveal();
            var marshaledEntry = Entries.MarshalBinary(entry);
            rev.Entry = Arrays.ByteArrayToHex(marshaledEntry);
            var req = new RestRequest("/reveal-entry/", Method.POST);
            var json = JsonConvert.SerializeObject(rev);
            Console.WriteLine("RE Json = " + json);

            req.RequestFormat = DataFormat.Json;
            req.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse resp = client.Execute<RestRequest>(req);
            Console.WriteLine("RevealEntry Resp = " + resp.StatusCode + "|" + resp.StatusCode);

            if (resp.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Entry Reveal Failed. Message: " + resp.ErrorMessage);
            }
            return true;
        }

        private class WallerCommit {
            public string Message { get; set; }
        }

        private class Reveal {
            public string Entry { get; set; }
        }
    }
}