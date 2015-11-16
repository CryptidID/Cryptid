using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using cryptid.Factom.API;
using Newtonsoft.Json;
using RestSharp;

namespace Cryptid.Factom.API
{
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    public class FactomApi {
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

        public class WallerCommit {
            string Message { get; set; }
        }
        public bool CommitEntry(EntryData entry, string name) {
            List<byte> byteList = new List<byte>();
            byteList.Add(0);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return true; // TODO: This, true for success, false=failed
        }

        private byte[] milliTime() {
            List<byte> byteList = new List<byte>();
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixMilliLong = (long) (DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
            byte[] unixBytes = CheckEndian(BitConverter.GetBytes(unixMilliLong));
            
        }

        private class Reveal {
            public string Entry { get; set; }
        }
        public bool RevealEntry(EntryData entry) {
            Reveal rev = new Reveal();
            byte[] marshaledEntry = MarshalBinary(entry);
            rev.Entry = ByteArrayToString(marshaledEntry);
            var req = new RestRequest("/reveal-chain/", Method.POST);
            req.AddParameter("application/json", rev, ParameterType.RequestBody);
            IRestResponse resp = client.Execute(req);
            return true;//TODO: This, true for success, false=failed
        }

        private static string ByteArrayToString(byte[] ba) {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public byte[] MarshalBinary(EntryData e) { //TODO: Make private
            List<byte> entryBStruct = new List<byte>();
            byte[] idsSize = MarshalExtIDsSize(e);

            
            idsSize = CheckEndian(idsSize);
            // Header 
                // 1 byte version
                byte version = 0;
                entryBStruct.Add(version);
                // 32 byte chainid
                byte[] chain = DecodeHexIntoBytes(e.ChainID);
                entryBStruct.AddRange(chain);
                // Ext Ids Size
                entryBStruct.AddRange(idsSize);

            // Payload
            // ExtIDS
                if (e.ExtIDs != null) {
                    byte[] ids = MarshalExtIDsBinary(e);
                    entryBStruct.AddRange(ids);
                }
            // Content
                byte[] content = Encoding.ASCII.GetBytes(e.Content);
                entryBStruct.AddRange(content);

            return entryBStruct.ToArray();
        }

        private byte[] MarshalExtIDsBinary(EntryData e) {
            List<byte> byteList = new List<byte>();
            foreach (var exID in e.ExtIDs) {
                // 2 byte size of ExtID
                Int16 extLen = Convert.ToInt16(exID.Length);
                byte[] bytes = BitConverter.GetBytes(extLen);
                bytes = CheckEndian(bytes);
                byteList.AddRange(bytes);
                byte[] extIDStr = Encoding.ASCII.GetBytes(exID);
                byteList.AddRange(extIDStr);
            }
            return byteList.ToArray();
        }

        private byte[] MarshalExtIDsSize(EntryData e) {
            if (e.ExtIDs == null) {
                Int16 extLen = 0;
                byte[] bytes = BitConverter.GetBytes(extLen);
                return CheckEndian(bytes);
            } else {
                var totalSize = 0;
                foreach (var extElement in e.ExtIDs) {
                    totalSize += extElement.Length + 2;
                }

                Int16 extLen = Convert.ToInt16(totalSize);


                byte[] bytes = BitConverter.GetBytes(extLen);
                return bytes;
                // return CheckEndian(bytes);
            }
        }

        /// <summary>
        /// Will correct a little endian byte[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private byte[] CheckEndian(byte[] bytes) {
            if (BitConverter.IsLittleEndian) {
                var byteList = bytes.Reverse(); // Must be in bigendian
                return byteList.ToArray();
            } else {
                return bytes;
            }
        }

        public byte[] DecodeHexIntoBytes(string input) {
            var result = new byte[(input.Length + 1) >> 1];
            int lastcell = result.Length - 1;
            int lastchar = input.Length - 1;
            // count up in characters, but inside the loop will
            // reference from the end of the input/output.
            for (int i = 0; i < input.Length; i++) {
                // i >> 1    -  (i / 2) gives the result byte offset from the end
                // i & 1     -  1 if it is high-nibble, 0 for low-nibble.
                result[lastcell - (i >> 1)] |= ByteLookup[i & 1, HexToInt(input[lastchar - i])];
            }
            return result;
        }


        private static int HexToInt(char c) {
            switch (c) {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    throw new FormatException("Unrecognized hex char " + c);
            }
        }
        private static readonly byte[,] ByteLookup = new byte[,]
        {
            // low nibble
            {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f},
            // high nibble
            {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0}
        };

    }
}