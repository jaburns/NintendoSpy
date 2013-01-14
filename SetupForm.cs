using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace N64Spy
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
            (new ViewForm( ctlPorts.SelectedItem.ToString() )).Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            populatePortList();
        }
    }
}
