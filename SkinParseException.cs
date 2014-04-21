using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy
{
    class SkinParseException : Exception
    {
        public SkinParseException()
            : base() { }

        public SkinParseException(string message)
            : base(message) { }

        public SkinParseException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public SkinParseException(string message, Exception innerException)
            : base(message, innerException) { }

        public SkinParseException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
