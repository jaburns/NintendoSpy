using System;
using System.Windows.Threading;

namespace NintendoSpy.Readers
{
    sealed public class MouseTester : IControllerReader
    {
        const double TIMER_MS = 7;

        DispatcherTimer _timer;

        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        public MouseTester(string portName)
        {
            _timer = new DispatcherTimer ();
            _timer.Interval = TimeSpan.FromMilliseconds (TIMER_MS);
            _timer.Tick += tick;
            _timer.Start ();
        }

        bool buttonsOn = false;
        float theta = 0;
        bool inCenter = false;
        int ticks = 0;


        void tick (object sender, EventArgs e)
        {
            var outState = new ControllerStateBuilder();

            if (ticks % 18 == 0)
                buttonsOn = buttonsOn ? false : true;

            ticks++;

            if (!inCenter && (theta == 0 || theta == 90 || theta == 180 || theta == 270))
                inCenter = true;
            else if (inCenter)
                inCenter = false;

            float x = 0;
            float y = 0;

            if(!inCenter)
            {
                x = (float)Math.Cos(theta * Math.PI / 180);
                y = (float)Math.Sin(theta * Math.PI / 180);
                theta = theta + 5;
                theta %= 360;

            }
            outState.SetButton("left", buttonsOn);
            outState.SetButton("middle", buttonsOn);
            outState.SetButton("right", buttonsOn);
            outState.SetButton("wired-1", buttonsOn);
            outState.SetButton("wired-2", buttonsOn);
            outState.SetButton("a", buttonsOn);
            outState.SetButton("b", buttonsOn);
            outState.SetButton("c", buttonsOn);
            outState.SetButton("start", buttonsOn);
            outState.SetButton("thumb", buttonsOn);
            outState.SetButton("scroll_up", buttonsOn);
            outState.SetButton("scroll_down", buttonsOn);

            outState.SetButton("mouse_center", true);

            outState.SetAnalog("stick_x", x);
            outState.SetAnalog("stick_y", y);

            if (ControllerStateChanged != null) ControllerStateChanged (this, outState.Build ());
        }

        public void Finish ()
        {
            if (_timer != null) {
                _timer.Stop ();
                _timer = null;
            }
        }
    }
}
