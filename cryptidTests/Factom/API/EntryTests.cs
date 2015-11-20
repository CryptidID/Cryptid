#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using cryptid.Factom.API;
using Cryptid.Factom.API;
using Cryptid.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace CryptidTests.Factom.API {
    [TestClass]
    public class EntryTests {
        [TestMethod]
        public void DecodeHexIntoBytesTest() {
            var inputs = new[] {
                "",
                "0",
                "10",
                "f",
                "0f",
                "010",
                "0ff",
                "f2ab"
            };
            var i = 0;
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0x10}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0xf}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0xf}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0x0, 0x10}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0x0, 0xff}));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] {0xf2, 0xab}));
            Debug.Assert(i == inputs.Length);
        }

        [TestMethod]
        public void GetChainTest() {
            //Chain api = new Chain();

            // Chain ID to test, 8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // http://explorer.factom.org/chain/8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // Has 2 entries
            // Chain ID to test, 475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // http://explorer.factom.org/chain/475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // Has 2 entries

            var api = new Entry();
            var val = Strings.DecodeHexIntoBytes("475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec");

            var chainHead = api.GetChainHead(val);
            var entries = api.GetAllChainEntries(chainHead);
            foreach (var entryElement in entries) {
                Console.WriteLine("Hash" + entryElement.EntryHash + "\n Data:" + api.GetEntryData(entryElement));
            }
        }

        [TestMethod]
        public void MarshalBinaryTest() {
            var e = new DataStructs.EntryData {
                Content =
                    Encoding.UTF8.GetBytes("Each directory listed in the Go path must have a prescribed structure:"),
                ChainId = Strings.DecodeHexIntoBytes("00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77")
            };
            var b = Entries.MarshalBinary(e);
            //Console.WriteLine("Entry");
            var s = "[";
            //Console.Write("[");
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x + " ";
            }
            //Console.Write("]");
            s = s.TrimEnd();
            s += "]";
            var expected =
                "[0 0 81 28 41 134 104 188 80 50 166 75 118 248 237 230 241 25 173 209 166 68 130 200 96 41 102 21 44 11 147 108 119 0 0 69 97 99 104 32 100 105 114 101 99 116 111 114 121 32 108 105 115 116 101 100 32 105 110 32 116 104 101 32 71 111 32 112 97 116 104 32 109 117 115 116 32 104 97 118 101 32 97 32 112 114 101 115 99 114 105 98 101 100 32 115 116 114 117 99 116 117 114 101 58]";
            Assert.AreEqual(s, expected);
            //var arr = new byte[2][] { Strings.DecodeHexIntoBytes("a136bf2a5b81a671d3f0c168f4"), Strings.DecodeHexIntoBytes("b35f223db2dced312581d22c46ba4117702d03") };
            var arr = new[] {
                Encoding.UTF8.GetBytes("a136bf2a5b81a671d3f0c168f4"),
                Encoding.UTF8.GetBytes("b35f223db2dced312581d22c46ba4117702d03")
            };
            e.ExtIDs = arr;
            b = Entries.MarshalBinary(e);
            s = "[";
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x + " ";
            }
            //Console.Write("]");
            s = s.TrimEnd();
            s += "]";
            expected =
                "[0 0 81 28 41 134 104 188 80 50 166 75 118 248 237 230 241 25 173 209 166 68 130 200 96 41 102 21 44 11 147 108 119 0 68 0 26 97 49 51 54 98 102 50 97 53 98 56 49 97 54 55 49 100 51 102 48 99 49 54 56 102 52 0 38 98 51 53 102 50 50 51 100 98 50 100 99 101 100 51 49 50 53 56 49 100 50 50 99 52 54 98 97 52 49 49 55 55 48 50 100 48 51 69 97 99 104 32 100 105 114 101 99 116 111 114 121 32 108 105 115 116 101 100 32 105 110 32 116 104 101 32 71 111 32 112 97 116 104 32 109 117 115 116 32 104 97 118 101 32 97 32 112 114 101 115 99 114 105 98 101 100 32 115 116 114 117 99 116 117 114 101 58]";

            Console.WriteLine(b.Length);
            Assert.AreEqual(s, expected);
            //Console.WriteLine(s);
            //Console.WriteLine(b.Length);
        }

        [TestMethod]
        public void HashEntryTest() {
            var e = new DataStructs.EntryData {
                Content =
                    Encoding.UTF8.GetBytes("Each directory listed in the Go path must have a prescribed structure:"),
                ChainId = Strings.DecodeHexIntoBytes("00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77")
            };
            var arr = new byte[2][] {
                Encoding.UTF8.GetBytes("a136bf2a5b81a671d3f0c168f4"),
                Encoding.UTF8.GetBytes("b35f223db2dced312581d22c46ba4117702d03")
            };
            e.ExtIDs = arr;
            var b = Entries.HashEntry(e);
            var s = "[";
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x + " ";
            }
            s = s.TrimEnd();
            s += "]";
            var expected =
                "[47 94 26 227 140 169 172 169 205 164 41 133 92 58 162 99 75 210 111 141 156 171 1 239 116 21 221 12 224 231 240 42]";
            Assert.AreEqual(s, expected);
        }

        [TestMethod]
        public void EntryCostTest() {
            var e = new DataStructs.EntryData {
                Content =
                    Encoding.UTF8.GetBytes("Each directory listed in the Go path must have a prescribed structure:"),
                ChainId = Strings.DecodeHexIntoBytes("00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77")
            };
            var arr = new byte[2][] {
                Strings.DecodeHexIntoBytes("a136bf2a5b81a671d3f0c168f4"),
                Strings.DecodeHexIntoBytes("b35f223db2dced312581d22c46ba4117702d03")
            };
            e.ExtIDs = arr;
            var b = Entries.EntryCost(e);
//            string s = "[";
//            foreach (var x in b) {
//                //Console.Write(x.ToString() + " ");
//                s += x.ToString() + " ";
//            }
//            s = s.TrimEnd();
//            s += "]";
//            var expected =
//                "[47 94 26 227 140 169 172 169 205 164 41 133 92 58 162 99 75 210 111 141 156 171 1 239 116 21 221 12 224 231 240 42]";
            Assert.AreEqual(b, (sbyte) 1);

            e.Content =
                Encoding.UTF8.GetBytes(
                    "Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:");
            b = Entries.EntryCost(e);
            Assert.AreEqual(b, (sbyte) 4);
        }

        [TestMethod]
        public void MakeAChainTest() {
            // Costs money to test
//            EntryData entry = new EntryData();
//            entry.Content = "I really hope this chain was committed properly. Tesing testinssssg!";
//            entry.ExtIDs = new string[] { "Dakota", "Steven", "Robert" };
//            var d = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Dakota"));
//            var s = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Steven"));
//            var r = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Robert"));
//            byte[] b = new byte[d.Length + r.Length + s.Length];
//            d.CopyTo(b, 0); s.CopyTo(b, d.Length); r.CopyTo(b, s.Length + d.Length); 
//            var chaininfo = SHA256.Create().ComputeHash(b);
//            Console.WriteLine("\nCHAINID=  " + Strings.RemoveDashes(BitConverter.ToString(chaininfo)) + "\n");
//            Entry entryApi = new Entry();
//            Chain chainApi = new Chain();
//            var chain = chainApi.NewChain(entry);
//            chainApi.CommitChain(chain, "EC2C5BNieAgMbUAuBCpKdrquS8jeWNFyX1EdJoFPqjocB33wAsbf");
//            System.Threading.Thread.Sleep(11000);
//            chainApi.RevealChain(chain);
//
//            Console.WriteLine("ChainId=" + chain.ChainId);
//            Console.WriteLine("ChainId=" + chain.FirstEntry.ChainId);
        }

        [TestMethod]
        public void MakeAEntryTest() {
            // Costs money to test
//            EntryData entry = new EntryData();
//            entry.Content = "Cryptid Entry!";
//            entry.ChainId = "5319D50D6D1DFCF4AEBB00E8DA812AC1D256C0B3E765B0240B51A708701F1985";
//            Entry entryApi = new Entry();
//            Chain chainApi = new Chain();
//            entryApi.CommitEntry(entry, "CCNEntryCreds");
//            System.Threading.Thread.Sleep(11000);
//            entryApi.RevealEntry(entry);
//
//            Console.WriteLine("ChainId=" + entry.ChainId);
        }
    }
}