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

        Dictionary <string, float> _analogs = new Dictionary <string, float> ();
        public IReadOnlyDictionary <string, float> Analogs { get; private set; }

        public Nintendo64 () {
            Buttons = _buttons;
            Analogs = _analogs;
        }

        public void ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return;

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                _buttons [BUTTONS [i]] = packet[i] != 0x00;
            }

            Func <byte, float> readStick = input => (float)((sbyte)input) / 128;

            _analogs ["stick_x"] = readStick (SignalTool.readByte (packet, BUTTONS.Length     ));
            _analogs ["stick_y"] = readStick (SignalTool.readByte (packet, BUTTONS.Length +  8));
        }
    }
}
