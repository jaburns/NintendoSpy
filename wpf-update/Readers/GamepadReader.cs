using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.DirectInput;
using System.Windows.Threading;

namespace NintendoSpy.Readers
{
    sealed public class GamepadReader : IControllerReader, IControllerState
    {
    // ----- Interface implementations with backing state -------------------------------------------------------------

        Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }

        Dictionary <string, float> _analogs = new Dictionary <string, float> ();
        public IReadOnlyDictionary <string, float> Analogs { get; private set; }

        public IControllerState State { get { return this; } }

        public event EventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

    // ----------------------------------------------------------------------------------------------------------------

        const double TIMER_MS = 30;

        DirectInput _dinput;
        DispatcherTimer _timer;
        Joystick _joystick;

        public GamepadReader ()
        {
            _dinput = new DirectInput();
 
            foreach (var device in _dinput.GetDevices (DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly)) {
                _joystick = new Joystick (_dinput, device.InstanceGuid);
                break;
            }

            _joystick.Poll ();
            _joystick.GetCurrentState ();
 
            foreach (var obj in _joystick.GetObjects()) {
                if ((obj.ObjectType & ObjectDeviceType.Axis) != 0) {
                    _joystick.GetObjectPropertiesById ((int)obj.ObjectType).SetRange(-1000, 1000);
                }
            }
 
            _joystick.Acquire();

            _timer = new DispatcherTimer ();
            _timer.Interval = TimeSpan.FromMilliseconds (TIMER_MS);
            _timer.Tick += tick;
            _timer.Start ();
        }

        void tick (object sender, EventArgs e)
        {
            _joystick.Poll (); // TODO handle .IsFailure
            
            var state = _joystick.GetCurrentState ();

            var buttons = state.GetButtons ();
            for (int i = 0; i < buttons.Length; ++i) {
                _buttons ["b"+i.ToString()] = buttons[i];
            }
        }

        public void Finish ()
        {

        }
    }
}
