using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using cryptid;
using cryptid.Factom.API;
using Cryptid.Factom.API;
using Cryptid.Utils;
using iTextSharp.text;
using SourceAFIS;
using SourceAFIS.Simple;
using ChainHeadData = cryptid.Factom.API.DataStructs.ChainHeadData;
using EntryBlockData = cryptid.Factom.API.DataStructs.EntryBlockData;
using EntryData = cryptid.Factom.API.DataStructs.EntryData;

namespace Cryptid {
    public static class CandidateDelegate {
        private const string FactomWallet = "CCNEntryCreds";

        private const int ExtIDsLength = 102; //2 byte header per ExtID + 2 sha256 hash + 32 byte random string

        /// <summary>
        /// Generates the ExtIDs for a packed candidate
        /// </summary>
        /// <param name="packedCandidate">The packed candidate to generate for</param>
        /// <returns>An array of ExtIDs</returns>
        public static byte[][] GenerateExtIDs(byte[] packedCandidate) {
            return new[] { Crypto.SHA256d(packedCandidate), Crypto.CRYPTID_SALT_HASH, Encoding.UTF8.GetBytes(Strings.RandomString(32)) };
        } 

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
            byte[] hexPacked = Encoding.UTF8.GetBytes(Convert.ToBase64String(packed));

            Entry entryApi = new Entry();
            Chain chainApi = new Chain();

            Chain.ChainType factomChain = null;

            foreach (DataSegment segment in DataSegment.Segmentize(hexPacked, firstSegmentLength: DataSegment.DefaultMaxSegmentLength - ExtIDsLength)) {
                byte[] dataToUpload = segment.Pack();
                var factomEntry = entryApi.NewEntry(dataToUpload, null, null);
                if (segment.CurrentSegment == 0) {
                    //New chain
                    factomEntry.ExtIDs = GenerateExtIDs(packed);
                    factomChain = chainApi.NewChain(factomEntry);               
                    chainApi.CommitChain(factomChain, FactomWallet); // Wallet Name
                    System.Threading.Thread.Sleep(10100);
                    chainApi.RevealChain(factomChain);
                }
                else {
                    //new entry
                    factomEntry.ChainID = factomChain.ChainId;
                    entryApi.CommitEntry(factomEntry, FactomWallet);
                    System.Threading.Thread.Sleep(10100);
                    entryApi.RevealEntry(factomEntry);
                }
            }

            return factomChain.ChainId;
        }

        /// <summary>
        /// - Verifies that we are allowed to update this chain
        /// - Enrolls a new candidate chain with a CandidateOldVersionRecord referencing the old chain
        /// - Adds a OldVersionRecord to the old chain referencing the new chain
        /// - Adds a ChainUpdateRecord to the chain requesting update, forwarding it to the new chain.
        /// This operation is irreversable.
        /// </summary>
        /// <param name="newCandidate">The updated candidate information</param>
        /// <param name="password">The password provided by the candidate</param>
        /// <param name="privKey">The private key to pack the data with</param>
        /// <param name="chainToUpdate">The chain ID of the chain to be updated</param>
        /// <returns>The chain ID pointing to the updated candidate</returns>
        public static byte[] UpdateCandidate(Candidate newCandidate, string password, Fingerprint fp, RSAParameters privKey, byte[] chainToUpdate) {
            if (FullVerifyFromChain(chainToUpdate, password, fp, privKey) < 50f) throw new Exception("Access confidence to low to update candidate.");
            
            byte[] newChainId = EnrollCandidate(newCandidate, password, privKey);

            CandidateOldVersionRecord ovr = new CandidateOldVersionRecord(chainToUpdate, newChainId);
            CandidateUpdatedRecord cur = new CandidateUpdatedRecord(chainToUpdate, newChainId);

            Entry entryApi = new Entry();
            EntryData oldRecordEntry = entryApi.NewEntry(ovr.Pack(privKey), null, Arrays.ByteArrayToHex(newChainId));
            EntryData newRecordEntry = entryApi.NewEntry(cur.Pack(privKey), null, Arrays.ByteArrayToHex(chainToUpdate));

            entryApi.CommitEntry(oldRecordEntry, FactomWallet);
            entryApi.CommitEntry(newRecordEntry, FactomWallet);

            System.Threading.Thread.Sleep(10100);

            entryApi.RevealEntry(oldRecordEntry);
            entryApi.RevealEntry(newRecordEntry);

            return newChainId;;
        }

        /// <summary>
        /// Fully verify a user from a chain. Full verify includes fingerprint, password and signature check
        /// </summary>
        /// <param name="chainId">The id of the chain to verify</param>
        /// <param name="password">The candidates password</param>
        /// <param name="fp">The candidates fingerprint</param>
        /// <param name="publicKey">The public key to verify the data with</param>
        /// <returns>The threshold at which the fingerprint verified</returns>
        public static float FullVerifyFromChain(byte[] chainId, string password, Fingerprint fp, RSAParameters publicKey) {
            var packed = GetPackedCandidate(chainId);
            return VerifyFingerprint(Unpack(packed, password, publicKey), fp);
        }

