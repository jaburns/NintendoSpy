using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Playstation2
    {
        const int PACKET_SIZE = 152;
        const int POLISHED_PACKET_SIZE = 33;

        static readonly string[] BUTTONS = {
            null, "select", "lstick", "rstick", "start", "up", "right", "down", "left", "l2", "r2", "l1", "r1", "triangle", "circle", "x", "square"
        };

        static float readAnalogButton(byte input)
        {
            return (float) (input) / 256;
        }
        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static float readMouse(bool over, byte data)
        {
            float val = 0;
            if (over)
            {
                if (data >= 128)
                    val = -1.0f;
                else
                    val = 1.0f;
            }
            else
            {
                if (data >= 128)
                    val = (-1.0f * (255 - data)) / 127.0f;
                else
                    val = data / 127.0f;
            }

            return val;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            polishedPacket[0] = 0;
            for (byte i = 0; i < 8; ++i)
            {
                polishedPacket[0] |= (byte)((packet[i] == 0 ? 0 : 1) << i);
            }

            for (byte i = 0; i < 16; ++i)
            {
                polishedPacket[i + 1] = packet[i + 8];
            }

            int nextNumBytes = 0;
            if (polishedPacket[0] == 0x73 || polishedPacket[0] == 0x79)
                nextNumBytes = 4;
            else if (polishedPacket[0] == 0x12)
                nextNumBytes = 2;

            for (int i = 0; i < nextNumBytes; ++i)
            {
                polishedPacket[17 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[17 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0 ? 0 : 1) << j);
                }
            }

            if (polishedPacket[0] == 0x79)
            {
                for (int i = 0; i < 12; ++i)
                {
                    polishedPacket[21 + i] = 0;
                    for (byte j = 0; j < 8; ++j)
                    {
                        polishedPacket[21 + i] |= (byte)((packet[56 + (i * 8 + j)] == 0 ? 0 : 1) << j);
                    }
                }
            }

            if (polishedPacket.Length < POLISHED_PACKET_SIZE) return null;

            if (polishedPacket[0] != 0x41 && polishedPacket[0] != 0x73 && polishedPacket[0] != 0x79 && polishedPacket[0] != 0x12) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            if (polishedPacket[0] == 0x73 || polishedPacket[0] == 0x79)
            {
                state.SetAnalog("rstick_x", readStick(polishedPacket[17]));
                state.SetAnalog("rstick_y", readStick(polishedPacket[18]));
                state.SetAnalog("lstick_x", readStick(polishedPacket[19]));
                state.SetAnalog("lstick_y", readStick(polishedPacket[20]));
            }
            else
            {
                state.SetAnalog("rstick_x", 0);
                state.SetAnalog("rstick_y", 0);
                state.SetAnalog("lstick_x", 0);
                state.SetAnalog("lstick_y", 0);
            }

            if (polishedPacket[0] == 0x79)
            {
                state.SetAnalog("analog_right", readAnalogButton(polishedPacket[21]));
                state.SetAnalog("analog_left", readAnalogButton(polishedPacket[22]));
                state.SetAnalog("analog_up", readAnalogButton(polishedPacket[23]));
                state.SetAnalog("analog_down", readAnalogButton(polishedPacket[24]));

                state.SetAnalog("analog_triangle", readAnalogButton(polishedPacket[25]));
                state.SetAnalog("analog_circle", readAnalogButton(polishedPacket[26]));
                state.SetAnalog("analog_x", readAnalogButton(polishedPacket[27]));
                state.SetAnalog("analog_square", readAnalogButton(polishedPacket[28]));

                state.SetAnalog("analog_l1", readAnalogButton(polishedPacket[29]));
                state.SetAnalog("analog_r1", readAnalogButton(polishedPacket[30]));
                state.SetAnalog("analog_l2", readAnalogButton(polishedPacket[31]));
                state.SetAnalog("analog_r2", readAnalogButton(polishedPacket[32]));
            }
            else
            {
                state.SetAnalog("analog_right", (float)(polishedPacket[6] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_left", (float)(polishedPacket[8] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_up", (float)(polishedPacket[5] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_down", (float)(polishedPacket[7] != 0x00 ? 1.0 : 0.0));

                state.SetAnalog("analog_triangle", (float)(polishedPacket[13] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_circle", (float)(polishedPacket[14] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_x", (float)(polishedPacket[15] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_square", (float)(polishedPacket[16] != 0x00 ? 1.0 : 0.0));

                state.SetAnalog("analog_l1", (float)(polishedPacket[11] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_r1", (float)(polishedPacket[12] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_l2", (float)(polishedPacket[9] != 0x00 ? 1.0 : 0.0));
                state.SetAnalog("analog_r2", (float)(polishedPacket[10] != 0x00 ? 1.0 : 0.0));
            }

            if (polishedPacket[0] == 0x12)
            {
                float x = readMouse(polishedPacket[10] == 0x00, polishedPacket[17]);
                float y = readMouse(polishedPacket[9] == 0x00, polishedPacket[18]);
                SignalTool.SetMouseProperties(x, y, state);
            }
            else
            {
                SignalTool.SetMouseProperties(0, 0, state);
            }

            return state.Build();
        }
    }
}
