using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JADE.Core.Bridge.Interrupts
{
    public abstract class MemoryInterruptBase : InterruptBase
    {
        MemoryManagementUnit.MMUBase mmu;

        public ushort BaseAddress
        {
            get;
            private set;
        }

        public MemoryInterruptBase(MemoryManagementUnit.MMUBase mmu, ushort baseAddress)
        {
            this.mmu = mmu;
            this.BaseAddress = baseAddress;
        }

        public byte ReadByte(ushort address)
        {
            byte flags = mmu.Stream.ReadByte(BaseAddress + address);
            return flags;
        }

        public void WriteByte(ushort address, byte value)
        {
            mmu.Stream.WriteByte(BaseAddress + address, value);
        }
    }
}
