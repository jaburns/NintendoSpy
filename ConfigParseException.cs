using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy
{
    public class ConfigParseException : Exception {
        public ConfigParseException()
            : base () {}
        public ConfigParseException (string message)
            : base (message) {}
        public ConfigParseException (string format, params object[] args)
            : base (string.Format (format, args)) {}
        public ConfigParseException (string message, Exception innerException)
            : base (message, innerException) {}
        public ConfigParseException (string format, Exception innerException, params object[] args)
            : base (string.Format (format, args), innerException) {}
    }
}
