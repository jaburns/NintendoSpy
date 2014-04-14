using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NintendoSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            populatePortList ();
            addSkin ("Default N64");
            addSkin ("Default GameCube");
        }

        void populatePortList()
        {
            var lastValue = "";
            if (portList.SelectedItem != null) {
                lastValue = (portList.SelectedItem as ComboBoxItem).Content as String;
            }

            portList.Items.Clear();
            portList.SelectedIndex = -1;

            string[] ports = SerialPort.GetPortNames();
            for( int i = 0; i < ports.Length; ++i ) 
            {
                var newItem = new ComboBoxItem ();
                newItem.Content = ports[i];
                portList.Items.Add (newItem);

                if (lastValue == ports[i]) {
                    portList.SelectedIndex = i;
                }
            }

            if (portList.SelectedIndex < 0) portList.SelectedIndex = 0;
        }

        void addSkin (string name) 
        {
            var item = new ListBoxItem ();
            item.Content = name;
            item.Foreground = new SolidColorBrush (Color.FromArgb (0xFF,0xCB,0xCB,0xCB));
            skinsList.Items.Add (item);
        }
    }
}
