using System.Windows.Forms;

namespace NintendoSpy
{
    public partial class ViewNES : Form
    {
        private ViewManager viewManager;

        public ViewNES( string comPort )
        {
            InitializeComponent();

            viewManager = new ViewManager
            (
                this, comPort, new ControllerReader_ButtonsOnly( 8 ),

                new Control[] {
                    btn_A, btn_B, btn_Select, btn_Start, btn_Dup, btn_Ddown, btn_Dleft, btn_Dright
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
