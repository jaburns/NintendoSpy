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

            var brush = new ImageBrush (skin.BackgroundImage);
            brush.Stretch = Stretch.Uniform;
            ControllerGrid.Background = brush;

            this.Background = new SolidColorBrush (skin.BackgroundColor);
        }
    }
}
