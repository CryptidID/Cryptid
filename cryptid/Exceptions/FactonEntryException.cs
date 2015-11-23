using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class FactomEntryException : Exception {
        public FactomEntryException() {
        }

        public FactomEntryException(string message) : base(message) {
        }

        public FactomEntryException(string message, Exception inner) : base(message, inner) {
        }
    }
}
