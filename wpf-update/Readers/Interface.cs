using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintendoSpy.Readers
{
    public struct ControlStickState {
        public float X, Y;
    }

    public interface IControllerState 
    {
        IReadOnlyDictionary <string, bool> Buttons { get; }
        IReadOnlyDictionary <string, ControlStickState> Sticks { get; }
        IReadOnlyDictionary <string, float> Analogs { get; }
    }

    public interface ISerialControllerState : IControllerState
    {
        void ReadFromPacket (byte[] packet);
    }

    public interface IControllerReader
    {
        IControllerState State { get; }
        event EventHandler ControllerStateChanged;
        event EventHandler ControllerDisconnected;
        void Finish ();
    }
}
