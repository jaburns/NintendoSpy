using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintendoSpy.Readers
{
    public class ControllerState
    {
        static public readonly ControllerState Zero = new ControllerState
            (new Dictionary <string, bool> (), new Dictionary <string, float> ());

        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }
        public IReadOnlyDictionary <string, float> Analogs { get; private set;  }

        public ControllerState (IReadOnlyDictionary <string, bool> buttons, IReadOnlyDictionary <string, float> analogs)
        {
            Buttons = buttons;
            Analogs = analogs;
        }
    }
}
