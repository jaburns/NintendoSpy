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

            for (int i = 0; i < buttons.Length; ++i) {
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
            "a", "b", "select", "start", "up", "down", "left", "right", "2", "1", "5", "9", "6", "10", "11", "7", "4", "3", "12", "8", null, null, null, null
        };

        static readonly string[] BUTTONS_NES_BACKCOMPAT = {
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

        static readonly string[] BUTTONS_AMIGA_ANALOG =
        {
            "1", "2", "3", "4", null, null
        };

        static readonly string[] BUTTONS_PSCLASSIC =
        {
            "r1", "l1", "r2", "l2", "square", "x", "circle", "triangle", null, null, "down", "up", "right", "left", "start", "select"
        };

        static readonly string[] BUTTONS_PCFX =
        {
            null, "1", "2", "3", "4", "5", "6", "select", "run", "up", "right", "down", "left", "mode1", null, "mode2"
        };

        static public ControllerState ReadFromPacket_Intellivision(byte[] packet)
        {
            return readPacketButtons(packet, BUTTONS_INTELLIVISION);
        }

        static public ControllerState ReadFromPacket_NES (byte[] packet) {
            return readPacketButtons(packet, packet.Length == 8 ? BUTTONS_NES_BACKCOMPAT : BUTTONS_NES);
        }

        static public ControllerState ReadFromPacket_PCFX(byte[] packet)
        {
            if (packet.Length != BUTTONS_PCFX.Length) return null;
            return readPacketButtons(packet, BUTTONS_PCFX);
        }

        static public ControllerState ReadFromPacket_PSClassic(byte[] packet) {
                return readPacketButtons_ascii(packet, BUTTONS_PSCLASSIC);
        }

        static private float AmigaAnalogXAxisData;

        static public ControllerState ReadFromPacket2_CD32(byte[] packet)
        {
            if (packet.Length == 6)
            {
                AmigaAnalogXAxisData = (((packet[4] >> 4) | (packet[5])) - 15.0f) / 15.0f;
            }
            return null;
        }

        static public ControllerState ReadFromPacket_CD32(byte[] packet)
        {
            ControllerStateBuilder state = null;
            if (packet.Length == 13)
            {
                return Classic.ReadFromPacket(packet);
            }
            else if (packet.Length == 6)
            {
                state = new ControllerStateBuilder();

                for (int i = 0; i < BUTTONS_AMIGA_ANALOG.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_AMIGA_ANALOG[i])) continue;
                    state.SetButton(BUTTONS_AMIGA_ANALOG[i], packet[i] == 0x01);
                }

                state.SetAnalog("y",(((packet[4] >> 4) | (packet[5])) - 15.0f)/-15.0f);
                state.SetAnalog("x", AmigaAnalogXAxisData);
            }
            else if (packet.Length == BUTTONS_CD32.Length)
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
                state.SetButton("right", (packet[0]& 0b00010000) == 0);
            }
            else if (packet.Length == 19)
            {
                state = new ControllerStateBuilder();

                state.SetButton("left", packet[0] != 0x00);
                state.SetButton("right", packet[2] != 0x00);

                sbyte xVal = (sbyte)SignalTool.readByteBackwards(packet, 3);
                sbyte yVal = (sbyte)SignalTool.readByteBackwards(packet, 11);

                SignalTool.SetMouseProperties( xVal / -128.0f, yVal / 128.0f, state);
            }

            return state != null ? state.Build() : null;
        }

        static public ControllerState ReadFromPacket_SNES (byte[] packet) {
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

        static public ControllerState ReadFromPacket_Jaguar (byte[] packet)
        {
            if (packet.Length < 4) return null;

            var state = new ControllerStateBuilder();

            state.SetButton("pause",    (packet[0] & 0b00000100) == 0x00);
            state.SetButton("a",        (packet[0] & 0b00001000) == 0x00);
            state.SetButton("right",    (packet[0] & 0b00010000) == 0x00);
            state.SetButton("left",     (packet[0] & 0b00100000) == 0x00);
            state.SetButton("down",     (packet[0] & 0b01000000) == 0x00);
            state.SetButton("up",       (packet[0] & 0b10000000) == 0x00);

            state.SetButton("b",        (packet[1] & 0b00001000) == 0x00);
            state.SetButton("1",        (packet[1] & 0b00010000) == 0x00);
            state.SetButton("4",        (packet[1] & 0b00100000) == 0x00);
            state.SetButton("l",        (packet[1] & 0b00100000) == 0x00);
            state.SetButton("7",        (packet[1] & 0b01000000) == 0x00);
            state.SetButton("x",        (packet[1] & 0b01000000) == 0x00);
            state.SetButton("star",     (packet[1] & 0b10000000) == 0x00);

            state.SetButton("c", (packet[2] & 0b00001000) == 0x00);
            state.SetButton("2", (packet[2] & 0b00010000) == 0x00);
            state.SetButton("5", (packet[2] & 0b00100000) == 0x00);
            state.SetButton("8", (packet[2] & 0b01000000) == 0x00);
            state.SetButton("y", (packet[2] & 0b01000000) == 0x00);
            state.SetButton("0", (packet[2] & 0b10000000) == 0x00);

            state.SetButton("option",   (packet[3] & 0b00001000) == 0x00);
            state.SetButton("3",        (packet[3] & 0b00010000) == 0x00);
            state.SetButton("6",        (packet[3] & 0b00100000) == 0x00);
            state.SetButton("r",        (packet[3] & 0b00100000) == 0x00);
            state.SetButton("9",        (packet[3] & 0b01000000) == 0x00);
            state.SetButton("z",        (packet[3] & 0b01000000) == 0x00);
            state.SetButton("pound",    (packet[3] & 0b10000000) == 0x00);

            return state.Build();

        }

    }
}
