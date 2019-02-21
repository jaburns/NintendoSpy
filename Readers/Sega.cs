using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Sega
    {
        const int PACKET_SIZE = 13;
        const int MOUSE_PACKET_SIZE = 24;

        static readonly string[] BUTTONS = {
            "ctrl", "up", "down", "left", "right", "b", "c", "a", "start", "z", "y", "x", "mode"
        };

        static readonly string[] MOUSE_BUTTONS = {
            "left", "right", "middle", "start"
        };

        static float readMouse(bool sign, bool over, byte data)
        {
            float val;
            if (over)
                val = 1.0f;
            else if (sign)
                val = 0xFF - data;
            else
                val = data;

            return val * (sign ? -1 : 1) / 255;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length != PACKET_SIZE && packet.Length != MOUSE_PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();
            if (packet.Length == PACKET_SIZE)
            {
                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                    state.SetButton(BUTTONS[i], packet[i] != 0x00);
                }
            }
            else if (packet.Length == MOUSE_PACKET_SIZE)
            {
                for (int i = 0; i < MOUSE_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(MOUSE_BUTTONS[i])) continue;
                    state.SetButton(MOUSE_BUTTONS[i], packet[i] != 0x00);
                }

                bool xSign = packet[4] != 0;
                bool ySign = packet[5] != 0;
                bool xOver = packet[6] != 0;
                bool yOver = packet[7] != 0;

                byte xVal = SignalTool.readByteBackwards(packet, 8);
                byte yVal = SignalTool.readByteBackwards(packet, 16);

                float x = readMouse(xSign, xOver, xVal);
                float y = readMouse(ySign, yOver, yVal);
                
                SignalTool.SetMouseProperties(x, y, state);

            }

            return state.Build ();
        }
    }
}
