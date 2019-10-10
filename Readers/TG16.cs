using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Tg16
    {
        const int PACKET_SIZE = 12;

        static readonly string[] BUTTONS = {
            "up", "right", "down", "left", "1", "2", "select", "run", "3", "4", "5", "6"
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
