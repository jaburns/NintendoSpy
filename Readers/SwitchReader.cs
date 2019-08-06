using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class SwitchReader
    {
        const int PACKET_SIZE = 57;
        const int POLISHED_PACKET_SIZE = 28;

        static readonly string[] BUTTONS = {
            "y", "x", "b", "a", null, null, "r", "zr", "-", "+", "rs", "ls", "home", "capture", null, null, "down", "up", "right", "left", null, null, "l", "zl"
        };

        static float readStick(byte input)
        {
            if (input < 128)
                return (float)input / 128;

            return (float)(255-input) / -128;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 24; ++i)
                polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 4; ++i)
            {
                packet[24 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[24 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("rstick_x", readStick(polishedPacket[26]));
            outState.SetAnalog("rstick_y", readStick(polishedPacket[27]));
            outState.SetAnalog("lstick_x", readStick(polishedPacket[24]));
            outState.SetAnalog("lstick_y", readStick(polishedPacket[25]));

            return outState.Build();
        }
    }
}
