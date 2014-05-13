using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    public delegate void StateEventHandler (IControllerReader sender, ControllerState state);

    public interface IControllerReader
    {
        event StateEventHandler ControllerStateChanged;
        event EventHandler ControllerDisconnected;

        void Finish ();
    }
}
