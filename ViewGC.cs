using System.Windows.Forms;

namespace NintendoSpy
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
                    null, null, null, btn_Start, btn_Y, btn_X, btn_B, btn_A, null, btn_L, btn_R, btn_Z, btn_Dup, btn_Ddown, btn_Dright, btn_Dleft
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
                },

                new DisplayTrigger[] {
                    new DisplayTrigger() {
                        display = triggerL,
                        mirror = true
                    },
                    new DisplayTrigger() {
                        display = triggerR,
                        mirror = false
                    },
                }
            );
        }

        private void ViewN64_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewManager.Close();
        }
    }
}
