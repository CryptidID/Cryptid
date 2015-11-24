using System;

namespace Cryptid.Exceptions {
    /// <summary>
    ///     This exception is thrown when an incorrect password is
    ///     provided to decrypt data.
    /// </summary>
    public class PasswordIncorrectException : Exception {
        public PasswordIncorrectException() {
        }

        public PasswordIncorrectException(string message) : base(message) {
        }

        public PasswordIncorrectException(string message, Exception inner) : base(message, inner) {
        }
    }
}