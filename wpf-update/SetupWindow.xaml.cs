using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

using NintendoSpy.Readers;
using System.ComponentModel;

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

            _vm.Sources.UpdateContents (new SetupWindowViewModel.Source[] {
                new SetupWindowViewModel.Source { Name="NES", Tag="nes" },
                new SetupWindowViewModel.Source { Name="Super NES", Tag="snes" },
                new SetupWindowViewModel.Source { Name="Nintendo 64", Tag="n64" },
                new SetupWindowViewModel.Source { Name="GameCube", Tag="gcn" },
                new SetupWindowViewModel.Source { Name="PC 360", Tag="pc360" }
            });

            _portListUpdateTimer = new DispatcherTimer ();
            _portListUpdateTimer.Interval = TimeSpan.FromSeconds (1);
            _portListUpdateTimer.Tick += portListUpdateTimer_Tick;
            _portListUpdateTimer.Start ();
        }

        void portListUpdateTimer_Tick (object sender, EventArgs e) {
            _vm.Ports.UpdateContents (SerialPort.GetPortNames ());
        }

        void goButton_Click (object sender, RoutedEventArgs e) 
        {
            this.Hide ();
            new ViewWindow (_vm.Skins.SelectedItem, getReader ()) .ShowDialog ();
            this.Show ();
        }

        IControllerReader getReader () 
        {
            switch (_vm.Sources.SelectedItem.Tag) {
                case "nes":   return new SerialControllerReader (_vm.Ports.SelectedItem, new NES ());
                case "snes":  return new SerialControllerReader (_vm.Ports.SelectedItem, new SuperNES ());
                case "n64":   return new SerialControllerReader (_vm.Ports.SelectedItem, new Nintendo64 ());
                case "gcn":   return new SerialControllerReader (_vm.Ports.SelectedItem, new GameCube ());
                case "pc360": return new XInputReader ();
                default: throw new Exception ("No source selected");
            }
        }

        private void SourceSelectComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Sources.SelectedItem == null) return;
            _vm.ComPortOptionVisibility = _vm.Sources.SelectedItem.Tag != "pc360" ? Visibility.Visible : Visibility.Hidden;
        }
    }

    public class SetupWindowViewModel : INotifyPropertyChanged
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

        public class Source {
            public string Name { get; set; }
            public string Tag  { get; set; } // This should be an enum
        }

        public ListView <string> Ports   { get; set; }
        public ListView <Skin>   Skins   { get; set; }
        public ListView <Source> Sources { get; set; } 

        Visibility _comPortOptionVisibility;
        public Visibility ComPortOptionVisibility {
            get { return _comPortOptionVisibility; }
            set {
                _comPortOptionVisibility = value;
                NotifyPropertyChanged ("ComPortOptionVisibility");
            }
        }

        public SetupWindowViewModel () {
            Ports   = new ListView <string> ();
            Skins   = new ListView <Skin> ();
            Sources = new ListView <Source> ();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged (string prop) {
            if (PropertyChanged == null) return;
            PropertyChanged (this, new PropertyChangedEventArgs (prop));
        }
    }
}

