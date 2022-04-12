using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge
{
    public abstract class DeviceBase
    {
        public abstract CentralProcessingUnit.CPUBase CPU
        {
            get;
        }
        public abstract PictureProcessingUnit.PPUBase PPU
        {
            get;
        }
        public abstract MemoryManagementUnit.MMUBase MMU
        {
            get;
        }

        public abstract ReadOnlyMemory.ROMBase ROM
        {
            get;
        }

        public DeviceBase()
        {
            this.Reset();
        }

        public abstract void Start();
        public abstract void Pause();

        public virtual void Reset()
        {
            this.MMU.Reset();
            this.CPU.Reset();
            this.PPU.Reset();
        }
    }
}
