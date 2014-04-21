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

        public ViewWindow (Skin skin, IControllerReader reader)
        {
            InitializeComponent ();

            _skin = skin;
            _reader = reader;

            ControllerGrid.Width = _skin.BackgroundImage.PixelWidth;
            ControllerGrid.Height = _skin.BackgroundImage.PixelHeight;

            var brush = new ImageBrush (_skin.BackgroundImage);
            brush.Stretch = Stretch.Uniform;
            ControllerGrid.Background = brush;

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
        }

        void reader_ControllerStateChanged (object sender, EventArgs e)
        {
            foreach (var button in _buttonsWithImages) {
                if (!_reader.State.Buttons.ContainsKey (button.Item1.Name)) continue;
                button.Item2.Visibility = _reader.State.Buttons [button.Item1.Name] ? Visibility.Visible : Visibility.Hidden ;
            }

            foreach (var stick in _sticksWithImages)
            {
                var skin = stick.Item1;
                var image = stick.Item2;

                var x = skin.Config.X + skin.XRange * _reader.State.Analogs [skin.XName];
                var y = skin.Config.Y - skin.YRange * _reader.State.Analogs [skin.YName];
                
                image.Margin = new Thickness (x,y,0,0);
            }
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
    }
}
