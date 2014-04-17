using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NintendoSpy.Readers
{
    sealed public class XInputReader : IControllerReader, IControllerState
    {
    // ----- Interface implementations with backing state -------------------------------------------------------------

        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }

        Dictionary <string, ControlStickState> _sticks = new Dictionary <string, ControlStickState> ();
        public IReadOnlyDictionary <string, ControlStickState> Sticks { get; private set; }

        Dictionary <string, float> _analogs = new Dictionary <string, float> ();
        public IReadOnlyDictionary <string, float> Analogs { get; private set; }

        public IControllerState State { get { return this; } }

        public event EventHandler ControllerStateChanged;

    // ----- Interop with XInput DLL ----------------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential)]
        struct XInputState
        {
            public UInt32 dwPacketNumber;
            public UInt16 wButtons;
            public Byte bLeftTrigger;
            public Byte bRightTrigger;
            public Int16 sThumbLX;
            public Int16 sThumbLY;
            public Int16 sThumbRX;
            public Int16 sThumbRY;
        }

        static class XInputDLL {
            [DllImport("xinput9_1_0.dll")]
            static public extern uint XInputGetState (uint userIndex, ref XInputState inputState);
        }

    // ----------------------------------------------------------------------------------------------------------------

        const double TIMER_MS = 30;

        DispatcherTimer _timer;

        public XInputReader ()
        {
            Buttons = _buttons;
            Sticks = _sticks;
            Analogs = _analogs;

            _timer = new DispatcherTimer ();
            _timer.Interval = TimeSpan.FromMilliseconds (TIMER_MS);
            _timer.Tick += tick;
            _timer.Start ();
        }

        void tick (object sender, EventArgs e)
        {
            XInputState state = new XInputState ();
            XInputDLL.XInputGetState (0, ref state);

            _buttons ["a"]     = (state.wButtons & 0x1000) != 0;
            _buttons ["b"]     = (state.wButtons & 0x2000) != 0;
            _buttons ["x"]     = (state.wButtons & 0x4000) != 0;
            _buttons ["y"]     = (state.wButtons & 0x8000) != 0;
            _buttons ["up"]    = (state.wButtons & 0x0001) != 0;
            _buttons ["down"]  = (state.wButtons & 0x0002) != 0;
            _buttons ["left"]  = (state.wButtons & 0x0004) != 0;
            _buttons ["right"] = (state.wButtons & 0x0008) != 0;
            _buttons ["start"] = (state.wButtons & 0x0010) != 0;
            _buttons ["back"]  = (state.wButtons & 0x0020) != 0;
            _buttons ["l3"]    = (state.wButtons & 0x0040) != 0;
            _buttons ["r3"]    = (state.wButtons & 0x0080) != 0;
            _buttons ["l"]     = (state.wButtons & 0x0100) != 0;
            _buttons ["r"]     = (state.wButtons & 0x0200) != 0;

            _sticks ["left"] = new ControlStickState {
                X = (float)state.sThumbLX / 32768,
                Y = (float)state.sThumbLY / 32768
            };
            _sticks ["right"] = new ControlStickState {
                X = (float)state.sThumbRX / 32768,
                Y = (float)state.sThumbRY / 32768
            };

            _analogs ["l"] = (float)state.bLeftTrigger / 255;
            _analogs ["r"] = (float)state.bRightTrigger / 255;

            if (ControllerStateChanged != null) ControllerStateChanged (this, null);
        }
    }
}
