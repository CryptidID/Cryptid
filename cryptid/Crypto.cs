﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cryptid {
    public static class Crypto {
        private const int RSA_KEY_SIZE = 4096;

        public static readonly byte[] CRYPTID_SALT = {
            0x1c, 0x9f, 0x1d, 0x8a, 0x11, 0x98, 0xfe, 0x3f, 0xf7, 0xb7, 0x9a, 0xf1,
            0x0f, 0xf3, 0x30, 0xf0, 0x05, 0x49, 0x87, 0x9c, 0x92, 0x70, 0xe2, 0x9c, 0x6c, 0x88, 0xff, 0x2b, 0xfd, 0x13,
            0xe1, 0x87, 0xfe, 0xa2, 0x2b, 0xe1, 0x59, 0xf5, 0xc0, 0x39, 0x3b, 0xcc, 0x34, 0x52, 0xf9, 0x0f, 0x85, 0x3e,
            0x32, 0x57, 0x37, 0xc8, 0xe7, 0x4b, 0xee, 0xa9, 0x55, 0x64, 0x1e, 0xe2, 0xd5, 0x7d, 0x48, 0x1b, 0x48, 0xff,
            0x2c, 0xdf, 0xd6, 0x12, 0x5e, 0x90, 0x99, 0x42, 0x74, 0x67, 0x6a, 0x66, 0xf2, 0x1e, 0x20, 0xfc, 0x10, 0xa1,
            0xbb, 0xa8, 0x1b, 0x23, 0x3d, 0xc9, 0x09, 0xc8, 0x5e, 0x9c, 0x8d, 0x9b, 0x68, 0xc5, 0xa8, 0xa1, 0xe4, 0xde,
            0x58, 0x9d, 0x80, 0x15, 0x50, 0x10, 0x6a, 0x6d, 0x1a, 0x8f, 0x80, 0xb6, 0x50, 0x95, 0xea, 0x59, 0xd7, 0x91,
            0xd0, 0x90, 0xc6, 0xd8, 0x9f, 0xe1, 0xa8, 0x04, 0x0f, 0xcd, 0xcd, 0x3f, 0x5b, 0x35, 0xcf, 0x82, 0x67, 0x18,
            0x38, 0x96, 0x4e, 0x28, 0xca, 0x87, 0x15, 0xa1, 0x57, 0x07, 0x80, 0x22, 0x37, 0x38, 0xae, 0xf1, 0xf4, 0x8d,
            0xd5, 0x26, 0x22, 0x78, 0xa1, 0xc7, 0x88, 0x53, 0x11, 0x42, 0x9f, 0x08, 0x13, 0xff, 0x24, 0x87, 0x4b, 0x61,
            0x8a, 0x5b, 0x56, 0x03, 0x84, 0x69, 0xf0, 0x21, 0xb7, 0x01, 0x34, 0x1e, 0x24, 0xb6, 0xa9, 0x96, 0x2c, 0x94,
            0x9d, 0x1c, 0x47, 0x61, 0x3f, 0xf4, 0xa1, 0x5e, 0x4d, 0xe1, 0xdd, 0x62, 0xed, 0x80, 0xa8, 0x10, 0x39, 0xff,
            0xc5, 0x50, 0xfd, 0xd9, 0x41, 0xc8, 0xa2, 0x11, 0xbe, 0x69, 0x65, 0x19, 0x2c, 0xb1, 0x73, 0x97, 0xb8, 0x93,
            0xfa, 0x2c, 0x81, 0xaf, 0x3a, 0x40, 0x40, 0xd2, 0xf9, 0xf5, 0xbf, 0x8a, 0x3c, 0x68, 0x03, 0x93, 0x9f, 0xb8,
            0xc7, 0xf8, 0x2f, 0xfe, 0xfd, 0x20, 0xb8, 0x63, 0x80, 0xa0, 0xa3, 0x18, 0x9f, 0xf1, 0x70, 0x3f, 0x34, 0xa9,
            0x12, 0x8c, 0x98, 0x52, 0x50, 0x9c, 0xef, 0x1a, 0xaa, 0x9d, 0x93, 0x20, 0x6a, 0x92, 0x24, 0x24, 0xf4, 0x88,
            0xd7, 0xef, 0x98, 0xe2, 0x28, 0xad, 0x35, 0x84, 0x78, 0xcf, 0xed, 0x66, 0xca, 0xce, 0xef, 0x69, 0xf8, 0x58,
            0x3f, 0x27, 0xda, 0xc8, 0xd0, 0xab, 0xca, 0x91, 0xe9, 0x1d, 0x31, 0x93, 0xaf, 0xe0, 0x34, 0x5b, 0x8f, 0xf2,
            0x48, 0xa9, 0x1d, 0x94, 0x32, 0x4e, 0xcd, 0x4c, 0xb3, 0x17, 0x6c, 0x55, 0xed, 0x45, 0x8c, 0xcf, 0x32, 0xa2,
            0x07, 0xe0, 0x09, 0x30, 0xbe, 0x8c, 0x83, 0x2a, 0xe6, 0xf3, 0x5e, 0x97, 0x4d, 0x57, 0x1b, 0x9f, 0x60, 0x1a,
            0xd2, 0x22, 0x75, 0xa4, 0xe8, 0x13, 0xba, 0x16, 0xe1, 0xf9, 0xb6, 0x69, 0x01, 0xb4, 0x05, 0xa2, 0xc2, 0x4b,
            0xc9, 0x72
        };

        public static readonly byte[] CRYPTID_SALT_HASH = SHA256.Create().ComputeHash(Crypto.CRYPTID_SALT);

        internal static byte[] AES_Encrypt(byte[] data, string password) {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            return AES_Encrypt(data, passwordBytes);
        }

        internal static byte[] AES_Decrypt(byte[] data, string password) {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            return AES_Decrypt(data, passwordBytes);
        }

        internal static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
            byte[] encryptedBytes = null;

            using (var ms = new MemoryStream()) {
                using (var AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, CRYPTID_SALT, 1000);
                    AES.Key = key.GetBytes(AES.KeySize/8);
                    AES.IV = key.GetBytes(AES.BlockSize/8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        internal static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
            byte[] decryptedBytes = null;

            using (var ms = new MemoryStream()) {
                using (var AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, CRYPTID_SALT, 1000);
                    AES.Key = key.GetBytes(AES.KeySize/8);
                    AES.IV = key.GetBytes(AES.BlockSize/8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static byte[] RSA_Sign(byte[] data, RSAParameters privKey) {
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider()) {
                try {
                    rsa.ImportParameters(privKey);
                    signedBytes = rsa.SignData(data, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException e) {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return signedBytes;
        }

        public static bool RSA_Verify(byte[] data, byte[] sig, RSAParameters pubKey) {
            using (var rsa = new RSACryptoServiceProvider()) {
                try {
                    rsa.ImportParameters(pubKey);
                    return rsa.VerifyData(data, CryptoConfig.MapNameToOID("SHA512"), sig);
                }
                catch (CryptographicException e) {
                    Console.WriteLine(e.Message);
                }
                finally {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return false;
        }

        public sealed class Crc32 : HashAlgorithm {
            public const UInt32 DefaultPolynomial = 0xedb88320u;
            public const UInt32 DefaultSeed = 0xffffffffu;

            static UInt32[] defaultTable;

            readonly UInt32 seed;
            readonly UInt32[] table;
            UInt32 hash;

            public Crc32()
                : this(DefaultPolynomial, DefaultSeed) {
            }

            public Crc32(UInt32 polynomial, UInt32 seed) {
                table = InitializeTable(polynomial);
                this.seed = hash = seed;
            }

            public override void Initialize() {
                hash = seed;
            }

            protected override void HashCore(byte[] array, int ibStart, int cbSize) {
                hash = CalculateHash(table, hash, array, ibStart, cbSize);
            }

            protected override byte[] HashFinal() {
                var hashBuffer = UInt32ToBigEndianBytes(~hash);
                HashValue = hashBuffer;
                return hashBuffer;
            }

            public override int HashSize { get { return 32; } }

            public static UInt32 Compute(byte[] buffer) {
                return Compute(DefaultSeed, buffer);
            }

            public static UInt32 Compute(UInt32 seed, byte[] buffer) {
                return Compute(DefaultPolynomial, seed, buffer);
            }

            public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer) {
                return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
            }

            static UInt32[] InitializeTable(UInt32 polynomial) {
                if (polynomial == DefaultPolynomial && defaultTable != null)
                    return defaultTable;

                var createTable = new UInt32[256];
                for (var i = 0; i < 256; i++) {
                    var entry = (UInt32)i;
                    for (var j = 0; j < 8; j++)
                        if ((entry & 1) == 1)
                            entry = (entry >> 1) ^ polynomial;
                        else
                            entry = entry >> 1;
                    createTable[i] = entry;
                }

                if (polynomial == DefaultPolynomial)
                    defaultTable = createTable;

                return createTable;
            }

            static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size) {
                var crc = seed;
                for (var i = start; i < size - start; i++)
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                return crc;
            }

            static byte[] UInt32ToBigEndianBytes(UInt32 uint32) {
                var result = BitConverter.GetBytes(uint32);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(result);

                return result;
            }
=======
        public static byte[] SHA256d(byte[] toHash) {
            return SHA256.Create().ComputeHash(SHA256.Create().ComputeHash(toHash));
>>>>>>> 3d9c3d67af5e0d194981fc70689ea05313411e9a
        }
    }
}
