using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.DirectInput;
using System.Windows.Threading;
using System.IO;

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
        const int RANGE = 1000;

        DirectInput _dinput;
        DispatcherTimer _timer;
        Joystick _joystick;

        public GamepadReader ()
        {
            Buttons = _buttons;
            Analogs = _analogs;

            _dinput = new DirectInput();
 
            var devices = _dinput.GetDevices (DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            if (devices.Count < 1) {
                throw new IOException ("GamepadReader could not find a connected gamepad.");
            }
            _joystick = new Joystick (_dinput, devices[0].InstanceGuid);
 
            foreach (var obj in _joystick.GetObjects()) {
                if ((obj.ObjectType & ObjectDeviceType.Axis) != 0) {
                    _joystick.GetObjectPropertiesById ((int)obj.ObjectType).SetRange (-RANGE, RANGE);
                }
            }
 
            if (_joystick.Acquire().IsFailure) {
                throw new IOException ("Connected gamepad could not be acquired.");
            }

            _timer = new DispatcherTimer ();
            _timer.Interval = TimeSpan.FromMilliseconds (TIMER_MS);
            _timer.Tick += tick;
            _timer.Start ();
        }

        static int octantAngle (int octant) {
            return 2750 + 4500 * octant;
        }

        void tick (object sender, EventArgs e)
        {
            if (_joystick.Poll().IsFailure) {
                Finish ();
                if (ControllerDisconnected != null) ControllerDisconnected (this, EventArgs.Empty);
                return;
            }
            
            var state = _joystick.GetCurrentState ();

            for (int i = 0; i < _joystick.Capabilities.ButtonCount; ++i) {
                _buttons ["b"+i.ToString()] = state.IsPressed(i);
            }

            int[] pov = state.GetPointOfViewControllers ();

            _buttons ["up"] = false;
            _buttons ["right"] = false;
            _buttons ["down"] = false;
            _buttons ["left"] = false;

            if (pov != null && pov.Length > 0 && pov[0] >= 0) {
                _buttons ["up"] = pov[0] > octantAngle (6) || pov[0] < octantAngle (1);
                _buttons ["right"] = pov[0] > octantAngle (0) && pov[0] < octantAngle (3);
                _buttons ["down"] = pov[0] > octantAngle (2) && pov[0] < octantAngle (5);
                _buttons ["left"] = pov[0] > octantAngle (4) && pov[0] < octantAngle (7);
            }    

            _analogs ["x"] = (float)state.X / RANGE;
            _analogs ["y"] = (float)state.Y / RANGE;
            _analogs ["z"] = (float)state.Z / RANGE;
            _analogs ["rx"] = (float)state.RotationX / RANGE;
            _analogs ["ry"] = (float)state.RotationY / RANGE;
            _analogs ["rz"] = (float)state.RotationZ / RANGE;

            if (ControllerStateChanged != null) ControllerStateChanged (this, EventArgs.Empty);
        }

        public void Finish ()
        {
            if (_joystick != null) {
                _joystick.Unacquire ();
                _joystick.Dispose ();
                _joystick = null;
            }
            if (_timer != null) {
                _timer.Stop ();
                _timer = null;
            }
        }
    }
}
