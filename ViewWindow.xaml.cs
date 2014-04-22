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
using System.Windows.Shapes;

using NintendoSpy.Readers;

namespace NintendoSpy
{
    public partial class ViewWindow : Window
    {
        Skin _skin;
        IControllerReader _reader;

        List <Tuple <Skin.Button,Image>> _buttonsWithImages = new List <Tuple <Skin.Button,Image>> ();
        List <Tuple <Skin.AnalogStick,Image>> _sticksWithImages = new List <Tuple <Skin.AnalogStick,Image>> ();

        // The triggers images are embedded inside of a Grid element so that we can properly mask leftwards and upwards
        // without the image aligning to the top left of its element.
        List <Tuple <Skin.AnalogTrigger,Grid>> _triggersWithGridImages = new List <Tuple <Skin.AnalogTrigger,Grid>> ();


        public ViewWindow (Skin skin, Skin.Background skinBackground, IControllerReader reader)
        {
            InitializeComponent ();

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
            
            foreach (var stick in _skin.AnalogSticks) {
                var image = getImageForElement (stick.Config);
                _sticksWithImages.Add (new Tuple <Skin.AnalogStick,Image> (stick, image));
                ControllerGrid.Children.Add (image);
            }

            _reader.ControllerStateChanged += reader_ControllerStateChanged;
            _reader.ControllerDisconnected += reader_ControllerDisconnected;
        }


        static Image getImageForElement (Skin.ElementConfig config)
        {
            var img = new Image ();
            img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
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

            img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            img.HorizontalAlignment = 
                  trigger.Direction == Skin.AnalogTrigger.DirectionValue.Left
                ? System.Windows.HorizontalAlignment.Right
                : System.Windows.HorizontalAlignment.Left;

            img.VerticalAlignment = 
                  trigger.Direction == Skin.AnalogTrigger.DirectionValue.Up
                ? System.Windows.VerticalAlignment.Bottom
                : System.Windows.VerticalAlignment.Top;

            img.Source = trigger.Config.Image;
            img.Stretch = Stretch.None;
            img.Margin = new Thickness (0, 0, 0, 0);
            img.Width = trigger.Config.Width;
            img.Height = trigger.Config.Height;

            var grid = new Grid ();

            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            grid.Margin = new Thickness (trigger.Config.X, trigger.Config.Y, 0, 0);
            grid.Width = trigger.Config.Width;
            grid.Height = trigger.Config.Height;

            grid.Children.Add (img);

            return grid;
        }


        void reader_ControllerDisconnected (object sender, EventArgs e)
        {
            Close ();
        }

        void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            _reader.Finish ();
        }

        void reader_ControllerStateChanged (object sender, EventArgs e)
        {
            foreach (var button in _buttonsWithImages) 
            {
                if (!_reader.State.Buttons.ContainsKey (button.Item1.Name)) continue;

                button.Item2.Visibility = _reader.State.Buttons [button.Item1.Name] ? Visibility.Visible : Visibility.Hidden ;
            }

            foreach (var stick in _sticksWithImages)
            {
                var skin = stick.Item1;
                var image = stick.Item2;

                float xrange = (skin.XReverse ? -1 :  1) * skin.XRange;
                float yrange = (skin.YReverse ?  1 : -1) * skin.YRange;

                var x = _reader.State.Analogs.ContainsKey (skin.XName)
                      ? skin.Config.X + xrange * _reader.State.Analogs [skin.XName]
                      : skin.Config.X ;

                var y = _reader.State.Analogs.ContainsKey (skin.YName)
                      ? skin.Config.Y + yrange * _reader.State.Analogs [skin.YName]
                      : skin.Config.Y ;
                
                image.Margin = new Thickness (x,y,0,0);
            }
            
            foreach (var trigger in _triggersWithGridImages)
            {
                var skin = trigger.Item1;
                var grid = trigger.Item2;

                if (!_reader.State.Analogs.ContainsKey (skin.Name)) continue;

                var val = _reader.State.Analogs [skin.Name];
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
    }
}
