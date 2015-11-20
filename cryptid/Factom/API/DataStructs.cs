using Cryptid.Utils;

namespace cryptid.Factom.API {
    public class DataStructs {
        public static EntryBlockData ConvertStringFormatToByteFormat(EntryBlockDataStringFormat blockStringFormat) {
            var block = new EntryBlockData();
            block.Header = new EntryBlockData.HeaderData();

            var len = blockStringFormat.EntryList.Length;
            block.EntryList = new EntryBlockData.EntryData[len];

            for (var i = 0; i < len; i++) {
                block.EntryList[i] = new EntryBlockData.EntryData();
                block.EntryList[i].EntryHash = Strings.DecodeHexIntoBytes(blockStringFormat.EntryList[i].EntryHash);
                block.EntryList[i].Timestamp = blockStringFormat.EntryList[i].Timestamp;
            }

            block.Header.BlockSequenceNumber = blockStringFormat.Header.BlockSequenceNumber;
            block.Header.ChainID = Strings.DecodeHexIntoBytes(blockStringFormat.Header.ChainID);
            block.Header.PrevKeyMr = Strings.DecodeHexIntoBytes(blockStringFormat.Header.PrevKeyMr);
            block.Header.Timestamp = blockStringFormat.Header.Timestamp;

            return block;
        }

        public static EntryData ConvertStringFormatToByteFormat(EntryDataStringFormat entryStringFormat) {
            var entry = new EntryData();
            entry.ChainID = Strings.DecodeHexIntoBytes(entryStringFormat.ChainID);
            entry.Content = Strings.DecodeHexIntoBytes(entryStringFormat.Content);

            var len = entryStringFormat.ExtIDs.Length;
            entry.ExtIDs = new byte[len][];
            for (var i = 0; i < len; i++) {
                entry.ExtIDs[i] = Strings.DecodeHexIntoBytes(entryStringFormat.ExtIDs[i]);
            }
            return entry;
        }

        public static ChainHeadData ConvertStringFormatToByteFormat(ChainHeadDataStringFormat chainStringFormat) {
            var chain = new ChainHeadData();
            chain.ChainHead = Strings.DecodeHexIntoBytes(chainStringFormat.ChainHead);
            return chain;
        }

        public static EntryBlockDataStringFormat ConvertByteFormatToStringFormat(EntryBlockData block) {
            var blockStringFormat = new EntryBlockDataStringFormat();
            var len = block.EntryList.Length;
            blockStringFormat.EntryList = new EntryBlockDataStringFormat.EntryData[len];

            for (var i = 0; i < len; i++) {
                blockStringFormat.EntryList[i] = new EntryBlockDataStringFormat.EntryData();
                blockStringFormat.EntryList[i].EntryHash = Arrays.ByteArrayToHex(block.EntryList[i].EntryHash);
                blockStringFormat.EntryList[i].Timestamp = block.EntryList[i].Timestamp;
            }

            blockStringFormat.Header.BlockSequenceNumber = block.Header.BlockSequenceNumber;
            blockStringFormat.Header.ChainID = Arrays.ByteArrayToHex(block.Header.ChainID);
            blockStringFormat.Header.PrevKeyMr = Arrays.ByteArrayToHex(block.Header.PrevKeyMr);
            blockStringFormat.Header.Timestamp = block.Header.Timestamp;

            return blockStringFormat;
        }

        public class EntryData {
            // Hex Bytes
            public byte[] ChainID { get; set; }
            // UTF8 Encoding
            public byte[] Content { get; set; }
            public byte[][] ExtIDs { get; set; }
        }


        public class EntryBlockData {
            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }

            public class HeaderData {
                public int BlockSequenceNumber { get; set; }
                // Hex Bytes
                public byte[] ChainID { get; set; }
                // Hex Bytes
                public byte[] PrevKeyMr { get; set; }
                public int Timestamp { get; set; }
            }

            public class EntryData {
                // Hex Bytes
                public byte[] EntryHash { get; set; }
                public int Timestamp { get; set; }
            }
        }

        public class ChainHeadData {
            // KeyMR of first Entry in Entry chain
            // Hex Bytes
            public byte[] ChainHead { get; set; }
        }

        //                             \\
        //  Conversion and Json Tools  \\
        //                             \\

        public class EntryDataStringFormat {
            public string ChainID { get; set; }
            public string Content { get; set; }
            public string[] ExtIDs { get; set; }
        }

        public class EntryBlockDataStringFormat {
            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }

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
        }

        public class ChainHeadDataStringFormat {
            // KeyMR of first Entry in Entry chain
            // Hex Bytes
            public string ChainHead { get; set; }
        }
    }
}