using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Timers;

namespace N64Spy
{
    public delegate void PacketEventHandler( object sender, byte[] packet );

    public class SerialMonitor
    {
        public event PacketEventHandler PacketReceived;

        private SerialPort datPort;
        private List<byte> localBuffer;

        private Timer timer;
        private int packetSize;

        public SerialMonitor( string portName, int baudRate, int packetSize )
        {
            this.packetSize = packetSize;
            this.localBuffer = new List<byte>();
            this.datPort = new SerialPort( portName, baudRate );
        }

        public void Start()
        {
            if( timer != null ) return;

            localBuffer.Clear();
            datPort.Open();
            timer = new Timer( 30 );
            timer.Elapsed += new ElapsedEventHandler( tick );
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            timer = null;
            datPort.Close();
        }

        private void tick( object sender, ElapsedEventArgs e )
        {
            // Read some data from the COM port and append it to our localBuffer.
            int readCount = datPort.BytesToRead;
            if( readCount < 1 ) return;
            byte[] readBuffer = new byte[ readCount ];
            datPort.Read( readBuffer, 0, readCount );
            datPort.DiscardInBuffer();
            localBuffer.AddRange( readBuffer );

            int lastSentinelIndex = localBuffer.LastIndexOf( 0x0A );
            int tailSize = localBuffer.Count - lastSentinelIndex;

            // If we have enough data on the tail for a full read, process the end as current packet and wipe the local buffer.
            if( tailSize == packetSize ) { 
                PacketReceived(
                    this,
                    localBuffer.GetRange(
                        lastSentinelIndex, packetSize
                    ).ToArray()
                );
                localBuffer.Clear();
            }
            // Otherwise, if we have enough buffer built up to do a read then do it and clear up to the last 
            else if( localBuffer.Count >= tailSize + packetSize ) {
                PacketReceived(
                    this,
                    localBuffer.GetRange(
                        lastSentinelIndex - packetSize, packetSize
                    ).ToArray()
                );
                localBuffer.RemoveRange( 0, lastSentinelIndex ); // Remove up to the last sentinel in the buffer.
            }
        }

    }
}