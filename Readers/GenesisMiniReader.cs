using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class GenesisMiniReader
    {
        const int PACKET_SIZE = 58;
        const int POLISHED_PACKET_SIZE = 28;

        static readonly string[] THREE_BUTTONS = {
            null, null, null, null, null, "b", "a", null, null, "c", null, null, null, "start", null, null
        };

        static readonly string[] SIX_BUTTONS = {
            null, null, null, null, "x", "a", "b", "y", "c", "z", "l", "r", "mode", "start", null, null
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

          
            for (int i = 40; i < 56; ++i)
                polishedPacket[i-40] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 2; ++i)
            {
                polishedPacket[16 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[16 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            if (polishedPacket[0] == 1 && polishedPacket[1] == 1 && polishedPacket[2] == 1 && polishedPacket[3] == 1)
            {
                var outState = new ControllerStateBuilder();
                for (int i = 0; i < THREE_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(THREE_BUTTONS[i])) continue;
                    outState.SetButton(THREE_BUTTONS[i], polishedPacket[i] != 0x00);
                }

                outState.SetButton("left", polishedPacket[16] < 0x7f);
                outState.SetButton("right", polishedPacket[16] > 0x7f);
                outState.SetButton("up", polishedPacket[17] < 0x7f);
                outState.SetButton("down", polishedPacket[17] > 0x7f);
                return outState.Build();
            }
            else if (polishedPacket[0] == 0 && polishedPacket[1] == 0 && polishedPacket[2] == 0 && polishedPacket[3] == 0)
            {
                var outState = new ControllerStateBuilder();
                for (int i = 0; i < SIX_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(SIX_BUTTONS[i])) continue;
                    outState.SetButton(SIX_BUTTONS[i], polishedPacket[i] != 0x00);
                }

                outState.SetButton("left", polishedPacket[16] < 0x80);
                outState.SetButton("right", polishedPacket[16] > 0x80);
                outState.SetButton("up", polishedPacket[17] < 0x80);
                outState.SetButton("down", polishedPacket[17] > 0x80);
                return outState.Build();
            }

            return null;
        }
    }
}
