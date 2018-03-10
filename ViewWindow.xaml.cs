using System;
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
using System.ComponentModel;
using System.Windows.Shapes;

using NintendoSpy.Readers;

namespace NintendoSpy
{
    public partial class ViewWindow : Window, INotifyPropertyChanged
    {
        Skin _skin;
        IControllerReader _reader;
        Keybindings _keybindings;
        BlinkReductionFilter _blinkFilter = new BlinkReductionFilter ();

        List <Tuple <Skin.Button,Image>> _buttonsWithImages = new List <Tuple <Skin.Button,Image>> ();
        List <Tuple <Skin.RangeButton,Image>> _rangeButtonsWithImages = new List <Tuple <Skin.RangeButton,Image>> ();
        List <Tuple <Skin.AnalogStick,Image>> _sticksWithImages = new List <Tuple <Skin.AnalogStick,Image>> ();

        // The triggers images are embedded inside of a Grid element so that we can properly mask leftwards and upwards
        // without the image aligning to the top left of its element.
        List <Tuple <Skin.AnalogTrigger,Grid>> _triggersWithGridImages = new List <Tuple <Skin.AnalogTrigger,Grid>> ();


        /// Expose the enabled status of the low-pass filter for data binding.
        public bool ButtonBlinkReductionEnabled {
            get { return _blinkFilter.ButtonEnabled; }
            set {

                _blinkFilter.ButtonEnabled = value;
                OnPropertyChanged ("ButtonBlinkReductionEnabled");
            }
        }
        public bool MassBlinkReductionEnabled
        {
            get { return _blinkFilter.MassEnabled; }
            set {
                _blinkFilter.MassEnabled = value;
                OnPropertyChanged("MassBlinkReductionEnabled");
            }
        }
        public bool AnalogBlinkReductionEnabled
        {
            get { return _blinkFilter.AnalogEnabled; }
            set {
                _blinkFilter.AnalogEnabled = value;
                OnPropertyChanged("AnalogBlinkReductionEnabled");
            }
        }
        public bool AllBlinkReductionEnabled
        {
            get { return ButtonBlinkReductionEnabled && AnalogBlinkReductionEnabled && MassBlinkReductionEnabled; }
            set {
                ButtonBlinkReductionEnabled = AnalogBlinkReductionEnabled = MassBlinkReductionEnabled = value;
                OnPropertyChanged("AllBlinkReductionEnabled");
            }
        }

