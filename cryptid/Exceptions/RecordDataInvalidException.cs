using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class RecordDataInvalidException : Exception {
        public RecordDataInvalidException() {
        }

        public RecordDataInvalidException(string message) : base(message) {
        }

        public RecordDataInvalidException(string message, Exception inner) : base(message, inner) {
        }
    }
}
