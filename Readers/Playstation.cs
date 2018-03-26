using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Playstation
    {
        const int PACKET_SIZE = 21;

        static readonly string[] BUTTONS = {
            null, "select", "lstick", "rstick", "start", "up", "right", "down", "left", "l2", "r2", "l1", "r1", "triangle", "circle", "x", "square"
        };

        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;
            // Currently only support digital and analog in red mode
            if (packet[0] != 0x41 && packet[0] != 0x73) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            if (packet[0] == 0x73)
            {
                state.SetAnalog("rstick_x", readStick(packet[17]));
                state.SetAnalog("rstick_y", readStick(packet[18]));
                state.SetAnalog("lstick_x", readStick(packet[19]));
                state.SetAnalog("lstick_y", readStick(packet[20]));
            }

            return state.Build ();
        }
    }
}
