using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class NeoGeoMini
    {
        const int PACKET_SIZE = 25;

        static readonly string[] BUTTONS = {
            "A", "B", null, "C", "D", null, null, null, null, null, "select", "start"
        };


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;
  
            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], packet[i] == 0x31);
            }

            int position = 0;
            for (byte j = 0; j < 4; ++j)
            {
                position |= (byte)((packet[16 + j] == 0x30 ? 0 : 1) << j);
            }

            float x = 0;
            float y = 0;

            switch (position)
            {
                case 0:
                    outState.SetButton("up", true);
                    y = -1;
                    outState.SetButton("down", false);
                    outState.SetButton("left", false);
                    outState.SetButton("right", false);
                    break;
                case 1:
                    outState.SetButton("up", true);
                    y = -1;
                    outState.SetButton("right", true);
                    x = 1;
                    outState.SetButton("down", false);
                    outState.SetButton("left", false);
                    break;
                case 2:
                    outState.SetButton("right", true);
                    x = 1;
                    outState.SetButton("down", false);
                    outState.SetButton("left", false);
                    outState.SetButton("up", false);
                    break;
                case 3:
                    outState.SetButton("right", true);
                    x = 1;
                    outState.SetButton("down", true);
                    y = 1;
                    outState.SetButton("up", false);
                    outState.SetButton("left", false);
                    break;
                case 4:
                    outState.SetButton("down", true);
                    y = 1;
                    outState.SetButton("up", false);
                    outState.SetButton("left", false);
                    outState.SetButton("right", false);
                    break;
                case 5:
                    outState.SetButton("left", true);
                    x = -1;
                    outState.SetButton("down", true);
                    y = 1;
                    outState.SetButton("right", false);
                    outState.SetButton("up", false);
                    break;
                case 6:
                    outState.SetButton("right", false);
                    outState.SetButton("down", false);
                    outState.SetButton("up", false);
                    outState.SetButton("left", true);
                    x = -1;
                    break;
                case 7:
                    outState.SetButton("up", true);
                    y = -1;
                    outState.SetButton("left", true);
                    x = -1;
                    outState.SetButton("right", false);
                    outState.SetButton("down", false);
                    break;
                default:
                    outState.SetButton("up", false);
                    outState.SetButton("left", false);
                    outState.SetButton("right", false);
                    outState.SetButton("down", false);
                    break;
            }

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

            outState.SetAnalog("lstick_x", x);
            outState.SetAnalog("lstick_y", y);

            return outState.Build();
            
        }
    }
}
