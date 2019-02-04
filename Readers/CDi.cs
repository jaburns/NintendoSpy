using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class CDi
    {
        const int PACKET_SIZE = 12;

        static readonly string[] BUTTONS = {
            "wired-up", "wired-down", "wired-left", "wired-right", "wired-1", "wired-2",
            "wireless-up", "wireless-down", "wireless-left", "wireless-right", "wireless-1", "wireless-2"
        };

        static float readAnalogButton(byte input)
        {
            return (float)(input) / 256;
        }

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            // Set double 1 buttons
            state.SetButton("wired-1a", packet[4] != 0x00);
            state.SetButton("wireless-1a", packet[10] != 0x00);

            // Handle 3 overriding other pushes
            if (packet[4] != 0x00 && packet[5] != 0x00)
            {
                state.SetButton("wired-3of3", true);
                state.SetButton("wired-1of3", false);
                state.SetButton("wired-1aof3", false);
                state.SetButton("wired-2of3", false);
            }
            else
            {
                state.SetButton("wired-3of3", false);
                state.SetButton("wired-1of3", packet[4] != 0x00);
                state.SetButton("wired-1aof3", packet[4] != 0x00);
                state.SetButton("wired-2of3", packet[5] != 0x00);
            }

            state.SetButton("wireless-1a", packet[10] != 0x00);

            state.SetAnalog("wired-analog_right", readAnalogButton(packet[3]));
            state.SetAnalog("wired-analog_left", readAnalogButton(packet[2]));
            state.SetAnalog("wired-analog_up", readAnalogButton(packet[0]));
            state.SetAnalog("wired-analog_down", readAnalogButton(packet[1]));

            state.SetAnalog("wireless-analog_right", readAnalogButton(packet[9]));
            state.SetAnalog("wireless-analog_left", readAnalogButton(packet[8]));
            state.SetAnalog("wireless-analog_up", readAnalogButton(packet[6]));
            state.SetAnalog("wireless-analog_down", readAnalogButton(packet[7]));

            float x = 0;
            float y = 0;

            if (packet[2] > 0)
                x = -1*readAnalogButton(packet[2]);
            else if (packet[3] > 0)
                x = readAnalogButton(packet[3]);

            if (packet[0] > 0)
                y = -1 * readAnalogButton(packet[0]);
            else if (packet[1] > 0)
                y = readAnalogButton(packet[1]);

            state.SetAnalog("stick_x", x);
            state.SetAnalog("stick_y", y);
            SignalTool.SetMouseProperties(x, y, state);

            return state.Build ();
        }
    }
}
