using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace N64Spy
{
    public partial class ViewN64 : Form
    {
        private SerialMonitor serialMonitor;
        private DisplayManager displayManager;

        public ViewN64( string comPort )
        {
            InitializeComponent();

            displayManager = new DisplayManager
            (
                new ControllerReader_N64(),

                new Control[] {
                    btn_A, btn_B, btn_Z, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright, null, null, btn_L, btn_R, btn_Cup, btn_Cdown, btn_Cleft, btn_Cright
                },

                new DisplayStick[] {
                    new DisplayStick() {
                        display = stick,
                        movementRadius = 12
                    }
                }
            );

            // Initialize our serial communications and start the read timer.
            serialMonitor = new SerialMonitor( comPort, 115200, 34 );
            serialMonitor.PacketReceived += serialMonitor_PacketReceived;
            serialMonitor.Start();
        }

        private void serialMonitor_PacketReceived(object sender, byte[] packet)
        {
            BeginInvoke( new Action( () => { displayManager.Update( packet ); } ) );
        }

        // When the window is closed, we want to close the serial port.
        private void frmView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( serialMonitor != null ) {
                serialMonitor.Stop();
                serialMonitor = null;
            }
        }
    }
}
