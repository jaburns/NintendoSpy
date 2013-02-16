using System.Windows.Forms;

namespace N64Spy
{
    public partial class ViewSnes : Form
    {
        private ViewManager viewManager;

        public ViewSnes( string comPort )
        {
            InitializeComponent();

            viewManager = new ViewManager
            (
                this, comPort, new ControllerReader_SNES(),

                new Control[] {
                    btn_B, btn_Y, btn_Select, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright, btn_A, btn_X, btn_L, btn_R, null, null, null, null
                },

                null
            );
        }

        private void ViewSnes_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewManager.Close();
        }
    }
}
