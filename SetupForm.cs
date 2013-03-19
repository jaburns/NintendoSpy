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
            string lastValue = ctlPorts.Text;

            ctlPorts.Items.Clear();
            ctlPorts.Text = "";
            ctlPorts.SelectedIndex = -1;

            string[] ports = SerialPort.GetPortNames();
            for( int i = 0; i < ports.Length; ++i ) {
                ctlPorts.Items.Add( ports[i] );
                if( lastValue == ports[i] ) {
                    ctlPorts.Text = lastValue;
                    ctlPorts.SelectedIndex = i;
                }
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
                case 3: view = new ViewNES ( port ); break;
            }

            if( view != null ) {
                btnGo.Enabled = false;
                view.Show();
                view.FormClosing += view_FormClosing;
            }
        }

        void view_FormClosing( object sender, FormClosingEventArgs e )
        {
            populatePortList();
            btnGo.Enabled = true;
            ((Form)sender).FormClosing -= view_FormClosing;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            populatePortList();
        }
    }
}
