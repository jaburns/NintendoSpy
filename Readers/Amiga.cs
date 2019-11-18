using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Amiga
    {
        static readonly string[] BUTTONS_CD32 =
        {
            null, "blue", "red", "yellow", "green", "forward", "backward", "pause", null
        };

        static readonly string[] BUTTONS_CDTV_REMOTE =
        {
            "mouse_left", "mouse_up", "mouse_right", "mouse_down", "right_button", "left_button", "Num1", "Num2", "Num3", "Num4", "Num5", "Num6", "Num7", "Num8", "Num9", "Num0", "Escape", "Enter", "Genlock", "CDTV", "Power", "Rew", "Play", "FF", "Stop", "VolumeUp", "VolumeDown", "left", "up", "right", "down", "2", "1", null, null
        };

        static readonly string[] BUTTONS_CDTV_JOYSTICK =
        {
            null, "2", "1", "right", "left", "down", "up", "Joy22", "Joy22", "Joy2Right", "Joy2Left", "Joy2Down", "Joy2Up", null, null, null, null, null, null, null, null, null, null, null, null, null
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            ControllerStateBuilder state = null;
            if (packet.Length == 13)
            {
                return Classic.ReadFromPacket(packet);
            }
            if (packet.Length == BUTTONS_CD32.Length)
            {
                state = new ControllerStateBuilder();

                for (int i = 0; i < BUTTONS_CD32.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_CD32[i])) continue;
                    state.SetButton(BUTTONS_CD32[i], (packet[i] & 0b10000000) == 0x00);
                }

                state.SetButton("up", (packet[8] & 0b00000001) == 0);
                state.SetButton("down", (packet[0] & 0b00000100) == 0);
                state.SetButton("left", (packet[0] & 0b00001000) == 0);
                state.SetButton("right", (packet[0] & 0b00010000) == 0);
            }
            else if (packet.Length == BUTTONS_CDTV_REMOTE.Length)
            {
                int checksum = (packet[33] >> 4) | packet[34];
                int checkedCheckSum = 0;
                for (int i = 0; i < 33; ++i)
                    checkedCheckSum += packet[i] == 0 ? 0 : 1;

                if (checksum == checkedCheckSum)
                {
                    state = new ControllerStateBuilder();

                    for (int i = 0; i < BUTTONS_CDTV_REMOTE.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(BUTTONS_CDTV_REMOTE[i])) continue;
                        state.SetButton(BUTTONS_CDTV_REMOTE[i], packet[i] != 0x00);
                    }

                    float x = 0;
                    float y = 0;

                    if (packet[0] != 0x00)
                        x = -0.25f;
                    else if (packet[2] != 0x00)
                        x = 0.25f;
                    if (packet[1] != 0x00)
                        y = 0.25f;
                    else if (packet[3] != 0x00)
                        y = -0.25f;

                    SignalTool.SetMouseProperties(x, y, state, .25f);
                }

            }
            else if (packet.Length == BUTTONS_CDTV_JOYSTICK.Length && packet[0] == 0)
            {
                int checksum = (packet[24] >> 4) | packet[25];
                int checkedCheckSum = 0;
                for (int i = 0; i < 24; ++i)
                    checkedCheckSum += packet[i] == 0 ? 0 : 1;

                if (checksum == checkedCheckSum)
                {
                    state = new ControllerStateBuilder();

                    for (int i = 0; i < BUTTONS_CDTV_JOYSTICK.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(BUTTONS_CDTV_JOYSTICK[i])) continue;
                        state.SetButton(BUTTONS_CDTV_JOYSTICK[i], packet[i] != 0x00);
                    }

                    SignalTool.FakeAnalogStick(packet[6], packet[5], packet[4], packet[3], state, "x", "y");
                    SignalTool.FakeAnalogStick(packet[12], packet[11], packet[10], packet[9], state, "Joy2x", "Joy2y");
                }
            }
            else if (packet.Length == 26 && packet[0] == 1)
            {
                int checksum = (packet[24] >> 4) | packet[25];
                int checkedCheckSum = 0;
                for (int i = 0; i < 24; ++i)
                    checkedCheckSum += packet[i] == 0 ? 0 : 1;

                if (checksum == checkedCheckSum)
                {
                    state = new ControllerStateBuilder();

                    state.SetButton("left_button", packet[2] == 0x00);
                    state.SetButton("right_button", packet[1] == 0x00);

                    sbyte xVal = (sbyte)SignalTool.readByte(packet, 3);
                    sbyte yVal = (sbyte)SignalTool.readByte(packet, 11);

                    SignalTool.SetMouseProperties(xVal / -128.0f, yVal / 128.0f, state);
                }
            }
            else if (packet.Length == 19)
            {
                state = new ControllerStateBuilder();

                state.SetButton("left_button", packet[0] != 0x00);
                state.SetButton("right_button", packet[2] != 0x00);

                sbyte xVal = (sbyte)SignalTool.readByteBackwards(packet, 3);
                sbyte yVal = (sbyte)SignalTool.readByteBackwards(packet, 11);

                SignalTool.SetMouseProperties(xVal / -128.0f, yVal / 128.0f, state);
            }
            else if (packet.Length == 36)
            {
                byte[] reconstructedPacket = new byte[18];

                int j = 0;
                for (int i = 0; i < 18; ++i)
                {
                    reconstructedPacket[i] = (byte)((packet[j] >> 4) | packet[j + 1]);
                    j += 2;
                }

                byte[] polishedPacket = new byte[128];

                int checksum = 0;
                for (int i = 0; i < 16; ++i)
                {
                    checksum += reconstructedPacket[i];
                    for (int k = 0; k < 8; ++k)
                    {
                        polishedPacket[(i * 8) + k] = (byte)((reconstructedPacket[i] & (1 << k)) != 0 ? 1 : 0);
                    }
                }

                short sentChecksum = (short)((reconstructedPacket[17] << 8) | reconstructedPacket[16]);
                if (checksum == sentChecksum)
                {
                    state = new ControllerStateBuilder();

                    for (int i = 0; i < 128; ++i)
                    {
                        string scanCode = i.ToString("X").ToUpper(); ;
                        state.SetButton(scanCode, polishedPacket[i] != 0x00);
                    }
                }

            }

            return state?.Build();
        }
    }
}
