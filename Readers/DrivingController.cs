using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
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

            if (packet[0] != 0)
                state.SetButton(BUTTONS[0], true);

            state.SetButton(BUTTONS[packet[1] + 1], true);

            return state.Build();
        }
    }
}
