using System;

namespace Cryptid.Exceptions {
    internal class RecordDataInvalidException : Exception {
        /// <summary>
        ///     This exception is thrown when an IRecord contains
        ///     invalid data.
        /// </summary>
        public RecordDataInvalidException() {
        }

        public RecordDataInvalidException(string message) : base(message) {
        }

        public RecordDataInvalidException(string message, Exception inner) : base(message, inner) {
        }
    }
}