        public ViewWindow (Skin skin, Skin.Background skinBackground, IControllerReader reader)
        {
            InitializeComponent ();
            DataContext = this;

            _skin = skin;
            _reader = reader;

            Title = skin.Name;

            ControllerGrid.Width = skinBackground.Width;
            ControllerGrid.Height = skinBackground.Height;
            var brush = new SolidColorBrush(skinBackground.Color);
            ControllerGrid.Background = brush;

            if (skinBackground.Image != null)
            {
                var img = new Image();
                img.VerticalAlignment = VerticalAlignment.Top;
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.Source = skinBackground.Image;
                img.Stretch = Stretch.Uniform;
                img.Margin = new Thickness(0, 0, 0, 0);
                img.Width = skinBackground.Image.PixelWidth;
                img.Height = skinBackground.Image.PixelHeight;
                ControllerGrid.Children.Add(img);
            }

            foreach (var detail in _skin.Details)
            {
                if (bgIsActive(skinBackground.Name, detail.Config.TargetBackgrounds, detail.Config.IgnoreBackgrounds))
                {
                    
                    var image = getImageForElement(detail.Config);
                    ControllerGrid.Children.Add(image);
                }
            }

            foreach (var trigger in _skin.AnalogTriggers) {
                if (bgIsActive(skinBackground.Name, trigger.Config.TargetBackgrounds, trigger.Config.IgnoreBackgrounds))
                {
                    var grid = getGridForAnalogTrigger(trigger);
                    _triggersWithGridImages.Add(new Tuple<Skin.AnalogTrigger, Grid>(trigger, grid));
                    ControllerGrid.Children.Add(grid);
                }
            }

            foreach (var button in _skin.Buttons) {
                if (bgIsActive(skinBackground.Name, button.Config.TargetBackgrounds, button.Config.IgnoreBackgrounds))
                {
                    var image = getImageForElement(button.Config);
                    _buttonsWithImages.Add(new Tuple<Skin.Button, Image>(button, image));
                    image.Visibility = Visibility.Hidden;
                    ControllerGrid.Children.Add(image);
                }
            }

            foreach (var button in _skin.RangeButtons) {
                if (bgIsActive(skinBackground.Name, button.Config.TargetBackgrounds, button.Config.IgnoreBackgrounds))
                {
                    var image = getImageForElement(button.Config);
                    _rangeButtonsWithImages.Add(new Tuple<Skin.RangeButton, Image>(button, image));
                    image.Visibility = Visibility.Hidden;
                    ControllerGrid.Children.Add(image);
                }
            }
            
            foreach (var stick in _skin.AnalogSticks) {
                if (bgIsActive(skinBackground.Name, stick.Config.TargetBackgrounds, stick.Config.IgnoreBackgrounds))
                {
                    var image = getImageForElement(stick.Config);
                    _sticksWithImages.Add(new Tuple<Skin.AnalogStick, Image>(stick, image));
                    ControllerGrid.Children.Add(image);
                }
            }

            _reader.ControllerStateChanged += reader_ControllerStateChanged;
            _reader.ControllerDisconnected += reader_ControllerDisconnected;

            try {
                _keybindings = new Keybindings (Keybindings.XML_FILE_PATH, _reader);
            } catch (ConfigParseException) {
                MessageBox.Show ("Error parsing keybindings.xml. Not binding any keys to gamepad inputs");
            }

            MassBlinkReductionEnabled = Properties.Settings.Default.MassFilter;
            AnalogBlinkReductionEnabled = Properties.Settings.Default.AnalogFilter;
            ButtonBlinkReductionEnabled = Properties.Settings.Default.ButtonFilter;
            Topmost = Properties.Settings.Default.TopMost;
        }


        static bool bgIsActive(string bgName, List<string> eligableBgs, List<string> ignoreBgs)
        {
            if (ignoreBgs.Contains(bgName))
            {
                return false;
            }
            return eligableBgs.Count == 0 || eligableBgs.Contains(bgName);
        }
        static Image getImageForElement (Skin.ElementConfig config)
        {
            var img = new Image ();
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;
            img.Source = config.Image;
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness (config.X, config.Y, 0, 0);
            img.Width = config.Width;
            img.Height = config.Height;
            return img;
        }

        static Grid getGridForAnalogTrigger (Skin.AnalogTrigger trigger)
        {
            var img = new Image ();
            img.VerticalAlignment = VerticalAlignment.Top;

            img.HorizontalAlignment = 
                  trigger.Direction == Skin.AnalogTrigger.DirectionValue.Left
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;

            img.VerticalAlignment = 
                  trigger.Direction == Skin.AnalogTrigger.DirectionValue.Up
                ? VerticalAlignment.Bottom
                : VerticalAlignment.Top;

            img.Source = trigger.Config.Image;
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness (0, 0, 0, 0);
            img.Width = trigger.Config.Width;
            img.Height = trigger.Config.Height;
            

            var grid = new Grid ();

            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.Margin = new Thickness (trigger.Config.X, trigger.Config.Y, 0, 0);
            grid.Width = trigger.Config.Width;
            grid.Height = trigger.Config.Height;

            grid.Children.Add (img);

            return grid;
        }

