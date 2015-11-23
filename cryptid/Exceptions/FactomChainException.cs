using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class FactomChainException : Exception {
        public FactomChainException() {
        }

        public FactomChainException(string message) : base(message) {
        }

        public FactomChainException(string message, Exception inner) : base(message, inner) {
        }
    }
}
