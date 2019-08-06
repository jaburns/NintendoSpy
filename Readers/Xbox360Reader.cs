using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Xbox360Reader
    {
        const int PACKET_SIZE = 96;
        const int POLISHED_PACKET_SIZE = 18;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "start", "back", "l3", "r3", "lb", "rb", "xbox", null, "a", "b", "x", "y"
        };

        static float readTrigger(byte input)
        {
            return (float)input / 255;
        }

        static float readStick(short input)
        {
            return (float)input / 32768;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 16; ++i)
                polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 2; ++i)
            {
                packet[16 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[16 + i] |= (byte)((packet[16 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            short[] sticks = new short[4];

            for (int i = 0; i < 4; ++i)
            {
                sticks[i] = 0;
                for (byte j = 0; j < 16; ++j)
                {
                    sticks[i] |= (short)((packet[32 + (i * 16 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("trig_l", readTrigger(polishedPacket[16]));
            outState.SetAnalog("trig_r", readTrigger(polishedPacket[17]));

            outState.SetAnalog("rstick_x", readStick(sticks[2]));
            outState.SetAnalog("rstick_y", readStick(sticks[3]));
            outState.SetAnalog("lstick_x", readStick(sticks[0]));
            outState.SetAnalog("lstick_y", readStick(sticks[1]));

            return outState.Build();
        }
    }
}
