using System;
using Cryptid;
using Cryptid.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptidTests {
    [TestClass]
    public class DataSegmentTest {
        [TestMethod]
        public void TestSegmentize() {
            var data = new byte[70000];
            var r = new Random();
            r.NextBytes(data);

            var s = DataSegment.Segmentize(data, firstSegmentLength: DataSegment.DefaultMaxSegmentLength - 512);
            var outData = DataSegment.Desegmentize(s);

            Assert.IsTrue(Bytes.Equality(data, outData), "Pre and post segmentation data does not match");
        }
    }
}