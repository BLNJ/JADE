using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge.MemoryManagementUnit
{
    public abstract class MemoryBase
    {
        public DeviceBase Device
        {
            get;
            private set;
        }

        public MemoryBase(DeviceBase device)
        {
            this.Device = device;
        }


    }
}
