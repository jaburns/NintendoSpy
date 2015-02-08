using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    sealed public class DelayedControllerReader : IControllerReader, IDisposable
    {
        private readonly IControllerReader baseControllerReader;
        private readonly int delayInMilliseconds;

        public event EventHandler ControllerDisconnected;
        public event StateEventHandler ControllerStateChanged;

        public IControllerReader BaseControllerReader { get { return baseControllerReader; } }
        public int DelayInMilliseconds { get { return delayInMilliseconds; } }

        public DelayedControllerReader(IControllerReader baseControllerReader, int delayInMilliseconds)
        {
            this.baseControllerReader = baseControllerReader;
            this.delayInMilliseconds = delayInMilliseconds;

            BaseControllerReader.ControllerStateChanged += BaseControllerReader_ControllerStateChanged;
        }

        private async void BaseControllerReader_ControllerStateChanged(IControllerReader sender, ControllerState state)
        {
            if (!disposedValue)
            {
                await Task.Delay(delayInMilliseconds);

                var controllerStateChanged = ControllerStateChanged;
                if (controllerStateChanged != null)
                    controllerStateChanged(this, state);
            }
        }

        public void Finish()
        {
            if (!disposedValue)
            {
                BaseControllerReader.Finish();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BaseControllerReader.ControllerStateChanged -= BaseControllerReader_ControllerStateChanged;       
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
