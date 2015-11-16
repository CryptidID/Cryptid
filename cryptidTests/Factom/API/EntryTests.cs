using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cryptid.Factom.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using cryptid.Factom.API;
using Cryptid.Utils;

namespace Cryptid.Factom.API.Tests {
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    [TestClass()]
    public class EntryTests {
        [TestMethod()]
        public void DecodeHexIntoBytesTest() {
            var api = new Entry();
            var inputs = new[] { "",
                    "0",
                     "10",
                     "f",
                     "0f",
                     "010",
                     "0ff",
                     "f2ab"
                 };
            var i = 0;
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0 }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0x10 }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0xf }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0xf }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0x0, 0x10 }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0x0, 0xff }));
            Debug.Assert(Strings.DecodeHexIntoBytes(inputs[i++]).SequenceEqual(new byte[] { 0xf2, 0xab }));
            Debug.Assert(i == inputs.Length);
        }

        [TestMethod()]
        public void GetChainTest() {
            //Chain api = new Chain();

            // Chain ID to test, 8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // http://explorer.factom.org/chain/8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // Has 2 entries
            // Chain ID to test, 475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // http://explorer.factom.org/chain/475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // Has 2 entries

            Entry api = new Entry();
            ChainHeadData chainHead = api.GetChainHead("475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec");
            EntryBlockData entryBlock = api.GetEntryBlockByKeyMR(chainHead);
            EntryData entry = api.GetEntryData(entryBlock.EntryList[0]);
            var entries = api.GetAllChainEntries(chainHead);
            foreach (var entryElement in entries) {
                Console.WriteLine("Hash" + entryElement.EntryHash + "\n Data:" + api.GetEntryData(entryElement));
            }
        }

        [TestMethod()]
        public void MarshalBinaryTest() {
            Entry api = new Entry();
            EntryData e = new EntryData();
            e.Content = "Each directory listed in the Go path must have a prescribed structure:";
            e.ChainID = "00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77";
            Byte[] b = Entries.MarshalBinary(e);
            //Console.WriteLine("Entry");
            string s = "[";
            //Console.Write("[");
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x.ToString() + " ";
            }
            //Console.Write("]");
            s = s.TrimEnd();
            s += "]";
            var expected =
                "[0 0 81 28 41 134 104 188 80 50 166 75 118 248 237 230 241 25 173 209 166 68 130 200 96 41 102 21 44 11 147 108 119 0 0 69 97 99 104 32 100 105 114 101 99 116 111 114 121 32 108 105 115 116 101 100 32 105 110 32 116 104 101 32 71 111 32 112 97 116 104 32 109 117 115 116 32 104 97 118 101 32 97 32 112 114 101 115 99 114 105 98 101 100 32 115 116 114 117 99 116 117 114 101 58]";
            Assert.AreEqual(s, expected);
            var arr = new string[2] { "a136bf2a5b81a671d3f0c168f4", "b35f223db2dced312581d22c46ba4117702d03" };
            e.ExtIDs = arr;
            b = Entries.MarshalBinary(e);
            s = "[";
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x.ToString() + " ";
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

        [TestMethod()]
        public void MilliTimeTest() {
//            var api = new Entry();
//            byte[] mt = api.MilliTime();
//            string s = "[";
//            foreach (var x in mt) {
//                //Console.Write(x.ToString() + " ");
//                s += x.ToString() + " ";
//            }
//            s = s.TrimEnd();
//            s += "]";
//            Console.WriteLine(s);
        }

        [TestMethod()]
        public void HashEntryTest() {
            var api = new Entry();
            EntryData e = new EntryData();
            e.Content = "Each directory listed in the Go path must have a prescribed structure:";
            e.ChainID = "00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77";
            var arr = new string[2] { "a136bf2a5b81a671d3f0c168f4", "b35f223db2dced312581d22c46ba4117702d03" };
            e.ExtIDs = arr;
            var b = Entries.HashEntry(e);
            string s = "[";
            foreach (var x in b) {
                //Console.Write(x.ToString() + " ");
                s += x.ToString() + " ";
            }
            s = s.TrimEnd();
            s += "]";
            var expected =
                "[47 94 26 227 140 169 172 169 205 164 41 133 92 58 162 99 75 210 111 141 156 171 1 239 116 21 221 12 224 231 240 42]";
            Assert.AreEqual(s, expected);
        }

        [TestMethod()]
        public void EntryCostTest() {
            var api = new Entry();
            EntryData e = new EntryData();
            e.Content = "Each directory listed in the Go path must have a prescribed structure:";
            e.ChainID = "00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77";
            var arr = new string[2] { "a136bf2a5b81a671d3f0c168f4", "b35f223db2dced312581d22c46ba4117702d03" };
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
            Assert.AreEqual(b, (sbyte)1);

            e.Content = "Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:";
            b = Entries.EntryCost(e);
            Assert.AreEqual(b, (sbyte)4);
        }

        [TestMethod()]
        public void CommitEntryTest() {
            var api = new Entry();
            EntryData e = new EntryData();
            e.Content = "Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:Each directory listed in the Go path must have a prescribed structure:";
            e.ChainID = "00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77";
            var arr = new string[2] { "a136bf2a5b81a671d3f0c168f4", "b35f223db2dced312581d22c46ba4117702d03" };
            e.ExtIDs = arr;

            var s = api.CommitEntry(e, "name");
            Console.WriteLine(s);
            var actual = "00015111de46c16c05277cb99f5cd3713d957da3162f34f5de34d1c41340a7caa46bb2d233c24f04";
            Assert.AreEqual(s,actual);
        }

        [TestMethod()]
        public void NewChainTest() {
            var api = new Chain();
            DataStructs.EntryData e = new DataStructs.EntryData();
            e.Content = "Each directory listed in the Go path must have a prescribed structure:";
            e.ChainID = "00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77";
            var arr = new string[2] { "a136bf2a5b81a671d3f0c168f4", "b35f223db2dced312581d22c46ba4117702d03" };
            e.ExtIDs = arr;

            var x = api.NewChain(e);
            Console.WriteLine(x.ChainId);
            Assert.AreEqual("aaf2c9dabc8711bf12d8be6c236717dc6f7b738d6bf7a7cab079f5c76f6eebee", x.ChainId);

            api.Commitchain(x, "name");
        }

        [TestMethod()]
        public void MakeAChainTest() {
            EntryData entry = new EntryData();
            entry.Content = "I really hope this chain was committed properly. Tesing testinssssg!";
            Entry entryApi = new Entry();
            Chain chainApi = new Chain();
            var chain = chainApi.NewChain(entry);
            chainApi.Commitchain(chain, "CryptidHasenteredTheGame");
            System.Threading.Thread.Sleep(11000);
            chainApi.RevealChain(chain);

            Console.WriteLine("ChainID=" + chain.ChainId);
            Console.WriteLine("ChainID=" + chain.FirstEntry.ChainID);
        }

        [TestMethod()]
        public void MakeAEntryTest() {
            EntryData entry = new EntryData();
            entry.Content = "I really hope this chain was committed properly. Tesing testinssssg!";
            entry.ChainID = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            Entry entryApi = new Entry();
            Chain chainApi = new Chain();
            entryApi.CommitEntry(entry, "");
            System.Threading.Thread.Sleep(11000);
            entryApi.RevealEntry(entry);

            Console.WriteLine("ChainID=" + entry.ChainID);
            
        }
    }
}