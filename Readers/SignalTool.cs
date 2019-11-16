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

        static float middleOfThree(float a, float b, float c)
        {
            // Compare each three number to find middle  
            // number. Enter only if a > b 
            if (a > b)
            {
                if (b > c)
                    return b;
                else if (a > c)
                    return c;
                else
                    return a;
            }
            else
            {
                // Decided a is not greater than b. 
                if (a > c)
                    return a;
                else if (b > c)
                    return c;
                else
                    return b;
            }
        }


        static float[] windowX = new float[3];
        static int windowPositionX = 0;
        static float[] windowY = new float[3];
        static int windowPositionY = 0;
        public static void SetMouseProperties(float x, float y, ControllerStateBuilder state, float maxCircleSize = 1.0f)
        {
            windowX[windowPositionX] = x;
            windowPositionX += 1;
            windowPositionX = (windowPositionX % 3);

            windowY[windowPositionY] = y;
            windowPositionY += 1;
            windowPositionY = (windowPositionY % 3);

            y = middleOfThree(windowY[0], windowY[1], windowY[2]);
            x = middleOfThree(windowX[0], windowX[1], windowX[2]);

            float y1 = y;
            float x1 = x;

            if (y != 0 || x != 0)
            {
                // Direction shows around the unit circle
                double radian = Math.Atan2(y, x);
                x1 = maxCircleSize*(float)Math.Cos(radian);
                y1 = maxCircleSize*(float)Math.Sin(radian);

                // Don't let magnitude exceed the unit circle
                if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > maxCircleSize)
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

        public static void FakeAnalogStick(byte up, byte down, byte left, byte right, ControllerStateBuilder state, string xName, string yName)
        {

            float x = 0;
            float y = 0;

            if (right != 0x00)
                x = 1;
            else if (left != 0x00)
                x = -1;

            if (up != 0x00)
                y = 1;
            else if (down != 0x00)
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

            state.SetAnalog(xName, x);
            state.SetAnalog(yName, y);
        }
    }
}
