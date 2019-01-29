using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Dreamcast
    {
        const int FRAME_HEADER_SIZE = 32;

        static readonly string[] BUTTONS = {
            null, null, null, null, null, "x", "y", null, "right", "left", "down", "up", "start", "a", "b", null
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

            byte numFrames = 0;
            for (int i = 0; i < 4; ++i)
            {
                numFrames |= (byte)(((packet[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
                numFrames |= (byte)(((packet[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
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

            if (dcCommand == 8 && numFrames >= 1)
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

                if (controllerType == 1 && numFrames == 3)
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

                    j += 16;

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
                    state.SetAnalog("trig_r", readTrigger(rtrigger));
                    state.SetAnalog("trig_l", readTrigger(ltrigger));

                    return state.Build();
                }

            }

            return null;
        }

    }
}
