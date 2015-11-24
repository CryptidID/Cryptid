using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is thrown when an error occurs in
    ///     the scope of a Factom entry.
    /// </summary>
    internal class FactomEntryException : Exception {
        public FactomEntryException() {
        }

        public FactomEntryException(string message) : base(message) {
        }

        public FactomEntryException(string message, Exception inner) : base(message, inner) {
        }
    }
}