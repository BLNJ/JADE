using JADE.Core.Bridge.Interrupts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Interrupts
{
    public class CPUInterrupts : MemoryInterruptBase
    {
        byte interruptFlag
        {
            get
            {
                return ReadByte(0);
            }
            set
            {
                this.WriteByte(0, value);
            }
        }
        public bool VBlank
        {
            get
            {
                return this.interruptFlag.GetBit(0);
            }
            set
            {
                this.interruptFlag = interruptFlag.SetBit(0, value);
                OnPropertyChanged();
            }
        }
        public bool LCD_STAT
        {
            get
            {
                return this.interruptFlag.GetBit(1);
            }
            set
            {
                this.interruptFlag = interruptFlag.SetBit(1, value);
                OnPropertyChanged();
            }
        }
        public bool Timer
        {
            get
            {
                return this.interruptFlag.GetBit(2);
            }
            set
            {
                this.interruptFlag = interruptFlag.SetBit(2, value);
                OnPropertyChanged();
            }
        }
        public bool Serial
        {
            get
            {
                return this.interruptFlag.GetBit(3);
            }
            set
            {
                this.interruptFlag = interruptFlag.SetBit(3, value);
                OnPropertyChanged();
            }
        }
        public bool Joypad
        {
            get
            {
                return this.interruptFlag.GetBit(4);
            }
            set
            {
                this.interruptFlag = interruptFlag.SetBit(4, value);
                OnPropertyChanged();
            }
        }

        public CPUInterrupts(Bridge.MemoryManagementUnit.MMUBase mmu, ushort baseAddress) : base(mmu, baseAddress)
        {
        }
    }
}
