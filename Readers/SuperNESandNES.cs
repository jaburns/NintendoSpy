using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    abstract public class ButtonsOnlyState : ISerialControllerState
    {
        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }

        public IReadOnlyDictionary <string, float> Analogs { get { return null; } }

        string[] _buttonNames;

        public ButtonsOnlyState (string[] buttonNames) {
            _buttonNames = buttonNames;
            Buttons = _buttons;
        }

        public void ReadFromPacket (byte[] packet)
        {
            if (packet.Length < _buttonNames.Length) return;

            for (int i = 0 ; i < _buttonNames.Length ; ++i) {
                if (string.IsNullOrEmpty (_buttonNames [i])) continue;
                _buttons [_buttonNames [i]] = packet[i] != 0x00;
            }
        }
    }

    sealed public class NES : ButtonsOnlyState
    {
        static readonly string[] BUTTONS = {
            "a", "b", "select", "start", "up", "down", "left", "right"
        };
        public NES () : base (BUTTONS) {}
    }

    sealed public class SuperNES : ButtonsOnlyState
    {
        static readonly string[] BUTTONS = {
            "b", "y", "select", "start", "up", "down", "left", "right", "a", "x", "l", "r", null, null, null, null
        };
        public SuperNES () : base (BUTTONS) {}
    }
}
