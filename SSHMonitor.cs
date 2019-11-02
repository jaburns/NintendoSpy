using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;

namespace RetroSpy
{
    //public delegate void PacketEventHandler (object sender, byte[] packet);

    public class SSHMonitor
    {
        const int BAUD_RATE = 115200;
        const int TIMER_MS  = 7;

        public event PacketEventHandler PacketReceived;
        public event EventHandler Disconnected;

        SshClient _client;
        ShellStream _data;
        List <byte> _localBuffer;
        string _command;

        DispatcherTimer _timer;

        public SSHMonitor(string hostname, string command)
        {
            _localBuffer = new List <byte> ();
            //_datPort = new SerialPort (portName, BAUD_RATE);
            _client = new SshClient(hostname, "retrospy", "retrospy");
            _command = command;
        }

        public void Start ()
        {
            if (_timer != null) return;

            _localBuffer.Clear ();
            //_datPort.Open ();
            _client.Connect();
            _data = _client.CreateShellStream("", 0, 0, 0, 0, 0);
            _data.WriteLine(_command);

            _timer = new DispatcherTimer ();
            _timer.Interval = TimeSpan.FromMilliseconds (TIMER_MS); 
            _timer.Tick += tick;
            _timer.Start ();
        }

        public void Stop ()
        {
            if (_data != null) {
                try { // If the device has been unplugged, Close will throw an IOException.  This is fine, we'll just keep cleaning up.
                    _data.Close ();
                }
                catch (IOException) {}
                _data = null;
                _client.Disconnect();
            }
            if (_timer != null) {
                _timer.Stop ();
                _timer = null;
            }
        }

        void tick (object sender, EventArgs e)
        {
            if (_data == null || !_data.CanRead || PacketReceived == null) return;

            // Try to read some data from the COM port and append it to our localBuffer.
            // If there's an IOException then the device has been disconnected.
            try {
                int readCount = (int)_data.Length;
                if (readCount < 1) return;
                byte[] readBuffer = new byte [readCount];
                _data.Read (readBuffer, 0, readCount);
                _localBuffer.AddRange (readBuffer);
            }
            catch (IOException) {
                Stop ();
                if (Disconnected != null) Disconnected (this, EventArgs.Empty);
                return;
            }

            // Try and find 2 splitting characters in our buffer.
            int lastSplitIndex = _localBuffer.LastIndexOf (0x0A);
            if (lastSplitIndex <= 1) return;
            int sndLastSplitIndex = _localBuffer.LastIndexOf (0x0A, lastSplitIndex - 1);
            if (lastSplitIndex == -1) return;

            // Grab the latest packet out of the buffer and fire it off to the receive event listeners.
            int packetStart = sndLastSplitIndex + 1;
            int packetSize  = lastSplitIndex - packetStart;
            PacketReceived (this, _localBuffer.GetRange (packetStart, packetSize).ToArray ());

            // Clear our buffer up until the last split character.
            _localBuffer.RemoveRange (0, lastSplitIndex);
        }
    }
}
