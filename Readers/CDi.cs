using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class CDi
    {
        const int PACKET_SIZE = 6;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "1", "2"
        };

        static float readAnalogButton(byte input)
        {
            return (float)(input) / 256;
        }

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length - 2 ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            if (packet[4] != 0x00 && packet[5] != 0x00)
            {
                state.SetButton("3", true);
                state.SetButton("1", false);
                state.SetButton("1a", false);
                state.SetButton("2", false);
            }
            else
            {
                state.SetButton("3", false);
                state.SetButton("1", packet[4] != 0x00);
                state.SetButton("1a", packet[4] != 0x00);
                state.SetButton("2", packet[5] != 0x00);

            }

            

            state.SetAnalog("analog_right", readAnalogButton(packet[3]));
            state.SetAnalog("analog_left", readAnalogButton(packet[2]));
            state.SetAnalog("analog_up", readAnalogButton(packet[0]));
            state.SetAnalog("analog_down", readAnalogButton(packet[1]));

            return state.Build ();
        }
    }
}
