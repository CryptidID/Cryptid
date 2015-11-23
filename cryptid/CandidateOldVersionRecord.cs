#region

using System;
using System.Linq;
using System.Security.Cryptography;
using Cryptid.Exceptions;
using Cryptid.Utils;

#endregion

namespace Cryptid {
    /// <summary>
    ///     Old Version Record for use when updating candidates to new chains.
    /// </summary>
    public class CandidateOldVersionRecord : IRecord {
        /// <summary>
        ///     The length of a chain ID
        /// </summary>
        private const int ChainIdLength = 32;

        /// <summary>
        ///     The record identier for this record
        /// </summary>
        public static readonly byte[] CandidateOldVersionPrefix = {0x0, 0x69, 0x79, 0x89, 0x99, 0x0};

        /// <summary>
        ///     Create a new CandidateOldVersionRecord
        /// </summary>
        /// <param name="currentChain">The chain id of the current chain</param>
        /// <param name="nextChain">The chain id of the next chain</param>
        public CandidateOldVersionRecord(byte[] currentChain, byte[] nextChain) {
            CurrentChain = currentChain;
            NextChain = nextChain;
        }

        /// <summary>
        ///     The chain id of the current chain
        /// </summary>
        public byte[] CurrentChain { get; set; }

        /// <summary>
        ///     The chain id of the chain that updated this one
        /// </summary>
        public byte[] NextChain { get; set; }

        /// <summary>
        ///     Pack the record to storable data
        /// </summary>
        /// <param name="privKey">The private key to sign the data with</param>
        /// <returns>The packed record</returns>
        public byte[] Pack(RSAParameters privKey) {
            var data = CandidateOldVersionPrefix;
            data = data.Concat(CurrentChain).ToArray();
            data = data.Concat(NextChain).ToArray();
            data = data.Concat(Crypto.RSA_Sign(data, privKey)).ToArray();
            return data;
        }

        /// <summary>
        ///     Unpack storable data to a CandidateOldVersionRecord
        /// </summary>
        /// <param name="packed">The packed OldVersionRecord data</param>
        /// <param name="pubKey">The public key to verify with</param>
        /// <returns>The record object</returns>
        public static CandidateOldVersionRecord Unpack(byte[] packed, RSAParameters pubKey) {
            var sig = Arrays.CopyOfRange(packed, packed.Length - 512, packed.Length);
            var prefix = Arrays.CopyOfRange(packed, 0, CandidateOldVersionPrefix.Length);

            if (!Crypto.RSA_Verify(packed, sig, pubKey)) {
                throw new DataVerifyException("Could not cryptographically verify candidate update record");
            }

            if (CandidateOldVersionPrefix != prefix ||
                packed.Length != ChainIdLength*2 + 512 + CandidateOldVersionPrefix.Length) {
                throw new RecordDataInvalidException("Invalid data provided for packed candidate update record");
            }

            return new CandidateOldVersionRecord(Arrays.CopyOfRange(packed, 0, ChainIdLength),
                Arrays.CopyOfRange(packed, ChainIdLength, ChainIdLength*2));
        }
    }
}