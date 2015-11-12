using System;
using Cryptid;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cryptid.Tests {
    [TestClass]
    public class CandidateTest {

        private Candidate TestCandidate { get; set; }

        public CandidateTest() {
            TestCandidate = new Candidate {
                Dac = "LONG-SURNAME-LONG-SURNAME",
                Dad = "ELIZABETH-ELIZABETH",
                Dag = "6437 CRAZYCOOLLONG ST",
                Dai = "REALLYLONGCITY",
                Daj = "NY",
                Dak = new Candidate.PostalCode("14623"),
                Dau = new Candidate.Height(5, 7),
                Day = Candidate.EyeColor.Brown,
                Dbb = new DateTime(1990,1,1),
                Dbc = Candidate.Sex.Male,
                Dbd = DateTime.UtcNow,
                Dcg = "USA",
                Dcs = "LONG-SURNAME-LONG-SURNAME"
            };
        }

        [TestMethod]
        public void TestSerializeDeserialize() {
            Assert.IsFalse(Candidate.Deserialize(TestCandidate.Serialize()).Equals(TestCandidate));
        }
    }
}
