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

namespace NintendoSpy
{
    public partial class ViewWindow : Window
    {
        public ViewWindow (Skin skin)
        {
            InitializeComponent ();

            ControllerGrid.Width = skin.BackgroundImage.PixelWidth;
            ControllerGrid.Height = skin.BackgroundImage.PixelHeight;

            this.Background = new SolidColorBrush (skin.BackgroundColor);

            var brush = new ImageBrush (skin.BackgroundImage);
            brush.Stretch = Stretch.Uniform;
            ControllerGrid.Background = brush;

            ControllerGrid.Children.Add (getImageForElement (skin.Buttons[0].Config));
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
