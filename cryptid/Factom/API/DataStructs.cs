using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cryptid.Factom.API {
    public class DataStructs {
        public class EntryData {
            public string ChainID { get; set; }
            // Data in Hex
            public string Content { get; set; }
            public string[] ExtIDs { get; set; }

            // Decodes the content from hex
            public override string ToString() {
                var HexValue = Content;
                var StrValue = "";
                while (HexValue.Length > 0) {
                    StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                    HexValue = HexValue.Substring(2, HexValue.Length - 2);
                }
                return StrValue;
            }
        }

        public class EntryBlockData {
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

            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }
        }

        public class ChainHeadData {
            // KeyMR of first Entry in Entry chain
            public string ChainHead { get; set; }
        }
    }
}
