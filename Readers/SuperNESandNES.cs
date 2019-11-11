using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class SuperNESandNES
    {
        static ControllerState readPacketButtons(byte[] packet, string[] buttons)
        {
            if (packet.Length < buttons.Length) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < buttons.Length; ++i)
            {
                if (string.IsNullOrEmpty(buttons[i])) continue;
                state.SetButton(buttons[i], packet[i] != 0x00);
            }

            return state.Build();
        }


        static ControllerState readPacketButtons_ascii(byte[] packet, string[] buttons)
        {
            if (packet.Length < buttons.Length) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < buttons.Length; ++i)
            {
                if (string.IsNullOrEmpty(buttons[i])) continue;
                state.SetButton(buttons[i], packet[i] != '0');
            }

            return state.Build();
        }
        static readonly string[] BUTTONS_NES = {
            "a", "b", "select", "start", "up", "down", "left", "right"
        };

        static readonly string[] BUTTONS_SNES = {
            "b", "y", "select", "start", "up", "down", "left", "right", "a", "x", "l", "r", null, null, null, null
        };

        static readonly string[] BUTTONS_INTELLIVISION = {
            "n", "nne", "ne", "ene", "e", "ese", "se", "sse", "s", "ssw", "sw", "wsw", "w", "wnw", "nw", "nnw", "1", "2", "3", "4", "5", "6", "7", "8", "9", "clear", "0", "enter", "topleft", "topright", "bottomleft", "bottomright"
        };

        static readonly string[] BUTTONS_CD32 =
        {
            null, "blue", "red", "yellow", "green", "forward", "backward", "pause", null
        };

        static readonly string[] BUTTONS_PSCLASSIC =
        {
            "r1", "l1", "r2", "l2", "square", "x", "circle", "triangle", null, null, "down", "up", "right", "left", "start", "select"
        };

        static readonly string[] BUTTONS_ATARI5200_1 =
        {
            "start", "pause", "reset", "1", "2", "3", "4", "5", "6", "7", "8", "9", "star", "0", "pound", null, null
        };

        static readonly string[] BUTTONS_ATARI5200_2 =
        {
            "trigger", "fire", null, null
        };

        static public ControllerState ReadFromPacket_Intellivision(byte[] packet)
        {
            return readPacketButtons(packet, BUTTONS_INTELLIVISION);
        }

        static public ControllerState ReadFromPacket_NES(byte[] packet)
        {
            return readPacketButtons(packet, BUTTONS_NES);
        }

        static public ControllerState ReadFromPacket_PSClassic(byte[] packet)
        {
            return readPacketButtons_ascii(packet, BUTTONS_PSCLASSIC);
        }

        static public ControllerState ReadFromPacket_CD32(byte[] packet)
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
            else if (packet.Length == 19)
            {
                state = new ControllerStateBuilder();

                state.SetButton("left", packet[0] != 0x00);
                state.SetButton("right", packet[2] != 0x00);

                sbyte xVal = (sbyte)SignalTool.readByteBackwards(packet, 3);
                sbyte yVal = (sbyte)SignalTool.readByteBackwards(packet, 11);

                SignalTool.SetMouseProperties(xVal / -128.0f, yVal / 128.0f, state);
            }

            return state != null ? state.Build() : null;
        }

        static public ControllerState ReadFromPacket_SNES(byte[] packet)
        {
            if (packet.Length < BUTTONS_SNES.Length) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS_SNES.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS_SNES[i])) continue;
                state.SetButton(BUTTONS_SNES[i], packet[i] != 0x00);
            }

            if (state != null && packet.Length == 32 && packet[15] != 0x00)
            {
                float y = (float)(SignalTool.readByte(packet, 17, 7, 0x1) * ((packet[16] & 0x1) != 0 ? 1 : -1)) / 127;
                float x = (float)(SignalTool.readByte(packet, 25, 7, 0x1) * ((packet[24] & 0x1) != 0 ? -1 : 1)) / 127;
                SignalTool.SetMouseProperties(x, y, state);

            }

            return state.Build();
        }

        static public ControllerState ReadFromPacket_Jaguar(byte[] packet)
        {
            if (packet.Length < 4) return null;

            var state = new ControllerStateBuilder();

            state.SetButton("pause", (packet[0] & 0b00000100) == 0x00);
            state.SetButton("a", (packet[0] & 0b00001000) == 0x00);
            state.SetButton("right", (packet[0] & 0b00010000) == 0x00);
            state.SetButton("left", (packet[0] & 0b00100000) == 0x00);
            state.SetButton("down", (packet[0] & 0b01000000) == 0x00);
            state.SetButton("up", (packet[0] & 0b10000000) == 0x00);

            state.SetButton("b", (packet[1] & 0b00001000) == 0x00);
            state.SetButton("1", (packet[1] & 0b00010000) == 0x00);
            state.SetButton("4", (packet[1] & 0b00100000) == 0x00);
            state.SetButton("l", (packet[1] & 0b00100000) == 0x00);
            state.SetButton("7", (packet[1] & 0b01000000) == 0x00);
            state.SetButton("x", (packet[1] & 0b01000000) == 0x00);
            state.SetButton("star", (packet[1] & 0b10000000) == 0x00);

            state.SetButton("c", (packet[2] & 0b00001000) == 0x00);
            state.SetButton("2", (packet[2] & 0b00010000) == 0x00);
            state.SetButton("5", (packet[2] & 0b00100000) == 0x00);
            state.SetButton("8", (packet[2] & 0b01000000) == 0x00);
            state.SetButton("y", (packet[2] & 0b01000000) == 0x00);
            state.SetButton("0", (packet[2] & 0b10000000) == 0x00);

            state.SetButton("option", (packet[3] & 0b00001000) == 0x00);
            state.SetButton("3", (packet[3] & 0b00010000) == 0x00);
            state.SetButton("6", (packet[3] & 0b00100000) == 0x00);
            state.SetButton("r", (packet[3] & 0b00100000) == 0x00);
            state.SetButton("9", (packet[3] & 0b01000000) == 0x00);
            state.SetButton("z", (packet[3] & 0b01000000) == 0x00);
            state.SetButton("pound", (packet[3] & 0b10000000) == 0x00);

            return state.Build();

        }

        static float atari5200_y;
        static bool atari5200_trigger;
        static bool atari5200_fire;

        static public ControllerState ReadFromPacket_Atari5200_2(byte[] packet)
        {
            if (packet.Length != BUTTONS_ATARI5200_2.Length) return null;

            atari5200_trigger = packet[0] == 0x00;
            atari5200_fire = packet[1] == 0x00;
            atari5200_y = (((packet[2] >> 4) | (packet[3])) - 128.0f) / 128.0f;

            return null;
        }

        static public ControllerState ReadFromPacket_Atari5200_1(byte[] packet)
        {
            if (packet.Length != BUTTONS_ATARI5200_1.Length) return null;

            var state = new ControllerStateBuilder();
            for (int i = 0; i < BUTTONS_ATARI5200_1.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS_ATARI5200_1[i])) continue;
                state.SetButton(BUTTONS_ATARI5200_1[i], packet[i] == 0x00);
            }

            state.SetButton("trigger", atari5200_trigger);
            state.SetButton("fire", atari5200_fire);

            state.SetAnalog("x", (((packet[15] >> 4) | (packet[16])) - 128.0f) / -128.0f);
            state.SetAnalog("y", atari5200_y);

            return state.Build();
        }
    }
}