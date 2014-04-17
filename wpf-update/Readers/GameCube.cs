using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class GameCube : ISerialControllerState
    {
        const int PACKET_SIZE = 64;

        static readonly string[] BUTTONS = {
            null, null, null, "start", "y", "x", "b", "a", null, "l", "r", "z", "up", "down", "right", "left"
        };

        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }

        Dictionary <string, ControlStickState> _sticks = new Dictionary <string, ControlStickState> ();
        public IReadOnlyDictionary <string, ControlStickState> Sticks { get; private set; }

        Dictionary <string, float> _analogs = new Dictionary <string, float> ();
        public IReadOnlyDictionary <string, float> Analogs { get; private set; }

        public GameCube () {
            Buttons = _buttons;
            Sticks = _sticks;
            Analogs = _analogs;
        }

        public void ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return;

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                _buttons [BUTTONS [i]] = packet[i] != 0x00;
            }

            Func <byte, float> readStick   = input => (float)(input - 128) / 128;
            Func <byte, float> readTrigger = input => (float)(input)       / 256;

            _sticks ["main"] = new ControlStickState {
                X = readStick (SignalTool.readByte (packet, BUTTONS.Length     )),
                Y = readStick (SignalTool.readByte (packet, BUTTONS.Length +  8))
            };
            _sticks ["c"] = new ControlStickState {
                X = readStick (SignalTool.readByte (packet, BUTTONS.Length + 16)),
                Y = readStick (SignalTool.readByte (packet, BUTTONS.Length + 24))
            };
            _analogs ["l"] = readTrigger (SignalTool.readByte (packet, BUTTONS.Length + 32));
            _analogs ["r"] = readTrigger (SignalTool.readByte (packet, BUTTONS.Length + 40));
        }
    }
}
