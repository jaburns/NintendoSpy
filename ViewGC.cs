using System.Windows.Forms;

namespace N64Spy
{
    public partial class ViewGC : Form
    {
        private ViewManager viewManager;

        public ViewGC( string comPort )
        {
            InitializeComponent();

            viewManager = new ViewManager
            (
                this, comPort, new ControllerReader_GC(),

                new Control[] {
                    //                                                        -      L     R    
                    null, null, null, btn_Start, btn_Y, btn_X, btn_B, btn_A, null, null, null, btn_Z, btn_Dup, btn_Ddown, btn_Dright, btn_Dleft
                },

                new DisplayStick[] {
                    new DisplayStick() {
                        display = stick,
                        movementRadius = 16
                    },
                    new DisplayStick() {
                        display = cstick,
                        movementRadius = 13
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
