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
    sealed public class GamepadReader : IControllerReader
    {
        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        const double TIMER_MS = 30;
        const int RANGE = 1000;

        DirectInput _dinput;
        DispatcherTimer _timer;
        Joystick _joystick;
        public static List<uint> GetDevices()
        {
            int amount = new DirectInput().GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).Count;
            var result = new List<uint>(amount);
            for (uint i = 0; i < amount; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public GamepadReader (int id = 0)
        {
            _dinput = new DirectInput();
 
            var devices = _dinput.GetDevices (DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            if (devices.Count - 1 < id) {
                throw new IOException ("GamepadReader could not find a connected gamepad with the given id.");
            }
            _joystick = new Joystick (_dinput, devices[id].InstanceGuid);
 
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

            var outState = new ControllerStateBuilder ();            
            var state = _joystick.GetCurrentState ();

            for (int i = 0; i < _joystick.Capabilities.ButtonCount; ++i) {
                outState.SetButton ("b"+i.ToString(), state.IsPressed(i));
            }

            int[] pov = state.GetPointOfViewControllers ();

            outState.SetButton ("up", false);
            outState.SetButton ("right", false);
            outState.SetButton ("down", false);
            outState.SetButton ("left", false);

            if (pov != null && pov.Length > 0 && pov[0] >= 0) {
                outState.SetButton ("up", pov[0] > octantAngle (6) || pov[0] < octantAngle (1));
                outState.SetButton ("right", pov[0] > octantAngle (0) && pov[0] < octantAngle (3));
                outState.SetButton ("down", pov[0] > octantAngle (2) && pov[0] < octantAngle (5));
                outState.SetButton ("left", pov[0] > octantAngle (4) && pov[0] < octantAngle (7));
            }    

            outState.SetAnalog ("x", (float)state.X / RANGE);
            outState.SetAnalog ("y", (float)state.Y / RANGE);
            outState.SetAnalog ("z", (float)state.Z / RANGE);
            outState.SetAnalog ("rx", (float)state.RotationX / RANGE);
            outState.SetAnalog ("ry", (float)state.RotationY / RANGE);
            outState.SetAnalog ("rz", (float)state.RotationZ / RANGE);

            if (ControllerStateChanged != null) ControllerStateChanged (this, outState.Build ());
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
