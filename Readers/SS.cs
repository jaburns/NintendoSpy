using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class SS
    {
        const int PACKET_SIZE = 13;

        static readonly string[] BUTTONS = {
            "Z", "Y", "X", "R", "B", "C", "A", "start", "up", "down", "left", "right", "L"
        };

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            return state.Build ();
        }
    }
}
