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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                this.mmu.Stream.WriteByte(0xFF44, value, jumpBack: true);
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        public byte WindowX
        {
            get
            {
                return (byte)(this.mmu.Stream.ReadByte(0xFF4B, jumpBack: true) + 7);
            }
            set
            {
                this.mmu.Stream.WriteByte(0xFF4B, (byte)(value - 7), jumpBack: true);
                OnPropertyChanged();
            }
        }

        public LCDPositionScrolling(MemoryManagementUnit.MMU mmu) : base(mmu)
        {

        }
    }
}
