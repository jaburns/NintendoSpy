using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class XInputReader : IControllerReader
    {
        public IControllerState State {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler ControllerStateChanged;
    }
}
