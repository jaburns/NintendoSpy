using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace NintendoSpy
{
    public partial class SetupWindow : Window
    {
        SetupWindowViewModel _vm;
        DispatcherTimer _portListUpdateTimer;

        public SetupWindow()
        {
            InitializeComponent();
            _vm = new SetupWindowViewModel ();
            DataContext = _vm;

            _vm.AddSkin (new SetupWindowViewModel.SkinEntry { Name = "Default N64" });
            _vm.AddSkin (new SetupWindowViewModel.SkinEntry { Name = "Default GameCube" });

            _portListUpdateTimer = new DispatcherTimer ();
            _portListUpdateTimer.Interval = TimeSpan.FromSeconds (1);
            _portListUpdateTimer.Tick += portListUpdateTimer_Tick;
            _portListUpdateTimer.Start ();
        }

        void portListUpdateTimer_Tick (object sender, EventArgs e) {
            _vm.UpdatePortList (SerialPort.GetPortNames ());
        }

        void goButton_Click (object sender, RoutedEventArgs e) {

        }
    }

    public class SetupWindowViewModel 
    {
        public struct SkinEntry {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        List <string> _ports = new List <String> ();
        List <SkinEntry> _skins = new List <SkinEntry> ();

        CollectionView _portsView;
        CollectionView _skinsView;

        public CollectionView Ports { get { return _portsView; } }
        public CollectionView Skins { get { return _skinsView; } }

        public string SelectedPort { get; set; }
        public string SelectedSkinPath { get; set; }

        public SetupWindowViewModel () {
            _portsView = new CollectionView (_ports);
            _skinsView = new CollectionView (_skins);
        }

        public void UpdatePortList (IEnumerable <string> ports) {
            _ports.Clear ();
            _ports.AddRange (ports);
            _portsView.Refresh ();
        }

        public void AddSkin (SkinEntry skin) {
            _skins.Add (skin);
            _skinsView.Refresh ();
        }
    }
}

