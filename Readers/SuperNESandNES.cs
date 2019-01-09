using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class SuperNESandNES
    {
        static ControllerState readPacketButtons(byte[] packet, string[] buttons)
        {
            if (packet.Length < buttons.Length) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < buttons.Length; ++i) {
                if (string.IsNullOrEmpty(buttons[i])) continue;
                state.SetButton(buttons[i], packet[i] != 0x00);
            }

            return state.Build();
        }

        static readonly string[] BUTTONS_NES = {
            "a", "b", "select", "start", "up", "down", "left", "right"
        };

        static readonly string[] BUTTONS_SNES = {
            "b", "y", "select", "start", "up", "down", "left", "right", "a", "x", "l", "r", null, null, null, null
        };

        static readonly string[] BUTTONS_INTELLIVISION = {
            "n", "nne", "ne", "ene", "e", "ese", "se", "sse", "s", "ssw", "sw", "wsw", "w", "wnw", "nw", "nnw", "1", "2", "3", "4", "5", "6", "7", "8", "9", "clear", "0", "enter", "topleft", "topright", "bottomleft", "bottomright"
        };

        static readonly string[] BUTTONS_CD32 =
        {
            "blue", "red", "yellow", "green", "forward", "backward", "pause", "up", "down", "left", "right"
        };

        static public ControllerState ReadFromPacket_Intellivision(byte[] packet)
        {
            return readPacketButtons(packet, BUTTONS_INTELLIVISION);
        }

        static public ControllerState ReadFromPacket_NES (byte[] packet) {
            return readPacketButtons (packet, BUTTONS_NES);
        }

        static public ControllerState ReadFromPacket_CD32(byte[] packet)
        {
            return readPacketButtons(packet, BUTTONS_CD32);
        }

        static public ControllerState ReadFromPacket_SNES (byte[] packet) {
            var controllerState = readPacketButtons (packet, BUTTONS_SNES);
            if (controllerState != null && packet[15] != 0x00)
            {
                // We have extended data do something with it
            }

            return controllerState;
        }
    }
}
