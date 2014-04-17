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

        public SetupWindow ()
        {
            InitializeComponent ();
            _vm = new SetupWindowViewModel ();
            DataContext = _vm;

            _vm.Skins.UpdateContents (Skin.LoadAllSkinsFromParentFolder ("skins"));

            _portListUpdateTimer = new DispatcherTimer ();
            _portListUpdateTimer.Interval = TimeSpan.FromSeconds (1);
            _portListUpdateTimer.Tick += portListUpdateTimer_Tick;
            _portListUpdateTimer.Start ();
        }

        void portListUpdateTimer_Tick (object sender, EventArgs e) {
            _vm.Ports.UpdateContents (SerialPort.GetPortNames ());
        }

        void goButton_Click (object sender, RoutedEventArgs e) {
            new ViewWindow (_vm.Skins.SelectedItem) .Show ();
        }
    }

    public class SetupWindowViewModel 
    {
        public class ListView <T>
        {
            List <T> _items;

            public CollectionView Items { get; private set; }
            public T SelectedItem { get; set; }

            public ListView () {
                _items = new List <T> ();
                Items = new CollectionView (_items);
            }

            public void UpdateContents (IEnumerable <T> items) {
                _items.Clear ();
                _items.AddRange (items);
                Items.Refresh ();
            }
        }

        public ListView <string> Ports { get; set; }
        public ListView <Skin>   Skins { get; set; }

        public SetupWindowViewModel () {
            Ports = new ListView <string> ();
            Skins = new ListView <Skin> ();
        }
    }
}

