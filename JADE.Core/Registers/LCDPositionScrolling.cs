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
                return base.mmu.Stream.ReadByte(0xFF42, jumpBack: true);
            }
            set
            {
                base.mmu.Stream.WriteByte(0xFF42, value, jumpBack: true);
            }
        }
        public byte ScrollX
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF43, jumpBack: true);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF43, value, jumpBack: true);
            }
        }

        public byte LY
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF44, jumpBack: true);
            }
            set
            {
                //this.mmu.WriteByte(0xFF44, value);
                this.mmu.Stream.WriteByte(0xFF44, 0, jumpBack: true);
            }
        }
        public byte LYC
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF45, jumpBack: true);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF45, value, jumpBack: true);
            }
        }
        public byte WindowY
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF4A, jumpBack: true);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF4A, value, jumpBack: true);
            }
        }
        public byte WindowX
        {
            get
            {
                return this.mmu.Stream.ReadByte(0xFF4B, jumpBack: true);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF4B, value, jumpBack: true);
            }
        }

        public LCDPositionScrolling(MemoryManagementUnit.MMU mmu) : base(mmu)
        {

        }
    }
}
