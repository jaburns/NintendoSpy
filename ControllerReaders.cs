using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N64Spy
{
    public struct ControllerStickState {
        public float X, Y;
    }

    public interface IControllerReader
    {
        bool GetButtonState( int button );
        int GetButtonCount();
        ControllerStickState GetStickState( int stick );
        int GetStickCount();

        void ReadFromPacket( byte[] packet );
    }


    public class ControllerReader_N64 : IControllerReader
    {
        private const int ButtonCount = 16;
        private bool[] buttons;
        private ControllerStickState stick;

        public bool GetButtonState( int button )
        {
            if( button < 0 || button >= ButtonCount ) return false;
            return buttons[button];
        }
        public int GetButtonCount() { return ButtonCount; }
        public ControllerStickState GetStickState( int stickId ) { return stick; }
        public int GetStickCount() { return 1; }

        public void ReadFromPacket( byte[] packet )
        {
            // Read the button states.
            buttons = new bool[ ButtonCount ];
            for( int i = 0 ; i < ButtonCount ; ++i ) {
                buttons[i] = (packet[i+1] & 0x0F) != 0;
            }
            
            // Read the analog stick state.
            stick.X = readAnalogValue( packet, 1 + ButtonCount     );
            stick.Y = readAnalogValue( packet, 1 + ButtonCount + 8 );
        }

        // Reads an analog value from the raw N64 data packet and returns a value in [ -1.0 , 1.0 ]
        private float readAnalogValue( byte[] packet, int offset )
        {
            byte val = 0;
            for( int i = 0 ; i < 8 ; ++i ) {
                if( (packet[i+offset] & 0x0F) != 0 ) {
                    val |= (byte)(1<<(7-i));
                }
            }
            return (float)((sbyte)val) / 128;
        }
    }


    public class ControllerReader_SNES : IControllerReader
    {
        private const int ButtonCount = 16;
        private bool[] buttons;

        public bool GetButtonState( int button )
        {
            if( button < 0 || button >= ButtonCount ) return false;
            return buttons[button];
        }
        public int GetButtonCount() { return ButtonCount; }
        public ControllerStickState GetStickState( int stickId ) { throw new NotSupportedException(); }
        public int GetStickCount() { return 0; }

        public void ReadFromPacket( byte[] packet )
        {
            // Read the button states.
            buttons = new bool[ ButtonCount ];
            for( int i = 0 ; i < ButtonCount ; ++i ) {
                buttons[i] = packet[i+1] == '0';
            }
        }
    }
}
