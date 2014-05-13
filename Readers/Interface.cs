using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintendoSpy.Readers
{
    public class ControllerState
    {
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }
        public IReadOnlyDictionary <string, float> Analogs { get; private set;  }

        public ControllerState (IReadOnlyDictionary <string, bool> buttons, IReadOnlyDictionary <string, float> analogs)
        {
            Buttons = buttons;
            Analogs = analogs;
        }
    }

    public class ControllerStateBuilder
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

    public delegate void StateEventHandler (IControllerReader sender, ControllerState state);

    public interface IControllerReader
    {
        event StateEventHandler ControllerStateChanged;
        event EventHandler ControllerDisconnected;

        void Finish ();
    }
}
