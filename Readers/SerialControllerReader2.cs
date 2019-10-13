using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    sealed public class SerialControllerReader2 : IControllerReader 
    {
        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        public event EventHandler ControllerDisconnected2;

        Func <byte[], ControllerState> _packetParser;
        Func<byte[], ControllerState> _packet2Parser;
        SerialMonitor _serialMonitor;
        SerialMonitor _serialMonitor2;

        public SerialControllerReader2 (string portName, string port2Name, Func <byte[], ControllerState> packetParser, Func<byte[], ControllerState> packet2Parser) 
        {
            _packetParser = packetParser;
            _packet2Parser = packet2Parser;

            _serialMonitor = new SerialMonitor (portName);
            _serialMonitor.PacketReceived += serialMonitor_PacketReceived;
            _serialMonitor.Disconnected += serialMonitor_Disconnected;
            _serialMonitor.Start();

            if (port2Name != "Not Connected")
            {
                _serialMonitor2 = new SerialMonitor(port2Name);
                _serialMonitor2.PacketReceived += serialMonitor2_PacketReceived;
                _serialMonitor2.Disconnected += serialMonitor2_Disconnected;
                _serialMonitor2.Start();

            }
            else
                _serialMonitor2 = null;                
        }

        void serialMonitor_Disconnected(object sender, EventArgs e)
        {
            Finish ();
            if (ControllerDisconnected != null) ControllerDisconnected (this, EventArgs.Empty);
        }

        void serialMonitor_PacketReceived (object sender, byte[] packet)
        {
            if (ControllerStateChanged != null) {
                var state = _packetParser (packet);
                if (state != null) {
                    ControllerStateChanged (this, state);
                }
            }
        }

        void serialMonitor2_Disconnected(object sender, EventArgs e)
        {
            Finish();
            if (ControllerDisconnected2 != null) ControllerDisconnected2(this, EventArgs.Empty);
        }

        void serialMonitor2_PacketReceived(object sender, byte[] packet)
        {
            _packet2Parser(packet);
        }

        public void Finish ()
        {
            if (_serialMonitor != null) {
                _serialMonitor.Stop ();
                _serialMonitor = null;
            }

            if (_serialMonitor2 != null)
            {
                _serialMonitor2.Stop();
                _serialMonitor2 = null;
            }
        }
    }
}
