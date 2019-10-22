			using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class PS4Reader
    {
        const int PACKET_SIZE = 153;
        const int PACKET_HEADER = 21;
        const int POLISHED_PACKET_SIZE = 24;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "x", "circle", "square", "triangle", "l1", "l2", "lstick", "r1", "r2", "rstick", "share", "options", "ps", "trackpad"
        };

        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static float readAnalogButton(byte input)
        {
            return (float)input / 256;
        }


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 18; ++i)
                polishedPacket[i] = (byte)((packet[PACKET_HEADER+i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 6; ++i)
            {
                polishedPacket[18 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[18 + i] |= (byte)((packet[PACKET_HEADER + 18 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("rstick_x", readStick(polishedPacket[20]));
            outState.SetAnalog("rstick_y", readStick(polishedPacket[21]));
            outState.SetAnalog("lstick_x", readStick(polishedPacket[18]));
            outState.SetAnalog("lstick_y", readStick(polishedPacket[19]));

            outState.SetAnalog("l_trig", readStick(polishedPacket[22]));
            outState.SetAnalog("r_trig", readStick(polishedPacket[23]));

            return outState.Build();
        }
    }
}
