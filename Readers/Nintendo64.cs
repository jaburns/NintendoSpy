using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Nintendo64
    {
        const int PACKET_SIZE = 32;

        static readonly string[] BUTTONS = {
            "a", "b", "z", "start", "up", "down", "left", "right", null, null, "l", "r", "cup", "cdown", "cleft", "cright"
        };

        static float readStick (byte input) {
            return (float)((sbyte)input) / 128;
        }

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            state.SetAnalog ("stick_x", readStick (SignalTool.readByte (packet, BUTTONS.Length    )));
            state.SetAnalog ("stick_y", readStick (SignalTool.readByte (packet, BUTTONS.Length + 8)));

            return state.Build ();
        }
    }
}
