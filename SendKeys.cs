using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy
{
    // Keycodes: http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
    // Letter keys map to 0x41.. etc. (i.e. capital ASCII letters)

    public class SendKeys
    {
        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        static INPUT inputForKey (ushort key, bool releasing)
        {
            return new INPUT {
                type = INPUT_KEYBOARD,
                u = new InputUnion {
                    ki = new KEYBDINPUT {
                        wVk = key,
                        wScan = 0,
                        dwFlags = releasing ? KEYEVENTF_KEYUP : 0,
                        dwExtraInfo = GetMessageExtraInfo(),
                    }
                }
            };
        }

        static void sendInputs (params INPUT[] inputs)
        {
            SendInput ((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        static public void PressKey (ushort key)
        {
            sendInputs (inputForKey (key, false));
        }

        static public void ReleaseKey(ushort key)
        {
            sendInputs (inputForKey (key, true));
        }

        static public void PressAndReleaseKey (ushort key)
        {
            sendInputs (
                inputForKey (key, false),
                inputForKey (key, true)
            );
        }
    }
}
