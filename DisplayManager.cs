using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace N64Spy
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

    // This class controls the visibility and position of visualization form controls for buttons and sticks in a ControllerState.
    public class DisplayManager
    {
        private IControllerReader reader;
        private Control[] buttons;
        private DisplayStick[] sticks;

        public DisplayManager( IControllerReader reader, Control[] buttons, DisplayStick[] sticks )
        {
            this.reader = reader;
            this.buttons = buttons;
            this.sticks = sticks;

            foreach( Control p in buttons ) {
                if( p == null ) continue;
                p.Visible = false;
            }
        }

        public void Update( byte[] packet )
        {
            reader.ReadFromPacket( packet );

            // Update buttons.
            for( int i = 0, max = reader.GetButtonCount() ; i < max && i < buttons.Length ; ++i ) {
                if( buttons[i] == null ) continue;
                buttons[i].Visible = reader.GetButtonState(i);
            }

            if( sticks == null ) return;

            // Update sticks, if there are any.
            for( int i = 0, max = reader.GetStickCount() ; i < max && i < sticks.Length ; ++i ) {
                ControllerStickState state = reader.GetStickState( i );
                sticks[i].display.Left = sticks[i].baseLeft + (int)( sticks[i].movementRadius * state.X );
                sticks[i].display.Top  = sticks[i].baseTop  - (int)( sticks[i].movementRadius * state.Y );
            }
        }
    }
}
