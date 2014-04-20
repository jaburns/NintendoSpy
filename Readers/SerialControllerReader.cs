using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class SerialControllerReader : IControllerReader 
    {
        public event EventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        ISerialControllerState _controller;
        public IControllerState State { get { return _controller; } } 

        SerialMonitor _serialMonitor;

        public SerialControllerReader (string portName, ISerialControllerState controller) 
        {
            _controller = controller;

            _serialMonitor = new SerialMonitor (portName);
            _serialMonitor.PacketReceived += serialMonitor_PacketReceived;
            _serialMonitor.Start ();
        }

        void serialMonitor_PacketReceived (object sender, byte[] packet)
        {
            _controller.ReadFromPacket (packet);
            if (ControllerStateChanged != null) ControllerStateChanged (this, EventArgs.Empty);
        }

        public void Finish ()
        {

        }
    }
}
