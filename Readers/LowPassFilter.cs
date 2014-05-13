using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    public class LowPassFilter
    {
        public class ZeroControllerState : IControllerState
        {            
            Dictionary <string, bool> _buttons = new Dictionary <string, bool> ();
            Dictionary <string, float> _analogs = new Dictionary <string, float> ();

            public IReadOnlyDictionary <string, bool> Buttons { get { return _buttons; } }
            public IReadOnlyDictionary <string, float> Analogs { get { return _analogs; } }
        }

        ZeroControllerState _tempZeroControllerState = new ZeroControllerState ();

        public bool Enabled { get; set; }

        public LowPassFilter () {
            Enabled = false;
        }

        public IControllerState Process (IControllerState newState)
        {
            if (!Enabled) return newState;

            // TODO stuff instead of returning zero.
            return _tempZeroControllerState;
        }
    }
}
