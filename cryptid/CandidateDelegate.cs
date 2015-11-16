using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using cryptid;
using cryptid.Factom.API;
using Cryptid.Factom.API;
using Cryptid.Utils;
using SourceAFIS;
using SourceAFIS.Simple;

namespace Cryptid {
    using ChainHeadData = DataStructs.ChainHeadData;
    using EntryBlockData = DataStructs.EntryBlockData;
    using EntryData = DataStructs.EntryData;

    public static class CandidateDelegate {
        /// <summary>
        /// Enroll a candidate a to Factom. A new chain is created and the candidate data is packed and split into entries for that chain.
        /// This operation is irreversable.
        /// </summary>
        /// <param name="c">The candidate to enroll</param>
        /// <param name="password">The password provided by the candidate</param>
        /// <param name="privKey">The private key to pack the data with</param>
        /// <returns>The chain ID of the enrolled candidate</returns>
        public static byte[] EnrollCandidate(Candidate c, string password, RSAParameters privKey) {
            byte[] packed = Pack(c, password, privKey);
            var candidate = Unpack(packed, password, privKey);
            //Upload to factom, return chain id
            EntryData entry = new EntryData();
            entry.Content = Arrays.ByteArrayToHex(packed);
            Entry entryApi = new Entry();
            Chain chainApi = new Chain();
            var chain = chainApi.NewChain(entry);
            chainApi.Commitchain(chain, Arrays.ByteArrayToHex(candidate.Uid)); // Name is UID in hex
            System.Threading.Thread.Sleep(11000);
            chainApi.RevealChain(chain);


            return null;
        }

        /// <summary>
        /// - Verifies that we are allowed to update this chain
        /// - Enrolls a new candidate chain with a CandidateOldVersionRecord referencing the old chain
        /// - Adds a CandidateUpdatedRecord to the old chain referencing the new chain
        /// - Adds a ChainUpdateRecord to the chain requesting update, forwarding it to the new chain.
        /// This operation is irreversable.
        /// </summary>
        /// <param name="newCandidate">The updated candidate information</param>
        /// <param name="password">The password provided by the candidate</param>
        /// <param name="privKey">The private key to pack the data with</param>
        /// <param name="chainToUpdate">The chain ID of the chain to be updated</param>
        /// <returns>The chain ID pointing to the updated candidate</returns>
        public static byte[] UpdateCandidate(Candidate newCandidate, string password, RSAParameters privKey,
            byte[] chainToUpdate) {
            byte[] packed = Pack(newCandidate, password, privKey);
            return null;
        }

        /// <summary>
        /// Fully verify a user from a chain. Full verify includes fingerprint, password and signature check
        /// </summary>
        /// <param name="chainId">The id of the chain to verify</param>
        /// <param name="password">The candidates password</param>
        /// <param name="fp">The candidates fingerprint</param>
        /// <param name="publicKey">The public key to verify the data with</param>
        /// <returns>Whether or not the chain data is valid</returns>
        public static bool FullVerifyFromChain(byte[] chainId, string password, Fingerprint fp, RSAParameters publicKey) {
            return false;
        }

        /// <summary>
        /// Gets the candidates packed bytes from the specified chain
        /// </summary>
        /// <param name="chainId">The id of the chain to get the candidate data from</param>
        /// <returns>The packed candidate data</returns>
        public static byte[] GetPackedCandidate(byte[] chainId) {
            return null;
        }

        /// <summary>
        /// Get all the UIDs used by a candidate (Follows OldVersionRecords and UpdatedRecords)
        /// </summary>
        /// <param name="chainId">The id of a chain owned by the candidate</param>
        /// <returns></returns>
        public static byte[][] GetCandidateUIDHistory(byte chainId) {
            return null;
        }

        /// <summary>
        /// Pack a candidate object into storable data
        /// </summary>
        /// <param name="c">The candidate object to pack</param>
        /// <param name="password">The password provided by the candidate</param>
        /// <param name="privKey">The private key to sign the data with</param>
        /// <returns></returns>
        public static byte[] Pack(Candidate c, string password, RSAParameters privKey) {
            // Serialize with MsgPack
            var data = c.Serialize();

            // Encrypted serialized data with password
            data = Crypto.AES_Encrypt(data, password);

            // Generate a unique ID by hashing the encrypted serialized data
            var uid = SHA512.Create().ComputeHash(data);

            // Append the uid to the front of encrypted data
            data = uid.Concat(data).ToArray();

            // Sign the data and append the RSA signature to the end
            data = data.Concat(Crypto.RSA_Sign(data, privKey)).ToArray();

            return data;
        }

        /// <summary>
        /// Unpack a candidate object from storable data
        /// </summary>
        /// <param name="packed">The packed candidate data</param>
        /// <param name="password">The password provided by the candidate</param>
        /// <param name="pubKey">The public key to verify the data with</param>
        /// <returns>The candidate object</returns>
        public static Candidate Unpack(byte[] packed, string password, RSAParameters pubKey) {
            // Check if the RSA signature is valid
            if (!VerifySignature(packed, pubKey))
                throw new Exception("The packed data could not be cryptographically verified.");

            // Extract the UID for later
            var uid = Arrays.CopyOfRange(packed, 0, 64);

            // Decrypt candidate data
            var data = Crypto.AES_Decrypt(Arrays.CopyOfRange(packed, 64, packed.Length - 512), password);

            if (data == null) throw new Exception("The provided password was not correct.");

            // Set the candidate uid and deserialize
            var c = Candidate.Deserialize(data);
            c.Uid = uid;

            return c;
        }

        /// <summary>
        /// Verify the signature of packed candidate data
        /// </summary>
        /// <param name="packed">The packed candidate data</param>
        /// <param name="pubKey">The public key to verify with</param>
        /// <returns></returns>
        public static bool VerifySignature(byte[] packed, RSAParameters pubKey) {
            var sig = Arrays.CopyOfRange(packed, packed.Length - 512, packed.Length);
            var data = Arrays.CopyOfRange(packed, 0, packed.Length - 512);
            return Crypto.RSA_Verify(data, sig, pubKey);
        }

        /// <summary>
        /// Verify a candidates fingerprint
        /// </summary>
        /// <param name="c">The candidate to compare against</param>
        /// <param name="fp">The provided fingerprint</param>
        /// <returns>A float from 0 to 100 which represents the strength of the fingerprint match. Higher is better.</returns>
        public static float VerifyFingerprint(Candidate c, Fingerprint fp) {
            AfisEngine afis = new AfisEngine();

            Person test = new Person(fp);
            afis.Extract(test);

            Person candidate = new Person(c.Fingerprint);

            return afis.Verify(candidate, test);
        }
    }
}