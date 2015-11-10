using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cryptid {
    public class StringValue : Attribute {
        private readonly string _value;

        public StringValue(string value) {
            _value = value;
        }

        public string Value {
            get { return _value; }
        }
    }
}
