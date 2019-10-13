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

        static readonly string[] BUTTONS = {
            null, "fire"
        };

        static float readPaddle(ushort input)
        {
            return (float)(input)/256;
        }
        
        static public ControllerState ReadFromPacket(byte[] packet)
        {
           
            if (packet.Length < PACKET_SIZE) return null;
            if (packet[4] != 5) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            state.SetAnalog("paddle", readPaddle(packet[2]));
           
            return state.Build();
        }
    }
}
