using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptid.Exceptions {
    class DataVerifyException : Exception {
        public DataVerifyException() {
        }

        public DataVerifyException(string message) : base(message) {
        }

        public DataVerifyException(string message, Exception inner) : base(message, inner) {
        }
    }
}
