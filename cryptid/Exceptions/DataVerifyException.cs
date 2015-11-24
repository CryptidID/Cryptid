using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is thrown when signed data can
    ///     not be verified using the provided public key.
    /// </summary>
    internal class DataVerifyException : Exception {
        public DataVerifyException() {
        }

        public DataVerifyException(string message) : base(message) {
        }

        public DataVerifyException(string message, Exception inner) : base(message, inner) {
        }
    }
}