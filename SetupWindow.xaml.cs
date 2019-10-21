using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

using RetroSpy.Readers;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace RetroSpy
{
    public partial class SetupWindow : Window
    {
        SetupWindowViewModel _vm;
        DispatcherTimer _portListUpdateTimer;
        DispatcherTimer _xiAndGamepadListUpdateTimer;
        List <Skin> _skins;

        public SetupWindow ()
        {
            InitializeComponent ();
            _vm = new SetupWindowViewModel ();
            DataContext = _vm;

            if (! Directory.Exists ("skins")) {
                MessageBox.Show ("Could not find skins folder!", "RetroSpy", MessageBoxButton.OK, MessageBoxImage.Error);
                Close ();
                return;
            }

            var results = Skin.LoadAllSkinsFromParentFolder ("skins");
            _skins = results.SkinsLoaded;

            _vm.Skins.UpdateContents (_skins.Where (x => x.Type == InputSource.DEFAULT));
            
            _vm.Sources.UpdateContents (InputSource.ALL);
            

            _vm.DelayInMilliseconds = Properties.Settings.Default.Delay;

            _vm.StaticViewerWindowName = Properties.Settings.Default.StaticViewerWindowName;

            _portListUpdateTimer = new DispatcherTimer ();
            _portListUpdateTimer.Interval = TimeSpan.FromSeconds (1);
            _portListUpdateTimer.Tick += (sender, e) => updatePortList ();
            _portListUpdateTimer.Start ();

            _xiAndGamepadListUpdateTimer = new DispatcherTimer();
            _xiAndGamepadListUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            _xiAndGamepadListUpdateTimer.Tick += (sender, e) =>
            {
                if (_vm.Sources.SelectedItem == InputSource.PAD || _vm.Sources.SelectedItem == InputSource.PADATOD)
                {
                    updateGamepadList();
                }
                else if (_vm.Sources.SelectedItem == InputSource.PC360)
                {
                    updateXIList();
                }
                //else if (_vm.Sources.SelectedItem == InputSource.XBOX)
                //{
                //    updateBeagleList();
                //}
                //else if (_vm.Sources.SelectedItem == InputSource.WII)
                //{
                //    updateBeagleI2CList();
                //}
            };
            _xiAndGamepadListUpdateTimer.Start();

            updatePortList ();
            _vm.Ports.SelectIdFromText(Properties.Settings.Default.Port);
            _vm.Ports2.SelectIdFromText(Properties.Settings.Default.Port2);
            _vm.XIAndGamepad.SelectFirst();
            _vm.Sources.SelectId(Properties.Settings.Default.Source);
            _vm.Skins.SelectId(Properties.Settings.Default.Skin);
            _vm.Hostname = Properties.Settings.Default.Hostname;

            if (results.ParseErrors.Count > 0)
            {
                showSkinParseErrors(results.ParseErrors);
            }
        }

        void showSkinParseErrors (List <string> errs) {
            StringBuilder msg = new StringBuilder ();
            msg.AppendLine ("Some skins were unable to be parsed:");
            foreach (var err in errs) msg.AppendLine (err);
            MessageBox.Show (msg.ToString (), "RetroSpy", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void updatePortList () {
            var ports = SerialPort.GetPortNames();
            _vm.Ports.UpdateContents (ports);
            var ports2 = new string[ports.Length + 1];
            ports2[0] = "Not Connected";
            for (int i = 0; i < ports.Length; ++i)
                ports2[i + 1] = ports[i];
            _vm.Ports2.UpdateContents(ports2);
        }

        void updateGamepadList()
        {
            _vm.XIAndGamepad.UpdateContents(GamepadReader.GetDevices());
        }

        void updateXIList()
        {
            _vm.XIAndGamepad.UpdateContents(XInputReader.GetDevices());
        }

        void updateBeagleList()
        {
            _vm.XIAndGamepad.UpdateContents(XboxReader.GetDevices());
        }

        void updateBeagleI2CList()
        {
            _vm.XIAndGamepad.UpdateContents(WiiReaderV1.GetDevices());
        }

        void goButton_Click (object sender, RoutedEventArgs e) 
        {
            this.Hide ();
            Properties.Settings.Default.Port = _vm.Ports.SelectedItem;
            Properties.Settings.Default.Port2 = _vm.Ports2.SelectedItem;
            Properties.Settings.Default.Source = _vm.Sources.GetSelectedId();
            Properties.Settings.Default.Skin = _vm.Skins.GetSelectedId();
            Properties.Settings.Default.Delay = _vm.DelayInMilliseconds;
            Properties.Settings.Default.Background = _vm.Backgrounds.GetSelectedId();
            Properties.Settings.Default.Hostname = _vm.Hostname;
            Properties.Settings.Default.StaticViewerWindowName = _vm.StaticViewerWindowName;
            Properties.Settings.Default.Save();

            try
            {
                IControllerReader reader;
                if (_vm.Sources.SelectedItem == InputSource.PAD || _vm.Sources.SelectedItem == InputSource.PADATOD)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                }
                else if (_vm.Sources.SelectedItem == InputSource.PC360)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                }
                else if (_vm.Sources.SelectedItem == InputSource.XBOX || _vm.Sources.SelectedItem == InputSource.PSCLASSIC ||
                         _vm.Sources.SelectedItem == InputSource.SWITCH || _vm.Sources.SelectedItem == InputSource.XBOX360 ||
                         _vm.Sources.SelectedItem == InputSource.GENMINI || _vm.Sources.SelectedItem == InputSource.C64MINI ||
                         _vm.Sources.SelectedItem == InputSource.NEOGEOMINI || _vm.Sources.SelectedItem == InputSource.PS3 
                         || _vm.Sources.SelectedItem == InputSource.PS4)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(txtHostname.Text);
                }
                else if (_vm.Sources.SelectedItem == InputSource.PADDLES)
                {
                    if (_vm.Ports.SelectedItem == _vm.Ports2.SelectedItem)
                        throw new Exception("Port 1 and Port 2 cannot be the same!");
                    reader = _vm.Sources.SelectedItem.BuildReader2(_vm.Ports.SelectedItem, _vm.Ports2.SelectedItem);
                }
                //else if (_vm.Sources.SelectedItem == InputSource.XBOX)
                //{
                //    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                //}
                //else if (_vm.Sources.SelectedItem == InputSource.WII)
                //{
                //    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                //}
                else {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.Ports.SelectedItem);
                }
                if (_vm.DelayInMilliseconds > 0)
                    reader = new DelayedControllerReader(reader, _vm.DelayInMilliseconds);

                new ViewWindow (_vm.Skins.SelectedItem,
                                _vm.Backgrounds.SelectedItem, 
                                reader, _vm.StaticViewerWindowName)
                    .ShowDialog ();
            }
#if DEBUG
            catch (ConfigParseException ex) {
#else
            catch (Exception ex) {
#endif
                MessageBox.Show (ex.Message, "RetroSpy", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Show ();
        }

        private void SourceSelectComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Sources.SelectedItem == null) return;
            _vm.ComPortOptionVisibility = _vm.Sources.SelectedItem.RequiresComPort ? Visibility.Visible : Visibility.Hidden;
            _vm.ComPort2OptionVisibility = _vm.Sources.SelectedItem.RequiresComPort2 ? Visibility.Visible : Visibility.Hidden;
            _vm.XIAndGamepadOptionVisibility = _vm.Sources.SelectedItem.RequiresId ? Visibility.Visible : Visibility.Hidden;
            _vm.SSHOptionVisibility = _vm.Sources.SelectedItem.RequiresHostname ? Visibility.Visible : Visibility.Hidden;
            updateGamepadList();
            updateXIList();
            updatePortList();
            updateBeagleList();
            updateBeagleI2CList();
            _vm.Skins.UpdateContents (_skins.Where (x => x.Type == _vm.Sources.SelectedItem));
            _vm.Skins.SelectFirst ();
            if(_vm.Sources.GetSelectedId() == Properties.Settings.Default.Source)
            {
                _vm.Skins.SelectId(Properties.Settings.Default.Skin);
            }
        }

        private void Skin_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Skins.SelectedItem == null) return;
            _vm.Backgrounds.UpdateContents (_vm.Skins.SelectedItem.Backgrounds);
            _vm.Backgrounds.SelectFirst ();
            if (_vm.Skins.GetSelectedId() == Properties.Settings.Default.Skin)
            {
                _vm.Backgrounds.SelectId(Properties.Settings.Default.Background);
            }
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

            public void SelectId(int id)
            {
                if (_items.Count > 0 && id >= 0 && id < _items.Count)
                {
                    SelectedItem = _items[id];
                }
                else
                {
                    SelectFirst();
                }
            }

            public void SelectIdFromText(T text)
            {
                int index = _items.IndexOf(text);
                SelectId(index);
            }

            public int GetSelectedId()
            {
                if( SelectedItem != null)
                {
                    return _items.IndexOf(SelectedItem);
                }
                return -1;
            }
        }

        public ListView <string> Ports { get; set; }
        public ListView<string> Ports2 { get; set; }
        public ListView <uint> XIAndGamepad { get; set; }
        public ListView <Skin> Skins { get; set; }
        public ListView <Skin.Background> Backgrounds { get; set; }
        public ListView <InputSource> Sources { get; set; }
        public int DelayInMilliseconds { get; set; }
        public bool StaticViewerWindowName { get; set; }
        public string Hostname { get; set; }

        Visibility _comPortOptionVisibility;
        public Visibility ComPortOptionVisibility {
            get { return _comPortOptionVisibility; }
            set {
                _comPortOptionVisibility = value;
                NotifyPropertyChanged ("ComPortOptionVisibility");
            }
        }

        Visibility _comPort2OptionVisibility;
        public Visibility ComPort2OptionVisibility
        {
            get { return _comPort2OptionVisibility; }
            set
            {
                _comPort2OptionVisibility = value;
                NotifyPropertyChanged("ComPort2OptionVisibility");
            }
        }

        Visibility _XIAndGamepadOptionVisibility;
        public Visibility XIAndGamepadOptionVisibility
        {
            get { return _XIAndGamepadOptionVisibility; }
            set
            {
                _XIAndGamepadOptionVisibility = value;
                NotifyPropertyChanged("XIAndGamepadOptionVisibility");
            }
        }

        Visibility _SSHOptionVisibility;
        public Visibility SSHOptionVisibility
        {
            get { return _SSHOptionVisibility; }
            set
            {
                _SSHOptionVisibility = value;
                NotifyPropertyChanged("SSHOptionVisibility");
            }
        }

        public SetupWindowViewModel () {
            Ports   = new ListView <string> ();
            Ports2 = new ListView<string>();
            XIAndGamepad = new ListView<uint>();
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

