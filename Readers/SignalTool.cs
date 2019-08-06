using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static internal class SignalTool
    {
        /// <summary>
        /// Reads a byte of data from a string of 8 bits in a controller data packet.
        /// </summary>
        public static byte readByte (byte[] packet, int offset, byte numBits = 8, byte mask = 0x0F)
        {
            byte val = 0;
            for (int i = 0 ; i < numBits; ++i) {
                if ((packet[i+offset] & mask) != 0) {
                    val |= (byte)(1<<((numBits-1)-i));
                }
            }
            return val;
        }

        public static byte readByteBackwards(byte[] packet, int offset, byte numBits = 8, byte mask = 0x0F)
        {
            byte val = 0;
            for (int i = 0; i < numBits; ++i)
            {
                if ((packet[i + offset] & mask) != 0)
                {
                    val |= (byte)(1 << i);
                }
            }
            return val;
        }

        public static void SetMouseProperties(float x, float y, ControllerStateBuilder state)
        {
            float y1 = y;
            float x1 = x;

            if (y != 0 || x != 0)
            {
                // Direction shows around the unit circle
                double radian = Math.Atan2(y, x);
                x1 = (float)Math.Cos(radian);
                y1 = (float)Math.Sin(radian);

                // Don't let magnitude exceed the unit circle
                if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > 1.0)
                {
                    x = x1;
                    y = y1;
                }
            }

            state.SetAnalog("mouse_center_x", 0);
            state.SetAnalog("mouse_center_y", 0);
            state.SetAnalog("mouse_direction_x", x1);
            state.SetAnalog("mouse_direction_y", y1);
            state.SetAnalog("mouse_magnitude_x", x);
            state.SetAnalog("mouse_magnitude_y", y);
        }
    }
}
