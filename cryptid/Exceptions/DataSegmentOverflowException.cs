using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class DataSegmentOverflowException : Exception {
        public DataSegmentOverflowException() {
        }

        public DataSegmentOverflowException(string message) : base(message) {
        }

        public DataSegmentOverflowException(string message, Exception inner) : base(message, inner) {
        }
    }
}