        /// <summary>
        /// Gets the candidates packed bytes from the specified chain
        /// </summary>
        /// <param name="chainId">The id of the chain to get the candidate data from</param>
        /// <returns>The packed candidate data</returns>
        public static byte[] GetPackedCandidate(byte[] chainId) {
            Entry entryApi = new Entry();
            var entries = entryApi.GetAllChainEntries(chainId);
            var entriesData = entries.Select(e => entryApi.GetEntryData(e).Content).ToList();
            List<DataSegment> segments = new List<DataSegment>();
            foreach (byte[] data in entriesData) {
                if(Bytes.StartsWith(data, DataSegment.DataSegmentPrefix)) segments.Add(DataSegment.Unpack(data));
            }
            return DataSegment.Desegmentize(segments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="publicKey">The public key to verify the data with</param>
        /// <returns></returns>
        public static List<CandidateOldVersionRecord> GetOldVersionRecords(byte[] chainId, RSAParameters pubKey) {
            Entry entryApi = new Entry();
            var entries = entryApi.GetAllChainEntries(chainId);
            var entriesData = entries.Select(e => entryApi.GetEntryData(e).Content).ToList();
            List<CandidateOldVersionRecord> records = new List<CandidateOldVersionRecord>();
            foreach (byte[] data in entriesData) {
                if (Bytes.StartsWith(data, CandidateOldVersionRecord.CandidateOldVersionPrefix)) records.Add(CandidateOldVersionRecord.Unpack(data, pubKey));
            }
            return records;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="publicKey">The public key to verify the data with</param>
        /// <returns></returns>
        public static List<CandidateUpdatedRecord> GetCandidateUpdatedRecords(byte[] chainId, RSAParameters pubKey) {
            Entry entryApi = new Entry();
            var entries = entryApi.GetAllChainEntries(chainId);
            var entriesData = entries.Select(e => entryApi.GetEntryData(e).Content).ToList();
            List<CandidateUpdatedRecord> records = new List<CandidateUpdatedRecord>();
            foreach (byte[] data in entriesData) {
                if (Bytes.StartsWith(data, CandidateUpdatedRecord.UpdatedRecordPrefix)) records.Add(CandidateUpdatedRecord.Unpack(data, pubKey));
            }
            return records;
        }

        /// <summary>
        /// Get all the chains used by a candidate (Follows OldVersionRecords and UpdatedRecords)
        /// </summary>
        /// <param name="chainId">The id of a chain owned by the candidate</param>
        /// <returns>A list of all the chains owned by a candidate</returns>
        public static List<byte[]> GetCandidateChainHistory(byte chainId, RSAParameters pubKey) {
            List<IRecord> toSort = new List<IRecord>();
            List<byte[]> ret = new List<byte[]>();

            while (toSort.Count > 0) {
                IRecord curr = toSort[0];
                if (curr is CandidateUpdatedRecord) {
                    var cur = (CandidateUpdatedRecord) curr;
                    if (!ret.Any(x => x.SequenceEqual(cur.PreviousChain))) {
                        ret.Add(cur.PreviousChain);
                        toSort.AddRange(GetCandidateUpdatedRecords(cur.PreviousChain, pubKey));
                        toSort.AddRange(GetOldVersionRecords(cur.PreviousChain, pubKey));
                    }

                    if (!ret.Any(x => x.SequenceEqual(cur.CurrentChain))) {
                        ret.Add(cur.CurrentChain);
                        toSort.AddRange(GetCandidateUpdatedRecords(cur.CurrentChain, pubKey));
                        toSort.AddRange(GetOldVersionRecords(cur.CurrentChain, pubKey));
                    }
                } else if (curr is CandidateOldVersionRecord) {
                    var ovr = (CandidateOldVersionRecord) curr;
                    if (!ret.Any(x => x.SequenceEqual(ovr.NextChain))) {
                        ret.Add(ovr.NextChain);
                        toSort.AddRange(GetCandidateUpdatedRecords(ovr.NextChain, pubKey));
                        toSort.AddRange(GetOldVersionRecords(ovr.NextChain, pubKey));
                    }

                    if (!ret.Any(x => x.SequenceEqual(ovr.CurrentChain))) {
                        ret.Add(ovr.CurrentChain);
                        toSort.AddRange(GetCandidateUpdatedRecords(ovr.CurrentChain, pubKey));
                        toSort.AddRange(GetOldVersionRecords(ovr.CurrentChain, pubKey));
                    }
                }
                toSort.RemoveAt(0);
            }

            return ret;
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