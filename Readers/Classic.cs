using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Classic
    {
        const int PACKET_SIZE = 6;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "1", "2"
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);

                if (packet[3] != 0x00)
                    state.SetAnalog("x", 1);
                else if (packet[2] != 0x00)
                    state.SetAnalog("x", -1);

                if (packet[0] != 0x00)
                    state.SetAnalog("y", 1);
                else if (packet[1] != 0x00)
                    state.SetAnalog("y", -1);
            }

            return state.Build();
        }
    }
}
