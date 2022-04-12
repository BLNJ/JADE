using JADE.Core.Bridge.MemoryManagementUnit;
using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge.CentralProcessingUnit
{
    public abstract class CPUBase<registerType> : DeviceBaseComponent where registerType : Register.CPURegistersBase
    {
        public abstract registerType Registers
        {
            get;
        }

        public MMUBase MMU
        {
            get
            {
                return base.device.MMU;
            }
        }

        public CPUBase(DeviceBase device) : base(device)
        {
        }
    }
}
