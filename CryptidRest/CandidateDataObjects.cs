namespace CryptidRest {
    /// <summary>
    /// The data objects for CandidateController
    /// </summary>
    internal class CandidateDataObjects {
        /// <summary>
        /// Data object for FullVerifyFromChain
        /// </summary>
        public class FullVerifyFromChain {
            /// <summary>
            /// The base64 encoded factom chain ID for the candidate
            /// </summary>
            public string ChainIdBase64 { get; set; }
            /// <summary>
            /// The password for this candidate
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// The base64 encoded fingerprint image for this candidate
            /// </summary>
            public string FingerprintImageBase64 { get; set; }
        }

        /// <summary>
        /// Data object for GetCandidateFromChain
        /// </summary>
        public class GetCandidateFromChain {
            /// <summary>
            /// The base64 encoded factom chain ID for the candidate
            /// </summary>
            public string ChainIdBase64 { get; set; }
            /// <summary>
            /// The password of the candidate
            /// </summary>
            public string Password { get; set; }
        }

        /// <summary>
        /// Data object for UnpackCandidate
        /// </summary>
        public class UnpackCandidate {
            /// <summary>
            /// The base64 encoded packed candidate data
            /// </summary>
            public string PackedBase64 { get; set; }
            /// <summary>
            /// The password of the candidate
            /// </summary>
            public string Password { get; set; }
        }

        /// <summary>
        /// Data object for VerifySignature
        /// </summary>
        public class VerifySignature {
            /// <summary>
            /// The base64 encoded packed candidate data
            /// </summary>
            public string PackedBase64 { get; set; }
        }
    }
}