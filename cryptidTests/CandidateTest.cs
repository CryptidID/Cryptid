using System;
using System.Drawing;
using System.Security.Cryptography;
using Cryptid.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceAFIS.Simple;

namespace Cryptid.Tests {
    [TestClass]
    public class CandidateTest {
        private const string Password = "-b{EC7R:mD+x+RFm";

        private readonly RSAParameters PrivateKey = Keys.PrivateKey("testdata/public.xml");
        private readonly RSAParameters PublicKey = Keys.PublicKey("testdata/public.xml");

        private Candidate TestCandidate {
            get {
                var c = new Candidate {
                    Dac = "LONG-SURNAME-LONG-SURNAME",
                    Dad = "ELIZABETH-ELIZABETH",
                    Dag = "6437 CRAZYCOOLLONG ST",
                    Dai = "REALLYLONGCITY",
                    Daj = "NY",
                    Dak = new Candidate.PostalCode("14623"),
                    Dau = new Candidate.Height(5, 7),
                    Day = Candidate.EyeColor.Brown,
                    Dbb = new DateTime(1990, 1, 1),
                    Dbc = Candidate.Sex.Male,
                    Dbd = DateTime.UtcNow,
                    Dcg = "USA",
                    Dcs = "LONG-SURNAME-LONG-SURNAME",
                    Fingerprint = new Fingerprint(),
                    Image = Image.FromFile("testdata/test-img.png")
                };

                c.Fingerprint.AsBitmap = new Bitmap("testdata/fingerprint-test.bmp");

                return c;
            }
        }

        [TestMethod]
        [DeploymentItem(@"testdata/fingerprint-test.bmp", "testdata")]
        [DeploymentItem(@"testdata/test-img.png", "testdata")]
        public void TestSerializeDeserialize() {
            var c = TestCandidate;
            Assert.AreEqual(Candidate.Deserialize(c.Serialize()), c);
        }

        [TestMethod]
        [DeploymentItem(@"testdata/fingerprint-test.bmp", "testdata")]
        [DeploymentItem(@"testdata/test-img.png", "testdata")]
        [DeploymentItem(@"testdata/private.xml", "testdata")]
        [DeploymentItem(@"testdata/public.xml", "testdata")]
        public void TestPackUnpack() {
            var c = TestCandidate;
            var packed = CandidateDelegate.Pack(c, Password, PrivateKey);
            Assert.AreEqual(c.Dcs, CandidateDelegate.Unpack(packed, Password, PublicKey).Dcs);
        }
    }
}