using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Paddles
    {
        static byte lastLeft = 0;
        static byte lastRight = 0;

        const int PACKET_SIZE = 6;

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
            if (packet[4] != 5) return null;
            if (packet[2] == 0 || packet[3] == 0) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            byte left = packet[2];
            byte right = packet[3];

            if ((Math.Abs(left - lastLeft)/256.0)*100.0 > (packet[5] - 11))
                lastLeft = left;
            else
                left = lastLeft;

            if ((Math.Abs(right - lastRight) / 256.0) * 100.0 > (packet[5] - 11))
                lastRight = right;
            else
                right = lastRight;

            state.SetAnalog("left", readPaddle(left));
            state.SetAnalog("right", readPaddle(right));

            return state.Build();
        }
    }
}
