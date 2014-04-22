using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

using NintendoSpy.Readers;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace NintendoSpy
{
    public partial class SetupWindow : Window
    {
        SetupWindowViewModel _vm;
        DispatcherTimer _portListUpdateTimer;
        List <Skin> _skins;

        public SetupWindow ()
        {
            InitializeComponent ();
            _vm = new SetupWindowViewModel ();
            DataContext = _vm;

            if (! Directory.Exists ("skins")) {
                MessageBox.Show ("Could not find skins folder!", "NintendoSpy", MessageBoxButton.OK, MessageBoxImage.Error);
                Close ();
                return;
            }

            var results = Skin.LoadAllSkinsFromParentFolder ("skins");
            _skins = results.SkinsLoaded;

            if (results.ParseErrors.Count > 0) {
                showSkinParseErrors (results.ParseErrors);
            }

            _vm.Skins.UpdateContents (_skins.Where (x => x.Type == InputSource.DEFAULT));

            _vm.Sources.UpdateContents (InputSource.ALL);
            _vm.Sources.SelectedItem = InputSource.DEFAULT;

            _portListUpdateTimer = new DispatcherTimer ();
            _portListUpdateTimer.Interval = TimeSpan.FromSeconds (1);
            _portListUpdateTimer.Tick += (sender, e) => updatePortList ();
            _portListUpdateTimer.Start ();

            updatePortList ();
            _vm.Ports.SelectFirst ();
        }

        void showSkinParseErrors (List <string> errs) {
            StringBuilder msg = new StringBuilder ();
            msg.AppendLine ("Some skins were unable to be parsed:");
            foreach (var err in errs) msg.AppendLine (err);
            MessageBox.Show (msg.ToString (), "NintendoSpy", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void updatePortList () {
            _vm.Ports.UpdateContents (SerialPort.GetPortNames ());
        }

        void goButton_Click (object sender, RoutedEventArgs e) 
        {
            this.Hide ();

            try {
                new ViewWindow (_vm.Skins.SelectedItem,
                                _vm.Backgrounds.SelectedItem, 
                                _vm.Sources.SelectedItem.BuildReader (_vm.Ports.SelectedItem))
                    .ShowDialog ();
            }
            catch (Exception ex) {
                MessageBox.Show (ex.Message, "NintendoSpy", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Show ();
        }

        private void SourceSelectComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Sources.SelectedItem == null) return;
            _vm.ComPortOptionVisibility = _vm.Sources.SelectedItem.RequiresComPort ? Visibility.Visible : Visibility.Hidden;
            _vm.Skins.UpdateContents (_skins.Where (x => x.Type == _vm.Sources.SelectedItem));
            _vm.Skins.SelectFirst ();
        }

        private void Skin_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Skins.SelectedItem == null) return;
            _vm.Backgrounds.UpdateContents (_vm.Skins.SelectedItem.Backgrounds);
            _vm.Backgrounds.SelectFirst ();
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
            
            public void SelectFirst () {
                if (_items.Count > 0) SelectedItem = _items [0];
            }
        }

        public ListView <string> Ports { get; set; }
        public ListView <Skin> Skins { get; set; }
        public ListView <Skin.Background> Backgrounds { get; set; }
        public ListView <InputSource> Sources { get; set; }

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
            Sources = new ListView <InputSource> ();
            Backgrounds = new ListView <Skin.Background> ();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged (string prop) {
            if (PropertyChanged == null) return;
            PropertyChanged (this, new PropertyChangedEventArgs (prop));
        }
    }
}

