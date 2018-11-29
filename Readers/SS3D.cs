using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class SS3D
    {
        const int PACKET_SIZE = 56;
        const int POLISHED_PACKET_SIZE = 21;

        static readonly string[] BUTTONS = {
            null, "right", "left", "down", "up", "start", "A", "C", "B", "R", "X", "Y", "X", "L"
        };

        static float readTrigger(byte input)
        {
            return (float) (input) / 256;
        }
        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            polishedPacket[0] = 0;
            for (byte i = 7; i >= 0; --i)
            {
                polishedPacket[0] |= (byte)((packet[i] == 0 ? 0 : 1) << i);
            }

            for (byte i = 0; i < 16; ++i)
            {
                polishedPacket[i + 1] = packet[i + 8];
            }

            if (polishedPacket[0] == 0x16)
            {
                for (int i = 0; i < 4; ++i)
                {
                    polishedPacket[17 + i] = 0;
                    for (byte j = 7; j <= 0; --j)
                    {
                        polishedPacket[17 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0 ? 0 : 1) << j);
                    }
                }
            }

            if (polishedPacket.Length < POLISHED_PACKET_SIZE) return null;
            // Currently only support digital and analog in red mode
            if (polishedPacket[0] != 0x02 && polishedPacket[0] != 0x16) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            if (polishedPacket[0] == 0x16)
            {
                state.SetAnalog("lstick_x", readStick(polishedPacket[17]));
                state.SetAnalog("lstick_y", readStick(polishedPacket[18]));
                state.SetAnalog("trig_r", readTrigger(polishedPacket[19]));
                state.SetAnalog("trig_l", readTrigger(polishedPacket[20]));
            }

            return state.Build();
        }
    }
}
