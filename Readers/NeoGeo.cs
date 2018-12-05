using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class NeoGeo
    {
        const int PACKET_SIZE = 10;

        static readonly string[] BUTTONS = {
            "select", "D", "B", "right", "down", "start", "C", "A", "left", "up"
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
                    state.SetAnalog("lstick_x", 1);
                else if (packet[8] != 0x00)
                    state.SetAnalog("lstick_x", -1);
                else
                    state.SetAnalog("lstick_x", 0);

                if (packet[4] != 0x00)
                    state.SetAnalog("lstick_y", 1);
                else if (packet[9] != 0x00)
                    state.SetAnalog("lstick_y", -1);
                else
                    state.SetAnalog("lstick_y", 0);
            }

            return state.Build();
        }
    }
}
