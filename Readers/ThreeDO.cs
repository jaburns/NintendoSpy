using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class ThreeDO
    {
        const int PACKET_SIZE = 16;
        const int MOUSE_PACKET_SIZE = 32;

        static readonly string[] BUTTONS = {
            null, null,"down", "up", "right", "left", "a", "b", "c","p", "x","r", "l", null, null, null
        };

        static readonly string[] MOUSE_BUTTONS = {
            null, null, null, null, null, null, null, "left", "middle", "right", null
        };

        static float readMouse(bool sign, byte data)
        {
            float val;
            if (sign)
                val = -1 * (0x7F - data);
            else
                val = data;

            return val / 127;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < 1) return null;

            var state = new ControllerStateBuilder();

            if (packet[0] == 0)
            {
                if (packet.Length < PACKET_SIZE) return null;

                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                    state.SetButton(BUTTONS[i], packet[i] != 0x00);
                }
            }
            else
            {
                if (packet.Length < MOUSE_PACKET_SIZE) return null;

                for (int i = 0; i < MOUSE_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(MOUSE_BUTTONS[i])) continue;
                    state.SetButton(MOUSE_BUTTONS[i], packet[i] != 0x00);
                }

                bool ySign = packet[11] != 0;
                byte yVal = SignalTool.readByte(packet, 14, 7);
                bool xSign = packet[21] != 0;
                byte xVal = SignalTool.readByte(packet, 24, 7);

                float x = readMouse(xSign, xVal);
                float y = readMouse(ySign, yVal);

                SignalTool.SetMouseProperties(x, y, state);
            }
            return state.Build ();
        }
    }
}
