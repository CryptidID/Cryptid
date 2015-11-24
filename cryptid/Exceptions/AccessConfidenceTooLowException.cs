using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is to be thrown when biometric comparison does not exceed
    ///     a given threshold. For example, if the desired fingerprint confidence is
    ///     50.0 but only a 25.0 is achieved.
    /// </summary>
    internal class AccessConfidenceTooLowException : Exception {
        public AccessConfidenceTooLowException() {
        }

        public AccessConfidenceTooLowException(string message) : base(message) {
        }

        public AccessConfidenceTooLowException(string message, Exception inner) : base(message, inner) {
        }
    }
}