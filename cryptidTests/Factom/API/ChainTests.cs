using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cryptid.Factom.API;
using Cryptid.Utils;

namespace Cryptid.Factom.API.Tests {
    [TestClass()]
    class ChainTests {
        [TestMethod()]
        public void NewChainTest() {
            var api = new Chain();
            DataStructs.EntryData e = new DataStructs.EntryData();
            e.Content = Encoding.UTF8.GetBytes("Each directory listed in the Go path must have a prescribed structure:");
            e.ChainID = Encoding.UTF8.GetBytes("00511c298668bc5032a64b76f8ede6f119add1a64482c8602966152c0b936c77");
            var arr = new byte[2][] { Encoding.UTF8.GetBytes("a136bf2a5b81a671d3f0c168f4"), Encoding.UTF8.GetBytes("b35f223db2dced312581d22c46ba4117702d03") };
            e.ExtIDs = arr;

            var x = api.NewChain(e);
            Console.WriteLine(x.ChainId);
        }
    }
}
