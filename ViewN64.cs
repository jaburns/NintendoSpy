using System.Windows.Forms;

namespace NintendoSpy
{
    public partial class ViewN64 : Form
    {
        private ViewManager viewManager;

        public ViewN64( string comPort )
        {
            InitializeComponent();

            viewManager = new ViewManager
            (
                this, comPort, new ControllerReader_N64(),

                new Control[] {
                    btn_A, btn_B, btn_Z, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright, null, null, btn_L, btn_R, btn_Cup, btn_Cdown, btn_Cleft, btn_Cright
                },

                new DisplayStick[] {
                    new DisplayStick() {
                        display = stick,
                        movementRadius = 12
                    }
                }
            );
        }

        private void ViewN64_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewManager.Close();
        }
    }
}
