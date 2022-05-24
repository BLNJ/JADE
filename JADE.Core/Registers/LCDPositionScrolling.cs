using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Registers
{
    public class LCDPositionScrolling : MemoryRegistersBase
    {
        public byte ScrollY
        {
            get
            {
                return base.mmu.Stream.ReadByte(0xFF42);
            }
            set
            {
                base.mmu.Stream.WriteByte(0xFF42, value);
            }
        }
        public byte ScrollX
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF43);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF43, value);
            }
        }

        public byte LY
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF44);
            }
            set
            {
                //this.mmu.WriteByte(0xFF44, value);
                this.mmu.Stream.WriteByte(0xFF44, 0);
            }
        }
        public byte LYC
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF45);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF45, value);
            }
        }
        public byte WindowY
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF4A);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF4A, value);
            }
        }
        public byte WindowX
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF4B);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF4B, value);
            }
        }

        public LCDPositionScrolling(MemoryManagementUnit.MMU mmu) : base(mmu)
        {

        }
    }
}
