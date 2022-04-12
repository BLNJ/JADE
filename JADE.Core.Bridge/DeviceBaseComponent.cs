using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge
{
    public abstract class DeviceBaseComponent
    {
        public DeviceBase device
        {
            get;
            private set;
        }

        public DeviceBaseComponent(DeviceBase device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            this.device = device;
        }

        public abstract void Step();
        public abstract void Reset();
    }
}
