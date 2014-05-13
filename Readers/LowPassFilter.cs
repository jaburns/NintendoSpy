using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    public class LowPassFilter
    {
        public bool Enabled { get; set; }

        public LowPassFilter () {
            Enabled = false;
        }

        public ControllerState Process (ControllerState newState)
        {
            if (!Enabled) return newState;

            // TODO stuff instead of returning zero.
            return newState;
        }
    }
}
