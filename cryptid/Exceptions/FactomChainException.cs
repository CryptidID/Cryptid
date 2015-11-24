using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is thrown when an error occurs in
    ///     the scope of a Factom chain.
    /// </summary>
    internal class FactomChainException : Exception {
        public FactomChainException() {
        }

        public FactomChainException(string message) : base(message) {
        }

        public FactomChainException(string message, Exception inner) : base(message, inner) {
        }
    }
}