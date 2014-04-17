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

        Dictionary <string,Image> _buttonImages = new Dictionary <string,Image> ();

        public ViewWindow (Skin skin, IControllerReader reader)
        {
            InitializeComponent ();

            _skin = skin;
            _reader = reader;

            ControllerGrid.Width = _skin.BackgroundImage.PixelWidth;
            ControllerGrid.Height = _skin.BackgroundImage.PixelHeight;

            this.Background = new SolidColorBrush (_skin.BackgroundColor);

            var brush = new ImageBrush (_skin.BackgroundImage);
            brush.Stretch = Stretch.Uniform;
            ControllerGrid.Background = brush;

            foreach (var button in _skin.Buttons) {
                var newImage = getImageForElement (button.Value.Config);
                _buttonImages [button.Key] = newImage;
                ControllerGrid.Children.Add (newImage);
            }

            _reader.ControllerStateChanged += reader_ControllerStateChanged;
        }

        void reader_ControllerStateChanged (object sender, EventArgs e)
        {
            foreach (var button in _buttonImages) {
                button.Value.Visibility = _reader.State.Buttons [button.Key] ? Visibility.Visible : Visibility.Hidden ;
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
