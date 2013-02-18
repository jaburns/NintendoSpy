using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NintendoSpy
{
    public struct DisplayStick
    {
        private Control _display;
        
        public Control display {
            set { 
                _display = value;
                baseLeft = _display.Left;
                baseTop  = _display.Top;
            }
            get { return _display;  }
        }
        
        public int movementRadius;

        public int baseLeft { get; private set; }
        public int baseTop  { get; private set; }
    }

    public class ViewManager
    {
        private Form _parent;
        private IControllerReader _reader;
        private Control[] _buttons;
        private DisplayStick[] _sticks;

        private SerialMonitor _serialMonitor;

        public ViewManager( Form parent, string comPort, IControllerReader reader, Control[] buttons, DisplayStick[] sticks )
        {
            _parent  = parent;
            _reader  = reader;
            _buttons = buttons;
            _sticks  = sticks;

            foreach( Control p in _buttons ) {
                if( p == null ) continue;
                p.Visible = false;
            }

            _serialMonitor = new SerialMonitor( comPort );
            _serialMonitor.PacketReceived += serialMonitor_PacketReceived;
            _serialMonitor.Disconnected += _serialMonitor_Disconnected;
            _serialMonitor.Start();
        }

        public void Close()
        {
            if( _serialMonitor != null ) {
                _serialMonitor.Stop();
                _serialMonitor = null;
            }
        }

        private void _serialMonitor_Disconnected(object sender, EventArgs e)
        {
            _parent.BeginInvoke( new Action( () => { _parent.Close(); } ) );
        }

        private void serialMonitor_PacketReceived( object sender, byte[] packet )
        {
            _parent.BeginInvoke( new Action( () => { update( packet ); } ) );
        }

        private void update( byte[] packet )
        {
            _reader.ReadFromPacket( packet );

            // Update buttons.
            bool[] buttonStates = _reader.GetButtonStates();
            for( int i = 0 ; i < _buttons.Length && i < buttonStates.Length ; ++i ) {
                if( _buttons[i] == null ) continue;
                _buttons[i].Visible = buttonStates[i];
            }

            ControllerStickState[] stickStates = _reader.GetStickStates();
            if( stickStates != null ) {
                for( int i = 0 ; i < _sticks.Length && i < stickStates.Length ; ++i ) {
                    _sticks[i].display.Left = _sticks[i].baseLeft + (int)( _sticks[i].movementRadius * stickStates[i].X );
                    _sticks[i].display.Top  = _sticks[i].baseTop  - (int)( _sticks[i].movementRadius * stickStates[i].Y );
                }
            }


        }
    }
}
