using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

using RetroSpy.Readers;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace RetroSpy
{
    public partial class ViewWindow : Window, INotifyPropertyChanged
    {
        public void calculateMagic()
        {
            _magicHeight = Height - _originalHeight;
            _magicWidth = Width - _originalWidth;
        }

        public double GetMagicHeight()
        {
            return _magicHeight;
        }

        public double GetMagicWidth()
        {
            return _magicWidth;
        }

        public double GetHeight()
        {
            return Height - _magicHeight;
        }

        public double GetWidth()
        {
            return Width - _magicWidth;
        }

        public double GetRatio()
        {
            return _originalWidth / _originalHeight;
        }

        private void AdjustImage(Skin.ElementConfig config, Image image, double xRatio, double yRatio)
        {
            uint newX = config.X = (uint)Math.Round(config.OriginalX * xRatio);
            uint newY = config.Y = (uint)Math.Round(config.OriginalY * yRatio);

            int newWidth = (int)Math.Round(config.Width * xRatio);
            int newHeight = (int)Math.Round(config.Height * yRatio);

            image.Margin = new Thickness(newX, newY, 0, 0);
            image.Width = newWidth;
            image.Height = newHeight;
        }

        private void AdjustGrid(Skin.ElementConfig config, Grid grid, double xRatio, double yRatio)
        {
            uint newX = config.X = (uint)Math.Round(config.OriginalX * xRatio);
            uint newY = config.Y = (uint)Math.Round(config.OriginalY * yRatio);

            uint newWidth = config.Width = (uint)Math.Round(config.OriginalWidth * xRatio);
            uint newHeight = config.Height = (uint)Math.Round(config.OriginalHeight * yRatio);

            ((Image)grid.Children[0]).Width = newWidth;
            ((Image)grid.Children[0]).Height = newHeight;

            grid.Margin = new Thickness(newX, newY, 0, 0);
            grid.Width = newWidth;
            grid.Height = newHeight;
        }

        public void AdjustControllerElements()
        {
            ControllerGrid.Width = ((Image)ControllerGrid.Children[0]).Width = GetWidth();
            ControllerGrid.Height = ((Image)ControllerGrid.Children[0]).Height = GetHeight();
            double xRatio = GetWidth() / _originalWidth;
            double yRatio = GetHeight() / _originalHeight;


            foreach (var detail in _detailsWithImages)
            {
                AdjustImage(detail.Item1.Config, detail.Item2, xRatio, yRatio);
            }

            foreach (var trigger in _triggersWithGridImages)
            {
                AdjustGrid(trigger.Item1.Config, trigger.Item2, xRatio, yRatio);
            }

            foreach (var button in _buttonsWithImages)
            {
                AdjustImage(button.Item1.Config, button.Item2, xRatio, yRatio);
            }

            foreach (var button in _rangeButtonsWithImages)
            {
                AdjustImage(button.Item1.Config, button.Item2, xRatio, yRatio);
            }

            foreach (var stick in _sticksWithImages)
            {
                AdjustImage(stick.Item1.Config, stick.Item2, xRatio, yRatio);
                stick.Item1.XRange = (uint)(stick.Item1.OriginalXRange * xRatio);
                stick.Item1.YRange = (uint)(stick.Item1.OriginalYRange * yRatio);
            }
        }


        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;

        private void Window_SourceInitialized(object sender, EventArgs ea)
        {   
            var hwnd = new WindowInteropHelper((Window)sender).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
       
            WindowAspectRatio.Register((ViewWindow)sender);
        }

        private double _originalHeight;
        private double _originalWidth;
        private double _magicWidth;
        private double _magicHeight;

        Skin _skin;
        IControllerReader _reader;
        Keybindings _keybindings;
        BlinkReductionFilter _blinkFilter = new BlinkReductionFilter ();

        List<Tuple<Skin.Detail, Image>> _detailsWithImages = new List<Tuple<Skin.Detail, Image>>();
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

            ControllerGrid.Width = Width = _originalWidth = skinBackground.Width;
            ControllerGrid.Height = Height = _originalHeight = skinBackground.Height;
            var brush = new SolidColorBrush(skinBackground.Color);
            ControllerGrid.Background = brush;

            if (skinBackground.Image != null)
            {
                var img = new Image();
                img.VerticalAlignment = VerticalAlignment.Top;
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.Source = skinBackground.Image;
                img.Stretch = Stretch.Fill;
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
                    _detailsWithImages.Add(new Tuple<Skin.Detail, Image>(detail, image));
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
            img.Stretch = Stretch.Uniform;
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
            if (this.Dispatcher.CheckAccess())
                Close();
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Close();
                });
            }
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

            // This assumes you can't press left/right and up/down at the same time.  The code gets more complicated otherwise.
            var compassDirectionStates = new Dictionary<string, bool>();
            if (newState.Buttons.ContainsKey("up") && newState.Buttons.ContainsKey("left") && newState.Buttons.ContainsKey("right") && newState.Buttons.ContainsKey("down"))
            {
                string[] compassDirections= { "north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest" };

                var compassDirectionStatesTemp = new bool[8];
                compassDirectionStatesTemp[0] = newState.Buttons["up"];
                compassDirectionStatesTemp[2] = newState.Buttons["right"];
                compassDirectionStatesTemp[4] = newState.Buttons["down"];
                compassDirectionStatesTemp[6] = newState.Buttons["left"];

                if (compassDirectionStatesTemp[0] && compassDirectionStatesTemp[2])
                {
                    compassDirectionStatesTemp[1] = true;
                    compassDirectionStatesTemp[0] = compassDirectionStatesTemp[2] = false;
                }
                else if (compassDirectionStatesTemp[2] && compassDirectionStatesTemp[4])
                {
                    compassDirectionStatesTemp[3] = true;
                    compassDirectionStatesTemp[2] = compassDirectionStatesTemp[4] = false;
                }
                else if (compassDirectionStatesTemp[4] && compassDirectionStatesTemp[6])
                {
                    compassDirectionStatesTemp[5] = true;
                    compassDirectionStatesTemp[4] = compassDirectionStatesTemp[6] = false;
                }
                else if (compassDirectionStatesTemp[6] && compassDirectionStatesTemp[0])
                {
                    compassDirectionStatesTemp[7] = true;
                    compassDirectionStatesTemp[6] = compassDirectionStatesTemp[0] = false;
                }

                for(int i = 0; i < compassDirections.Length; ++i)
                {
                    compassDirectionStates[compassDirections[i]] = compassDirectionStatesTemp[i];
                }
            }

            foreach (var button in _buttonsWithImages)
            {
                if (newState.Buttons.ContainsKey(button.Item1.Name))
                {
                    if (button.Item2.Dispatcher.CheckAccess())
                        button.Item2.Visibility = newState.Buttons[button.Item1.Name] ? Visibility.Visible : Visibility.Hidden;
                    else
                    {
                        button.Item2.Dispatcher.Invoke(() =>
                        {
                            button.Item2.Visibility = newState.Buttons[button.Item1.Name] ? Visibility.Visible : Visibility.Hidden;
                        });
                    }
                }
                else if (compassDirectionStates.ContainsKey(button.Item1.Name))
                {
                    if (button.Item2.Dispatcher.CheckAccess())
                        button.Item2.Visibility = compassDirectionStates[button.Item1.Name] ? Visibility.Visible : Visibility.Hidden;
                    else
                    {
                        button.Item2.Dispatcher.Invoke(() =>
                        {
                            button.Item2.Visibility = compassDirectionStates[button.Item1.Name] ? Visibility.Visible : Visibility.Hidden;
                        });
                    }
                }

            }

            foreach (var button in _rangeButtonsWithImages) 
            {
                if (!newState.Analogs.ContainsKey (button.Item1.Name)) continue;

                var value = newState.Analogs [button.Item1.Name];
                var visible = button.Item1.From <= value && value <= button.Item1.To;
                button.Item2.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            }

            foreach (var stick in _sticksWithImages)
            {
                var skin = stick.Item1;
                var image = stick.Item2;

                float xrange = (skin.XReverse ? -1 : 1) * skin.XRange;
                float yrange = (skin.YReverse ? 1 : -1) * skin.YRange;

                var x = newState.Analogs.ContainsKey(skin.XName)
                      ? skin.Config.X + xrange * newState.Analogs[skin.XName]
                      : skin.Config.X;

                var y = newState.Analogs.ContainsKey(skin.YName)
                      ? skin.Config.Y + yrange * newState.Analogs[skin.YName]
                      : skin.Config.Y;

                if (image.Dispatcher.CheckAccess())
                    image.Margin = new Thickness(x, y, 0, 0);
                else
                {
                    image.Dispatcher.Invoke(() =>
                    {
                        image.Margin = new Thickness(x, y, 0, 0);
                    });
                }
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
                        if (grid.Dispatcher.CheckAccess())
                        {
                            grid.Width = skin.Config.Width * val;
                        }
                        else
                        {
                            grid.Dispatcher.Invoke(() =>
                            {
                                grid.Width = skin.Config.Width * val;
                            });
                        }
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Left:
                        var width = skin.Config.Width * val;
                        var offx = skin.Config.Width - width;
                        if (grid.Dispatcher.CheckAccess())
                        {
                            grid.Margin = new Thickness(skin.Config.X + offx, skin.Config.Y, 0, 0);
                            grid.Width = width;
                        }
                        else
                        {
                            grid.Dispatcher.Invoke(() =>
                            {
                                grid.Margin = new Thickness(skin.Config.X + offx, skin.Config.Y, 0, 0);
                                grid.Width = width;
                            });
                        }
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Down:
                        if (grid.Dispatcher.CheckAccess())
                        {
                            grid.Height = skin.Config.Height * val;
                        }
                        else
                        {
                            grid.Dispatcher.Invoke(() =>
                            {
                                grid.Height = skin.Config.Height * val;
                            });
                        }
                        break;

                    case Skin.AnalogTrigger.DirectionValue.Up:
                        var height = skin.Config.Height * val;
                        var offy = skin.Config.Height - height;
                        if (grid.Dispatcher.CheckAccess())
                        {
                            grid.Margin = new Thickness(skin.Config.X, skin.Config.Y + offy, 0, 0);
                            grid.Height = height;
                        }
                        else
                        {
                            grid.Dispatcher.Invoke(() =>
                            {
                                grid.Margin = new Thickness(skin.Config.X, skin.Config.Y + offy, 0, 0);
                                grid.Height = height;
                            });
                        }
                        break;
                    case Skin.AnalogTrigger.DirectionValue.Fade:
                        if (grid.Dispatcher.CheckAccess())
                        {
                            grid.Height = skin.Config.Height;
                            grid.Width = skin.Config.Width;
                            grid.Opacity = val;
                        }
                        else
                        {
                            grid.Dispatcher.Invoke(() =>
                            {
                                grid.Height = skin.Config.Height;
                                grid.Width = skin.Config.Width;
                                grid.Opacity = val;
                            });
                        }
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

    internal class WindowAspectRatio
    {
        private double _ratio;
        private ViewWindow _window;
        private bool _calculatedMagic;

        private WindowAspectRatio(ViewWindow window)
        {
            _calculatedMagic = false;
            _window = window;
            _ratio = window.GetRatio();
            ((HwndSource)HwndSource.FromVisual(window)).AddHook(DragHook);
        }

        public static void Register(ViewWindow window)
        {
            new WindowAspectRatio(window);
        }

        internal enum WM
        {
            WINDOWPOSCHANGING = 0x0046,
        }

        [Flags()]
        public enum SWP
        {
            NoMove = 0x2,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        private IntPtr DragHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handeled)
        {
            if (!_calculatedMagic)
            {
                _window.calculateMagic();
                _calculatedMagic = true;
            }

            if ((WM)msg == WM.WINDOWPOSCHANGING)
            {

                WINDOWPOS position = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

                if ((position.flags & (int)SWP.NoMove) != 0 ||
                    HwndSource.FromHwnd(hwnd).RootVisual == null) return IntPtr.Zero;

                double magicWidth = _window.GetMagicWidth();
                double magicHeight = _window.GetMagicHeight();

                position.cx = (int)( magicWidth + ((position.cy-magicHeight) * _ratio));

                Marshal.StructureToPtr(position, lParam, true);
                handeled = true;

                _window.AdjustControllerElements();
            }

            return IntPtr.Zero;
        }
    }
}
