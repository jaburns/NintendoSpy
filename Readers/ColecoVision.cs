using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class ColecoVision
    {
        static bool firstRun = false;
        static int spinnerValue = 0;
        static bool spinnerValueChanged = false;

        const int PACKET_SIZE = 11;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "L", null, null, null, null, "R" 
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE ) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }
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
            
            state.SetButton("1", packet[5] == 0 && packet[6] == 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("2", packet[5] == 0 && packet[6] == 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("3", packet[5] != 0 && packet[6] == 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("4", packet[5] != 0 && packet[6] != 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("5", packet[5] == 0 && packet[6] != 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("6", packet[5] != 0 && packet[6] == 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("7", packet[5] == 0 && packet[6] == 0 && packet[7] != 0 && packet[8] != 0);
            state.SetButton("8", packet[5] == 0 && packet[6] != 0 && packet[7] != 0 && packet[8] != 0);
            state.SetButton("9", packet[5] == 0 && packet[6] != 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("star", packet[5] == 0 && packet[6] != 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("0", packet[5] != 0 && packet[6] != 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("pound", packet[5] != 0 && packet[6] == 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("purple", packet[5] != 0 && packet[6] != 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("blue", packet[5] != 0 && packet[6] == 0 && packet[7] != 0 && packet[8] != 0);

            if (firstRun == false)
            {
                spinnerValue = packet[10] - 11;
                firstRun = true;
            }

            if (spinnerValueChanged == false && spinnerValue != (packet[10] - 11))
            {
                spinnerValueChanged = true;
            }

            for (int i = 0; i < 64; ++i)
                state.SetButton("E" + i.ToString(), spinnerValueChanged && i == (packet[10] - 11));

            return state.Build();
        }
    }
}
