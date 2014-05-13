using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class ControllerStateBuilder
    {
        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        Dictionary <string, float> _analogs = new Dictionary <string, float> ();

        public void SetButton (string name, bool value) {
            _buttons [name] = value;
        }

        public void SetAnalog (string name, float value) {
            _analogs [name] = value;
        }

        public ControllerState Build () {
            return new ControllerState (_buttons, _analogs);
        }
    }
}
