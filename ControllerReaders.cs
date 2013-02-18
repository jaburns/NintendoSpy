using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintendoSpy
{
    public struct ControllerStickState
    {
        public float X, Y;
    }

    public interface IControllerReader
    {
        bool[] GetButtonStates();
        ControllerStickState[] GetStickStates();
        float[] GetTriggerStates();

        void ReadFromPacket( byte[] packet );
    }

    public static class SignalTools
    {
        // Reads a byte of data from a string of 8 bits in a controller data packet.
        public static byte readByte( byte[] packet, int offset )
        {
            byte val = 0;
            for( int i = 0 ; i < 8 ; ++i ) {
                if( (packet[i+offset] & 0x0F) != 0 ) {
                    val |= (byte)(1<<(7-i));
                }
            }
            return val;
        }
    }

    public class ControllerReader_GC : IControllerReader
    {
        private const int PACKET_SIZE = 64;
        private const int BUTTONS = 16;

        private bool[] _buttons;
        private ControllerStickState[] _sticks;
        private float[] _triggers;

        public bool[] GetButtonStates() { return _buttons; }
        public ControllerStickState[] GetStickStates() { return _sticks; }
        public float[] GetTriggerStates() { return _triggers; }

        public ControllerReader_GC()
        {
            _buttons = new bool[ BUTTONS ];
            _sticks = new ControllerStickState[2];
            _triggers = new float[2];
        }

        public void ReadFromPacket( byte[] packet )
        {
            if( packet.Length < PACKET_SIZE ) return;

            for( int i = 0 ; i < BUTTONS ; ++i ) {
                _buttons[i] = packet[i] != 0x00;
            }

            Func<byte,float> readStick   = (input) => (float)(input - 128) / 128;
            Func<byte,float> readTrigger = (input) => (float)(input)       / 256;

            _sticks[0].X = readStick( SignalTools.readByte( packet, BUTTONS      ) );
            _sticks[0].Y = readStick( SignalTools.readByte( packet, BUTTONS +  8 ) );
            _sticks[1].X = readStick( SignalTools.readByte( packet, BUTTONS + 16 ) );
            _sticks[1].Y = readStick( SignalTools.readByte( packet, BUTTONS + 24 ) );
            _triggers[0] = readStick( SignalTools.readByte( packet, BUTTONS + 32 ) );
            _triggers[1] = readStick( SignalTools.readByte( packet, BUTTONS + 40 ) );
        }
    }

    public class ControllerReader_N64 : IControllerReader
    {
        private const int PACKET_SIZE = 32;
        private const int BUTTONS = 16;

        private bool[] _buttons;
        private ControllerStickState[] _sticks;

        public bool[] GetButtonStates() { return _buttons; }
        public ControllerStickState[] GetStickStates() { return _sticks; }
        public float[] GetTriggerStates() { return null; }

        public ControllerReader_N64()
        {
            _buttons = new bool[ BUTTONS ];
            _sticks = new ControllerStickState[1];
        }

        public void ReadFromPacket( byte[] packet )
        {
            if( packet.Length < PACKET_SIZE ) return;

            for( int i = 0 ; i < BUTTONS ; ++i ) {
                _buttons[i] = packet[i] != 0x00;
            }

            Func<byte,float> readStick = (input) => (float)((sbyte)input) / 128;

            _sticks[0].X = readStick( SignalTools.readByte( packet, BUTTONS      ) );
            _sticks[0].Y = readStick( SignalTools.readByte( packet, BUTTONS +  8 ) );
        }
    }

    public class ControllerReader_SNES : IControllerReader
    {
        private const int PACKET_SIZE = 16;
        private const int BUTTONS = 16;

        private bool[] _buttons;

        public bool[] GetButtonStates() { return _buttons; }
        public ControllerStickState[] GetStickStates() { return null; }
        public float[] GetTriggerStates() { return null; }

        public ControllerReader_SNES()
        {
            _buttons = new bool[ BUTTONS ];
        }

        public void ReadFromPacket( byte[] packet )
        {
            if( packet.Length < PACKET_SIZE ) return;

            for( int i = 0 ; i < BUTTONS ; ++i ) {
                _buttons[i] = packet[i] != 0x00;
            }
        }
    }
}
