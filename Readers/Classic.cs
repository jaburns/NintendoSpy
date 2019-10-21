using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Classic
    {
        const int PACKET_SIZE = 7;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "1", "2", "3"
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
             if (packet.Length < PACKET_SIZE) return null;
            
            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);

                float x = 0;
                float y = 0;

                if (packet[3] != 0x00)
                    x = 1;
                else if (packet[2] != 0x00)
                    x = -1;

                if (packet[0] != 0x00)
                    y = 1;
                else if (packet[1] != 0x00)
                    y = -1;

                if (y != 0 || x != 0)
                {
                    // point on the unit circle at the same angle
                    double radian = Math.Atan2(y, x);
                    float x1 = (float)Math.Cos(radian);
                    float y1 = (float)Math.Sin(radian);

                    // Don't let magnitude exceed the unit circle
                    if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > 1.0)
                    {
                        x = x1;
                        y = y1;
                    }
                }

                state.SetAnalog("x", x);
                state.SetAnalog("y", y);
            }

            return state.Build();
        }
    }
}
