using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cryptid.Factom.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Factom.API.Tests {
    [TestClass()]
    public class ChainTests {
        [TestMethod()]
        public void getChainTest() {
            Chain api = new Chain();
            string key = api.GetChainHead("df3ade9eec4b08d5379cc64270c30ea7315d8a8a1a69efe2b98a60ecdd69e604");
            Console.WriteLine("KeyMR: " + key);
            string entryBlockStruct = api.GetEntryBlockByKeyMR(key);
            Console.WriteLine("EntryBlock: " + entryBlockStruct);

        }
    }
}