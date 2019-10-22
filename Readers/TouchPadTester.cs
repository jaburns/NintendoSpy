using System;
using System.Windows.Threading;

namespace RetroSpy.Readers
{
    sealed public class TouchPadTester : IControllerReader
    {
        Random rand;
        const double TIMER_MS = 7;

        DispatcherTimer _timer;

        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        public TouchPadTester(string portName)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(TIMER_MS);
            _timer.Tick += tick;
            rand = new Random();
            _timer.Start();
        }

        bool visable = false;
        bool buttonsOn = false;
        float theta = 0;
        bool inCenter = false;
        int ticks = 0;


        void tick(object sender, EventArgs e)
        {
            Random rand = new Random();
            var outState = new ControllerStateBuilder();

            if (ticks % 18 == 0)
                buttonsOn = buttonsOn ? false : true;

            if (ticks % 36 == 0)
                visable = visable ? false : true;

            ticks++;

            double x1 = rand.NextDouble();
            double y1 = rand.NextDouble();
            double x2 = rand.NextDouble();
            double y2 = rand.NextDouble();

            double x3 = rand.NextDouble();
            double y3 = rand.NextDouble();
            double x4 = rand.NextDouble();
            double y4 = rand.NextDouble();


            if (visable)
            {
                if (buttonsOn)
                {
                    outState.SetAnalog("touchpad_x3", (float)x3);
                    outState.SetAnalog("touchpad_y3", (float)y3);
                    outState.SetAnalog("touchpad_x4", (float)x4);
                    outState.SetAnalog("touchpad_y4", (float)y4);
                }
                else
                {
                    outState.SetAnalog("touchpad_x1", (float)x1);
                    outState.SetAnalog("touchpad_y1", (float)y1);
                    outState.SetAnalog("touchpad_x2", (float)x2);
                    outState.SetAnalog("touchpad_y2", (float)y2);
                }

            }
            if (ControllerStateChanged != null) ControllerStateChanged(this, outState.Build());
        }

        public void Finish()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}
