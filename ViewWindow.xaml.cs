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
        public bool BlinkReductionEnabled {
            get { return _blinkFilter.Enabled; }
            set { _blinkFilter.Enabled = value;  OnPropertyChanged ("BlinkReductionEnabled"); }
        }



        public ViewWindow (Skin skin, Skin.Background skinBackground, IControllerReader reader)
        {
            InitializeComponent ();
            DataContext = this;

            _skin = skin;
            _reader = reader;

            ControllerGrid.Width = skinBackground.Image.PixelWidth;
            ControllerGrid.Height = skinBackground.Image.PixelHeight;

            var brush = new ImageBrush (skinBackground.Image);
            brush.Stretch = Stretch.Uniform;
            ControllerGrid.Background = brush;

            foreach (var trigger in _skin.AnalogTriggers) {
                var grid = getGridForAnalogTrigger (trigger);
                _triggersWithGridImages.Add (new Tuple <Skin.AnalogTrigger,Grid> (trigger, grid));
                ControllerGrid.Children.Add (grid);
            }

            foreach (var button in _skin.Buttons) {
                var image = getImageForElement (button.Config);
                _buttonsWithImages.Add (new Tuple <Skin.Button,Image> (button, image));
                image.Visibility = Visibility.Hidden;
                ControllerGrid.Children.Add (image);
            }

            foreach (var button in _skin.RangeButtons) {
                var image = getImageForElement (button.Config);
                _rangeButtonsWithImages.Add (new Tuple <Skin.RangeButton,Image> (button, image));
                image.Visibility = Visibility.Hidden;
                ControllerGrid.Children.Add (image);
            }
            
            foreach (var stick in _skin.AnalogSticks) {
                var image = getImageForElement (stick.Config);
                _sticksWithImages.Add (new Tuple <Skin.AnalogStick,Image> (stick, image));
                ControllerGrid.Children.Add (image);
            }

            _reader.ControllerStateChanged += reader_ControllerStateChanged;
            _reader.ControllerDisconnected += reader_ControllerDisconnected;

            try {
                _keybindings = new Keybindings (Keybindings.XML_FILE_PATH, _reader);
            } catch (ConfigParseException) {
                MessageBox.Show ("Error parsing keybindings.xml. Not binding any keys to gamepad inputs");
            }
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
            img.Stretch = Stretch.None;
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
        }

        void BlinkReductionEnabledEnabled_Click (object sender, RoutedEventArgs e) {
            this.BlinkReductionEnabled = !this.BlinkReductionEnabled;
        }


        void reader_ControllerDisconnected (object sender, EventArgs e)
        {
            Close ();
        }

        void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
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
