using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Paddles
    {

        const int PACKET_SIZE = 6;

        static float readPaddle(ushort input)
        {
            return (float)(input) / 256;
        }

        static bool SecondButton;
        static float SecondPaddle;

        static public ControllerState ReadFromPacket(byte[] packet)
        {

            if (packet.Length < PACKET_SIZE) return null;
            if (packet[4] != 5) return null;

            var state = new ControllerStateBuilder();

            state.SetAnalog("paddle", readPaddle(packet[2]));
            state.SetAnalog("paddle2", SecondPaddle);
            state.SetButton("fire2", SecondButton);
            state.SetButton("fire", (packet[1] != 0x00));

            return state.Build();
        }

        static public ControllerState ReadFromSecondPacket(byte[] packet)
        {

            if (packet.Length < PACKET_SIZE) return null;
            if (packet[4] != 5) return null;

            SecondButton = (packet[1] != 0x00);
            SecondPaddle = readPaddle(packet[2]);

            return null;
        }
    }
}
