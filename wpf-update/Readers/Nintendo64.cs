using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class Nintendo64 : ISerialControllerState
    {
        const int PACKET_SIZE = 32;

        static readonly string[] BUTTONS = {
            "a", "b", "z", "start", "up", "down", "left", "right", null, null, "l", "r", "cup", "cdown", "cleft", "cright"
        };

        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }

        Dictionary <string, ControlStickState> _sticks = new Dictionary <string, ControlStickState> ();
        public IReadOnlyDictionary <string, ControlStickState> Sticks { get; private set; }

        public IReadOnlyDictionary <string, float> Analogs { get { return null; } }

        public Nintendo64 () {
            Buttons = _buttons;
            Sticks = _sticks;
        }

        public void ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return;

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                _buttons [BUTTONS [i]] = packet[i] != 0x00;
            }

            Func <byte, float> readStick = input => (float)((sbyte)input) / 128;

            _sticks ["main"] = new ControlStickState {
                X = readStick (SignalTool.readByte (packet, BUTTONS.Length     )),
                Y = readStick (SignalTool.readByte (packet, BUTTONS.Length +  8))
            };
        }
    }
}
