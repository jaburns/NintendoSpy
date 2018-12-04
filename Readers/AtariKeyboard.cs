using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class AtariKeyboard
    {
        const int PACKET_SIZE = 1;

        static readonly string[] BUTTONS = {
            null, "1", "2", "3", "4", "5", "6", "7", "8", "9", "star", "0", "pound"
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();

            if (packet[0] != 0)
                state.SetButton(BUTTONS[packet[0]], true);

            return state.Build();
        }
    }
}
