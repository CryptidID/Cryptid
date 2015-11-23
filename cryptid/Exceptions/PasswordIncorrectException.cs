using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class PasswordIncorrectException : Exception {
        public PasswordIncorrectException() {
        }

        public PasswordIncorrectException(string message) : base(message) {
        }

        public PasswordIncorrectException(string message, Exception inner) : base(message, inner) {
        }
    }
}
