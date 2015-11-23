using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class AccessConfidenceTooLowException : Exception {
        public AccessConfidenceTooLowException() {
        }

        public AccessConfidenceTooLowException(string message) : base(message) {
        }

        public AccessConfidenceTooLowException(string message, Exception inner) : base(message, inner) {
        }
    }
}
