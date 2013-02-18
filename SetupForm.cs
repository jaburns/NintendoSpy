using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace NintendoSpy
{
    // This simple form allows the user to select a COM port to open the N64 spy on.
    // It is the starting GUI of the application.
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();
            populatePortList();
        }

        private void populatePortList()
        {
            ctlPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            for( int i = 0; i < ports.Length; ++i ) {
                ctlPorts.Items.Add(ports[i]);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if( ctlPorts.SelectedItem == null ) return;
            string port = ctlPorts.SelectedItem.ToString();

            Form view = null;
            switch( ctlConsole.SelectedIndex ) {
                case 0: view = new ViewSnes( port ); break;
                case 1: view = new ViewN64 ( port ); break;
                case 2: view = new ViewGC  ( port ); break;
            }

            if( view != null ) {
                view.Show();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            populatePortList();
        }
    }
}