        void AlwaysOnTop_Click (object sender, RoutedEventArgs e) {
            this.Topmost = !this.Topmost;
            Properties.Settings.Default.TopMost = Topmost;
        }
        void AllBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (this.ButtonBlinkReductionEnabled && this.AnalogBlinkReductionEnabled && this.MassBlinkReductionEnabled)
            {
                this.AllBlinkReductionEnabled = false;

            }
            else
            {
                this.AllBlinkReductionEnabled = true;
            }
            Properties.Settings.Default.ButtonFilter = ButtonBlinkReductionEnabled;
            Properties.Settings.Default.AnalogFilter = AnalogBlinkReductionEnabled;
            Properties.Settings.Default.MassFilter = MassBlinkReductionEnabled;
        }
        void ButtonBlinkReductionEnabled_Click (object sender, RoutedEventArgs e) {
            this.ButtonBlinkReductionEnabled = !this.ButtonBlinkReductionEnabled;
            Properties.Settings.Default.ButtonFilter = ButtonBlinkReductionEnabled;
        }
        void AnalogBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            this.AnalogBlinkReductionEnabled = !this.AnalogBlinkReductionEnabled;
            Properties.Settings.Default.AnalogFilter = AnalogBlinkReductionEnabled;
        }
        void MassBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            this.MassBlinkReductionEnabled = !this.MassBlinkReductionEnabled;
            Properties.Settings.Default.MassFilter = MassBlinkReductionEnabled;
        }


        void reader_ControllerDisconnected (object sender, EventArgs e)
        {
            Close ();
        }

        void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (_keybindings != null) {
                _keybindings.Finish ();
            }
            _reader.Finish ();
        }

        void reader_ControllerStateChanged (IControllerReader reader, ControllerState newState)
        {
            newState = _blinkFilter.Process (newState);

            foreach (var button in _buttonsWithImages) 
            {
                if (!newState.Buttons.ContainsKey (button.Item1.Name)) continue;

                button.Item2.Visibility = newState.Buttons [button.Item1.Name] ? Visibility.Visible : Visibility.Hidden ;
            }

            foreach (var button in _rangeButtonsWithImages) 
            {
                if (!newState.Analogs.ContainsKey (button.Item1.Name)) continue;

                var value = newState.Analogs [button.Item1.Name];
                var visible = button.Item1.From <= value && value <= button.Item1.To;

                button.Item2.Visibility = visible ? Visibility.Visible : Visibility.Hidden ;
            }

            foreach (var stick in _sticksWithImages)
            {
                var skin = stick.Item1;
                var image = stick.Item2;

                float xrange = (skin.XReverse ? -1 :  1) * skin.XRange;
                float yrange = (skin.YReverse ?  1 : -1) * skin.YRange;

                var x = newState.Analogs.ContainsKey (skin.XName)
                      ? skin.Config.X + xrange * newState.Analogs [skin.XName]
                      : skin.Config.X ;

                var y = newState.Analogs.ContainsKey (skin.YName)
                      ? skin.Config.Y + yrange * newState.Analogs [skin.YName]
                      : skin.Config.Y ;
                
                image.Margin = new Thickness (x,y,0,0);
            }
            
            foreach (var trigger in _triggersWithGridImages)
            {
                var skin = trigger.Item1;
                var grid = trigger.Item2;

                if (!newState.Analogs.ContainsKey (skin.Name)) continue;

                var val = newState.Analogs [skin.Name];
                if (skin.UseNegative) val *= -1;
                if (skin.IsReversed) val = 1 - val;
                if (val < 0) val = 0;
                switch (skin.Direction) 
                {
                    case Skin.AnalogTrigger.DirectionValue.Right:
                        grid.Width = skin.Config.Width * val;
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Left:
                        var width = skin.Config.Width * val;
                        var offx = skin.Config.Width - width;
                        grid.Margin = new Thickness (skin.Config.X + offx, skin.Config.Y, 0, 0);
                        grid.Width = width;
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Down:
                        grid.Height = skin.Config.Height * val;
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Up:
                        var height = skin.Config.Height * val;
                        var offy = skin.Config.Height - height;
                        grid.Margin = new Thickness (skin.Config.X, skin.Config.Y + offy, 0, 0);
                        grid.Height = height;
                        break;
                }
            }
        }

        // INotifyPropertyChanged interface implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged (string propertyName) {
            if (PropertyChanged != null) PropertyChanged (this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
