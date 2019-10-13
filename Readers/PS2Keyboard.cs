using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    class PS2Keyboard
    {
        const int PACKET_SIZE = 68;

        static readonly string[] KEYS = {
            null, "F9", null, "F5", "F3", "F1", "F2", "F12", null, "F10", "F8", "F6", "F4", "Tab", "`", null, null,
            "AltLeft", "ShiftLeft", null, "CtrlLeft", "Q", "1", null, null, null, "Z", "S", "A", "W", "2", null, null, "C",
            "X", "D", "E", "4", "3", null, null, "Spacebar", "V", "F", "T", "R", "5", null, null, "N", "B", "H", "G", "Y",
            "6", null, null, null, "M", "J", "U", "7", "8", null, null, ",",
            "K", "I", "O", "0", "9", null, null, ".", "/", "L", ";",
            "P", "-", null, null, null, "'", null, "[", "=", null, null, "CapsLock", "ShiftRight", "Enter", "]",
            null, "\\", null, null, null, null, null, null, null, null, "Backspace", null, null, "Num1", null, "Num4",
            "Num7", null, null, null, "Num0", "Num.", "Num2", "Num5", "Num6", "Num8", "ESC", "NumLock", "F11", "Num+",
            "Num3", "Num-", "Num*", "Num9", "ScrollLock", null, null, null, null, "F7", null, null, null, null,
            null, null, null, null, null, null, null, null, null, "AltRight", "PrtScr", null, "CtrlRight", null, null, null,
            null, null, null, null, null, null, null, "WindowsLeft", null, null, null, null, null, null,
            null, "WindowsRight", null, null, null, null, null, null, null, "Menus", null, null,
            null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, "Num/", null,
            null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, "NumEnter", null, null, null, null, null, null, "PauseBreak", null,
            null, null, null, null, null, null, "End", null, "LeftArrow", "Home", null,
            null, null, "Insert", "Delete", "DownArrow", null, "RightArrow", "UpArrow",
            null, null, null, null, "PageDown", null, "PrtScr", "PageUp", null, null
        };

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] reconstructedPacket = new byte[34];

            int j = 0;
            for (int i = 0; i < 34; ++i)
            {
                reconstructedPacket[i] = (byte)((packet[j] >> 4) | packet[j+1]);
                j += 2;
            }

            byte[] polishedPacket = new byte[256];

            int checksum = 0;
            for (int i = 0; i < 32; ++i)
            {
                checksum += reconstructedPacket[i];
                for (int k = 0; k < 8; ++k)
                {
                    polishedPacket[(i * 8) + k] = (byte)((reconstructedPacket[i] & (1 << k)) != 0 ? 1 : 0);
                }
            }

            var state = new ControllerStateBuilder();

            short sentChecksum = (short)((reconstructedPacket[33] << 8) | reconstructedPacket[32]);

            if (checksum != sentChecksum)
            {
                state.SetButton("ChecksumFail", true);
                return state.Build();
            }


            for (int i = 0; i < KEYS.Length; ++i)
            {
                if (string.IsNullOrEmpty(KEYS[i])) continue;
                state.SetButton(KEYS[i], polishedPacket[i] != 0x00);
                state.SetButton("ChecksumFail", false);
            }

            return state.Build();
        }
    }
}