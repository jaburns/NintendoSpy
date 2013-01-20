using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace N64Spy
{
    public partial class ViewSnes : Form
    {
        private SerialMonitor serialMonitor;
        private DisplayManager displayManager;

        public ViewSnes( string comPort )
        {
            InitializeComponent();

            displayManager = new DisplayManager
            (
                new ControllerReader_SNES(),

                new Control[] {
                    btn_B, btn_Y, btn_Select, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright, btn_A, btn_X, btn_L, btn_R, null, null, null, null
                },

                null
            );

            // Initialize our serial communications and start the read timer.
            serialMonitor = new SerialMonitor( comPort, 115200, 18 );
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
