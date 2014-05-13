using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintendoSpy.Readers
{
    public class ControllerState
    {
        public IReadOnlyDictionary <string, bool> Buttons { get; private set; }
        public IReadOnlyDictionary <string, float> Analogs { get; private set;  }

        public ControllerState (IReadOnlyDictionary <string, bool> buttons, IReadOnlyDictionary <string, float> analogs)
        {
            Buttons = buttons;
            Analogs = analogs;
        }
    }
}
