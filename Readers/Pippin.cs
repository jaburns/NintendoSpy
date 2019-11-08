using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    class Pippin
    {
        const int PACKET_SIZE = 30;
        const int POLISHED_PACKET_SIZE = 16;

        static readonly string[] BUTTONS = {
            null, "1", "2", "blue", "yellow", "up", "left", "right", "down", "red", "green", "square", "circle", "diamond"
        };

        static float readMouse(byte data)
        {
            if (data >= 64)
                return (-1.0f * (127 - data)) / 63.0f;
            else
                return data / 63.0f;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length != PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            polishedPacket[0] = (byte)((packet[0] >> 4) | packet[1]);
            polishedPacket[1] = (byte)(packet[9] == 0 ? 0 : 1);
            polishedPacket[2] = (byte)(packet[17] == 0 ? 0 : 1);

            for (int i = 18; i < 29; ++i)
                polishedPacket[i - 15] = (byte)(packet[i] == 0 ? 0 : 1);

            for (byte i = 0; i < 7; ++i)
            {
                polishedPacket[14] |= (byte)((packet[i+2] == 0 ? 0 : 1) << i);
            }

            for (byte i = 0; i < 7; ++i)
            {
                polishedPacket[15] |= (byte)((packet[i + 10] == 0 ? 0 : 1) << i);
            }

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], polishedPacket[i] == 0x00);
            }


            float x = 0; 
            float y = 0;
            if (packet[29] == 1)
            {
                y = readMouse(polishedPacket[14]);
                x = readMouse(polishedPacket[15]);
            }
            SignalTool.SetMouseProperties(x, y, state);

            return state.Build();
        }

    }

}
