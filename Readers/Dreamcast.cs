using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Dreamcast
    {
        const int FRAME_HEADER_SIZE = 32;

        static readonly string[] BUTTONS = {
            "right2", "left2", "down2", "up2", "d", "x", "y", "z", "right", "left", "down", "up", "start", "a", "b", "c"    
        };

        static readonly string[] MOUSE_BUTTONS = {
            null, null, null, null, "start", "left", "right", "middle"
        };

        static float readTrigger(byte input)
        {
            return (float)(input) / 256;
        }
        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < FRAME_HEADER_SIZE) return null;

            var state = new ControllerStateBuilder();

            int j = 0;

            byte numWords = 0;
            for (int i = 0; i < 4; ++i)
            {
                numWords |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                numWords |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                j += 2;
            }

            // Skip sender and receiver address
            j += 16;

            byte dcCommand = 0;
            for (int i = 0; i < 4; ++i)
            {
                dcCommand |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                dcCommand |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                j += 2;
            }

            if (dcCommand == 8 && numWords >= 1)
            {
                uint controllerType = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        controllerType |= (uint)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (k * 2) + (i * 8)));
                        controllerType |= (uint)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (k * 2) + (i * 8)));
                        j += 2;
                    }
                }

                j += 16;

                if (controllerType == 1 && numWords == 3)
                {

                    byte ltrigger = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        ltrigger |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        ltrigger |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    byte rtrigger = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        rtrigger |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        rtrigger |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    int k = 0;
                    for (int i = 0; i < BUTTONS.Length/2; ++i)
                    {
                        if (!string.IsNullOrEmpty(BUTTONS[k]))
                            state.SetButton(BUTTONS[k], (packet[j] & 0x2) == 0x0);
                        if (!string.IsNullOrEmpty(BUTTONS[k+1]))
                            state.SetButton(BUTTONS[k+1], (packet[j+1] & 0x1) == 0x0);

                        k += 2;
                        j += 2;
                    }

                    byte joyy2 = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        joyy2 |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        joyy2 |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    byte joyx2 = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        joyx2 |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        joyx2 |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    byte joyy = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        joyy |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        joyy |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    byte joyx = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        joyx |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                        joyx |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
                        j += 2;
                    }

                    state.SetAnalog("stick_x", readStick(joyx));
                    state.SetAnalog("stick_y", readStick(joyy));
                    state.SetAnalog("stick_x2", readStick(joyx2));
                    state.SetAnalog("stick_y2", readStick(joyy2));
                    state.SetAnalog("trig_r", readTrigger(rtrigger));
                    state.SetAnalog("trig_l", readTrigger(ltrigger));


                }
                else if (controllerType == 0x200 && numWords == 6)
                {
                    j += 24;

                    int k = 0;
                    for (int i = 0; i < MOUSE_BUTTONS.Length / 2; ++i)
                    {
                        if (!string.IsNullOrEmpty(MOUSE_BUTTONS[k]))
                            state.SetButton(MOUSE_BUTTONS[k], (packet[j] & 0x2) == 0x0);
                        if (!string.IsNullOrEmpty(MOUSE_BUTTONS[k + 1]))
                            state.SetButton(MOUSE_BUTTONS[k + 1], (packet[j + 1] & 0x1) == 0x0);

                        k += 2;
                        j += 2;
                    }

                    ushort axis1 = 0;
                    for (int i = 1; i >= 0; --i)
                    {
                        for (k = 0; k < 4; ++k)
                        {
                            axis1 |= (ushort)(((packet[j] & 0x02) != 0 ? 1 : 0) << ((7 - k * 2) + (i * 8)));
                            axis1 |= (ushort)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (k * 2) + (i * 8)));
                            j += 2;
                        }
                    }

                    ushort axis2 = 0;
                    for (int i = 1; i >= 0; --i)
                    {
                        for (k = 0; k < 4; ++k)
                        {
                            axis2 |= (ushort)(((packet[j] & 0x02) != 0 ? 1 : 0) << ((7 - k * 2) + (i * 8)));
                            axis2 |= (ushort)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (k * 2) + (i * 8)));
                            j += 2;
                        }
                    }

                    j += 16;

                    ushort axis3 = 0;
                    for (int i = 1; i >= 0; --i)
                    {
                        for (k = 0; k < 4; ++k)
                        {
                            axis3 |= (ushort)(((packet[j] & 0x02) != 0 ? 1 : 0) << ((7 - k * 2) + (i * 8)));
                            axis3 |= (ushort)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (k * 2) + (i * 8)));
                            j += 2;
                        }
                    }

                    float x = (axis2 - 512) / 512.0f;
                    float y = -1 * (axis1 - 512) / 512.0f;
                    
                    SignalTool.SetMouseProperties(x, y, state);

                    state.SetButton("scroll_up", axis3 < 512);
                    state.SetButton("scroll_down", axis3 > 512);

                }
                return state.Build();
            }

            return null;
        }

    }
}
