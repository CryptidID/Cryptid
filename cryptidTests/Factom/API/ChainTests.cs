using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cryptid.Factom.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cryptid.Factom.API.Tests {
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    [TestClass()]
    public class ChainTests {
        [TestMethod()]
        public void getChainTest() {
            //Chain api = new Chain();

            // Chain ID to test, 8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // http://explorer.factom.org/chain/8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182
            // Has 2 entries
            // Chain ID to test, 475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // http://explorer.factom.org/chain/475fbcef5e3a4e1621ed9a6fda5840c1d654715e55a8f5e514af0fb879ce0aec
            // Has 2 entries
            
           FactomApi api = new FactomApi();
           ChainHeadData chainHead = api.GetChainHead("8bc16ce03a246f4fdfd93a6729f8cb3132b4df592ae82bd6f8437b9898735182");
            EntryBlockData entryBlock = api.GetEntryBlockByKeyMR(chainHead);
             EntryData entry = api.GetEntryData(entryBlock.EntryList[0]);
        }
    }
}