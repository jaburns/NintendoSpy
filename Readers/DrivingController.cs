using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class DrivingController
    {
        const int PACKET_SIZE = 2;

        static readonly string[] BUTTONS = {
            "fire", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();

            state.SetButton(BUTTONS[0], packet[0] != 0);

            for (int i = 1; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], (i-1+65) == packet[1]);
            }
          
            
            return state.Build();
        }
    }
}
