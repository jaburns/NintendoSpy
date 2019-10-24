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
        const int POLISHED_PACKET_SIZE = 30;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "x", "circle", "square", "triangle", "l1", "l2", "lstick", "r1", "r2", "rstick", "share", "options", "ps", "trackpad", "trackpad0_touch", "trackpad1_touch"
        };

        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static float readAnalogButton(byte input)
        {
            return (float)input / 256;
        }

        static float readTouchPad(byte input, int maxValue)
        {
            if (input > maxValue)
                return 1.0f;

            if (input < 0)
                return 0.0f;

            return (float)input / (float)maxValue;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 20; ++i)
                polishedPacket[i] = (byte)((packet[PACKET_HEADER+i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 10; ++i)
            {
                polishedPacket[18 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[18 + i] |= (byte)((packet[PACKET_HEADER + 20 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("rstick_x", readStick(polishedPacket[22]));
            outState.SetAnalog("rstick_y", readStick(polishedPacket[23]));
            outState.SetAnalog("lstick_x", readStick(polishedPacket[20]));
            outState.SetAnalog("lstick_y", readStick(polishedPacket[21]));

            outState.SetAnalog("l_trig", readStick(polishedPacket[24]));
            outState.SetAnalog("r_trig", readStick(polishedPacket[25]));

            if (polishedPacket[18] == 1) // touch
            {
                if (polishedPacket[17] == 1) // click
                {
                    outState.SetAnalog("touchpad_x3", readTouchPad(polishedPacket[26], 1920));
                    outState.SetAnalog("touchpad_y3", readTouchPad(polishedPacket[27], 943));
                }
                else
                {
                    outState.SetAnalog("touchpad_x1", readTouchPad(polishedPacket[26], 1920));
                    outState.SetAnalog("touchpad_y1", readTouchPad(polishedPacket[27], 943));
                }
            }

            if (polishedPacket[19] == 1) // touch
            {
                if (polishedPacket[17] == 1) // click
                {
                    outState.SetAnalog("touchpad_x4", readTouchPad(polishedPacket[28], 1920));
                    outState.SetAnalog("touchpad_y4", readTouchPad(polishedPacket[29], 943));
                }
                else
                {
                    outState.SetAnalog("touchpad_x2", readTouchPad(polishedPacket[28], 1920));
                    outState.SetAnalog("touchpad_y2", readTouchPad(polishedPacket[29], 943));
                }
            }

            return outState.Build();
        }
    }
}
