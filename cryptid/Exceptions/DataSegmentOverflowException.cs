using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is to be thrown when an attempt is made
    ///     to store too much data in a DataSegment.
    /// </summary>
    internal class DataSegmentOverflowException : Exception {
        public DataSegmentOverflowException() {
        }

        public DataSegmentOverflowException(string message) : base(message) {
        }

        public DataSegmentOverflowException(string message, Exception inner) : base(message, inner) {
        }
    }
}