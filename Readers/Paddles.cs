using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Paddles
    {

        const int PACKET_SIZE = 4;

        static readonly string[] BUTTONS = {
            "2", "1"
        };

        static float readPaddle(ushort input)
        {
            return (float)(input)/256;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
           
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            state.SetAnalog("left", readPaddle(packet[2]));
            state.SetAnalog("right", readPaddle(packet[3]));

            return state.Build();
        }
    }
}
