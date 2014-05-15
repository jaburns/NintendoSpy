using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    public class BlinkReductionFilter
    {
        public bool Enabled { get; set; }

        List <ControllerState> _states = new List <ControllerState> ();

        public BlinkReductionFilter ()
        {
            Enabled = false;
            _states.Add (ControllerState.Zero);
            _states.Add (ControllerState.Zero);
            _states.Add (ControllerState.Zero);
        }

        public ControllerState Process (ControllerState state)
        {
            if (!Enabled) return state;

            _states.RemoveAt (0);
            _states.Add (state);

            var stateIsNoise = false;

            foreach (var button in _states[0].Buttons.Keys)
            {
                if (_states[0].Buttons[button] == _states[2].Buttons[button] &&
                    _states[0].Buttons[button] != _states[1].Buttons[button])
                {
                    stateIsNoise = true;
                    break;
                }
            }

            if (stateIsNoise) {
                // TODO check analogs
            }

            return stateIsNoise ? _states[2] : _states[1];
        }
    }
}
