using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptidRest {
    class CandidateDataObjects {
        public class FullVerifyFromChain {
            public string ChainIdBase64 { get; set; }
            public string Password { get; set; }
            public string FingerprintImageBase64 { get; set; }
        }

        public class GetCandidateFromChain {
            public string ChainIdBase64 { get; set; }
            public string Password { get; set; }
        }

        public class UnpackCandidate {
            public string PackedBase64 { get; set; }
            public string Password { get; set; }
        }

        public class VerifySignature {
            public string PackedBase64 { get; set; }
        }
    }
}
