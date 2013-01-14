using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace N64Spy
{
    public partial class ViewForm : Form
    {
        // Serial communication.
        private SerialPort datPort;
        private List<byte> localBuffer;
        private const int PACKET_SIZE = 34;
        private const int BUTTON_COUNT = 16;

        // Display state.
        private const float STICK_MOVE_RADIUS = 12;
        private PictureBox[] buttonDisplays;
        private int StickOriginX;
        private int StickOriginY;


        // The readPacket routine will produce one of these and pass it to the view handling code.
        private struct N64ControllerState
        {
            public bool[] buttons;
            public float analogX;
            public float analogY;
        }


    #region Initialization and clean up.

        public ViewForm( string comPort )
        {
            InitializeComponent();

            // Store a list of references to the form components representing the controller buttons in the order the come from the reader.
            buttonDisplays = new PictureBox[] {
                btn_A, btn_B, btn_Z, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright, null, null, btn_L, btn_R, btn_Cup, btn_Cdown, btn_Cleft, btn_Cright
            };

            // Hide all the button indicators by default.
            foreach( PictureBox p in buttonDisplays ) {
                if( p == null ) continue;
                p.Visible = false;
            }

            // Initialize our local read buffer.
            localBuffer = new List<byte>();

            // Check where the center of the joystick has bee placed.
            StickOriginX = stick.Left;
            StickOriginY = stick.Top;

            // Initialize our serial communications and start the read timer.
            datPort = new SerialPort( comPort, 115200 );
            datPort.Open();
            socketReadTimer.Enabled = true;
        }

        // When the window is closed, we want to close the serial port.
        private void frmView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( datPort != null ) {
                datPort.Close();
            }
        }

    #endregion


    #region Serial communication and signal processing

        // Fired at a regular interval to read from the COM port.
        private void socketReadTimer_Tick(object sender, EventArgs e)
        {
            // Read some data from the COM port and append it to our localBuffer.
            int readCount = datPort.BytesToRead;
            if( readCount < 1 ) return;
            byte[] readBuffer = new byte[ readCount ];
            datPort.Read( readBuffer, 0, readCount );
            datPort.DiscardInBuffer();
            localBuffer.AddRange( readBuffer );

            //
            int lastSentinelIndex = localBuffer.LastIndexOf( 0x0A );
            int tailSize = localBuffer.Count - lastSentinelIndex;

            // If we have enough data on the tail for a full read, process the end as current packet and wipe the local buffer.
            if( tailSize == PACKET_SIZE ) { 
                readPacket(
                    localBuffer.GetRange(
                        lastSentinelIndex, PACKET_SIZE
                    ).ToArray()
                );
                localBuffer.Clear();
            }
            // Otherwise, if we have enough buffer built up to do a read then do it and clear up to the last 
            else if( localBuffer.Count >= tailSize + PACKET_SIZE ) {
                readPacket(
                    localBuffer.GetRange(
                        lastSentinelIndex - PACKET_SIZE, PACKET_SIZE
                    ).ToArray()
                );
                localBuffer.RemoveRange( 0, lastSentinelIndex ); // Remove up to the last sentinel in the buffer.
            }
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

        // Given a valid N64 controller input packet, this routine creates an N64ControllerState and passes it to the updateDisplay method.
        private void readPacket( byte[] packet )
        {
            N64ControllerState controller = new N64ControllerState();

            // Read the button states.
            controller.buttons = new bool[ BUTTON_COUNT ];
            for( int i = 0 ; i < BUTTON_COUNT ; ++i ) {
                controller.buttons[i] = (packet[i+1] & 0x0F) != 0;
            }
            
            // Read the analog stick state.
            controller.analogX = readAnalogValue( packet, 1 + BUTTON_COUNT     );
            controller.analogY = readAnalogValue( packet, 1 + BUTTON_COUNT + 8 );
            
            // Need to invoke the update routine in a UI thread.
            BeginInvoke( new Action( () => { updateDisplay( controller ); } ) );
        }

    #endregion


    #region Display handling

        private void updateDisplay( N64ControllerState controller )
        {
            // Update buttons.
            for( int i = 0 ; i < BUTTON_COUNT ; ++i ) {
                if( buttonDisplays[i] == null ) continue;
                buttonDisplays[i].Visible = controller.buttons[i];
            }

            // Update stick.
            stick.Left = StickOriginX + (int)( STICK_MOVE_RADIUS * controller.analogX );
            stick.Top  = StickOriginY - (int)( STICK_MOVE_RADIUS * controller.analogY );
        }

    #endregion

    }
}
