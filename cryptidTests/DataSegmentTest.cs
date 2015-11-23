using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cryptid;

namespace CryptidTests {
    [TestClass]
    public class DataSegmentTest {
        [TestMethod]
        public void TestSegmentize() {
            byte[] data = new byte[70000];
            Random r = new Random();
            r.NextBytes(data);

            var s = DataSegment.Segmentize(data, firstSegmentLength: DataSegment.DefaultMaxSegmentLength - 512);
            var outData = DataSegment.Desegmentize(s);

            Assert.IsTrue(Cryptid.Utils.Bytes.Equality(data, outData), "Pre and post segmentation data does not match");
        }
    }
}
