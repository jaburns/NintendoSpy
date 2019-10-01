using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class SwitchReader
    {
        const int PRO_PACKET_SIZE = 57;
        const int POKKEN_PACKET_SIZE = 58;
        const int POLISHED_PACKET_SIZE = 28;

        static readonly string[] PRO_BUTTONS = {
            "y", "x", "b", "a", null, null, "r", "zr", "-", "+", "rs", "ls", "home", "capture", null, null, "down", "up", "right", "left", null, null, "l", "zl"
        };

        static readonly string[] POKKEN_BUTTONS = {
            "y", "b", "a", "x", "l", "r", "zl", "zr", "-", "+", null, null, "home", "capture"
        };


        static float readStick(byte input)
        {
            if (input < 127)
                return (float)input / 128;

            return (float)(255-input) / -128;
        }

        static float readPokkenStick(byte input, bool invert)
        {
            return invert ? -1.0f*((float)(input - 128) / 128) : (float)(input - 128) / 128;
        }


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PRO_PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            if (packet.Length == PRO_PACKET_SIZE)
            {
                for (int i = 0; i < 24; ++i)
                    polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

                for (int i = 0; i < 4; ++i)
                {
                    polishedPacket[24 + i] = 0;
                    for (byte j = 0; j < 8; ++j)
                    {
                        polishedPacket[24 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                    }
                }

                var outState = new ControllerStateBuilder();

                for (int i = 0; i < PRO_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(PRO_BUTTONS[i])) continue;
                    outState.SetButton(PRO_BUTTONS[i], polishedPacket[i] != 0x00);
                }

                outState.SetAnalog("rstick_x", readStick(polishedPacket[26]));
                outState.SetAnalog("rstick_y", readStick(polishedPacket[27]));
                outState.SetAnalog("lstick_x", readStick(polishedPacket[24]));
                outState.SetAnalog("lstick_y", readStick(polishedPacket[25]));

                return outState.Build();
            }
            else if (packet.Length == POKKEN_PACKET_SIZE)
            {
                for (int i = 0; i < 16; ++i)
                    polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

                polishedPacket[16] = 0;
                for (byte j = 0; j < 4; ++j)
                {
                    polishedPacket[16] |= (byte)((packet[16 + j] == 0x30 ? 0 : 1) << j);
                }

                for (int i = 0; i < 4; ++i)
                {
                    polishedPacket[17 + i] = 0;
                    for (byte j = 0; j < 8; ++j)
                    {
                        polishedPacket[17 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                    }
                }
                
                var outState = new ControllerStateBuilder();

                for (int i = 0; i < POKKEN_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(POKKEN_BUTTONS[i])) continue;
                    outState.SetButton(POKKEN_BUTTONS[i], polishedPacket[i] != 0x00);
                }

                
                switch(polishedPacket[16])
                {
                    case 0:
                        outState.SetButton("up", true);
                        outState.SetButton("down", false);
                        outState.SetButton("left", false);
                        outState.SetButton("right", false);
                        break;
                    case 1:
                        outState.SetButton("up", true);
                        outState.SetButton("right", true);
                        outState.SetButton("down", false);
                        outState.SetButton("left", false);
                        break;
                    case 2:
                        outState.SetButton("right", true);
                        outState.SetButton("down", false);
                        outState.SetButton("left", false);
                        outState.SetButton("up", false);
                        break;
                    case 3:
                        outState.SetButton("right", true);
                        outState.SetButton("down", true);
                        outState.SetButton("up", false);
                        outState.SetButton("left", false);
                        break;
                    case 4:
                        outState.SetButton("down", true);
                        outState.SetButton("up", false);
                        outState.SetButton("left", false);
                        outState.SetButton("right", false);
                        break;
                    case 5:
                        outState.SetButton("left", true);
                        outState.SetButton("down", true);
                        outState.SetButton("right", false);
                        outState.SetButton("up", false);
                        break;
                    case 6:
                        outState.SetButton("right", false);
                        outState.SetButton("down", false);
                        outState.SetButton("up", false);
                        outState.SetButton("left", true);
                        break;
                    case 7:
                        outState.SetButton("up", true);
                        outState.SetButton("left", true);
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

                outState.SetAnalog("lstick_x", readPokkenStick(polishedPacket[17], false));
                outState.SetAnalog("lstick_y", readPokkenStick(polishedPacket[18], true));
                outState.SetAnalog("rstick_x", readPokkenStick(polishedPacket[19], false));
                outState.SetAnalog("rstick_y", readPokkenStick(polishedPacket[20], true));
                
                return outState.Build();
            }

            return null;
        }
    }
}
