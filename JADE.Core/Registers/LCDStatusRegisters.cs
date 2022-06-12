using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Registers
{
    public class LCDStatusRegisters : MemoryRegistersBase
    {
        private byte lcd_status
        {
            get
            {
                return base.mmu.Stream.ReadByte(0xFF41, jumpBack: true);
            }
            set
            {
                base.mmu.Stream.WriteByte(0xFF41, value, jumpBack: true);
            }
        }
        public bool LYC_LY_CoincidenceInterrupt
        {
            get
            {
                return this.lcd_status.GetBit(6);
            }
            set
            {
                this.lcd_status = this.lcd_status.SetBit(6, value);
            }
        }
        public bool Mode2_OAMInterrupt
        {
            get
            {
                return this.lcd_status.GetBit(5);
            }
            set
            {
                this.lcd_status = this.lcd_status.SetBit(5, value);
            }
        }
        public bool Mode1_VBlankInterrupt
        {
            get
            {
                return this.lcd_status.GetBit(4);
            }
            set
            {
                this.lcd_status = this.lcd_status.SetBit(4, value);
            }
        }
        public bool Mode0_HBlankInterrupt
        {
            get
            {
                return this.lcd_status.GetBit(3);
            }
            set
            {
                this.lcd_status = this.lcd_status.SetBit(3, value);
            }
        }
        public bool CoincidenceFlag
        {
            get
            {
                return this.lcd_status.GetBit(2);
            }
            //set
            //{
            //    this.lcd_status = this.lcd_status.SetBit(2, value);
            //}
        }
        public ModeFlag Mode
        {
            get
            {
                bool bit1 = this.lcd_status.GetBit(1);
                bool bit0 = this.lcd_status.GetBit(0);
                byte value = 0;
                value = (value.SetBit(0, bit0).SetBit(1, bit1));

                return (ModeFlag)value;
            }
            set
            {
                bool bit1 = ((byte)value).GetBit(1);
                bool bit0 = ((byte)value).GetBit(0);

                this.lcd_status = this.lcd_status.SetBit(1, bit1);
                this.lcd_status = this.lcd_status.SetBit(0, bit0);
            }
        }

        public LCDStatusRegisters(MemoryManagementUnit.MMU mmu) : base(mmu)
        {

        }
        public void Reset()
        {
            this.Mode = ModeFlag.VBlank;
        }

        public enum ModeFlag : byte
        {
            HBlank = 0,
            VBlank = 1,
            SearchingOAM = 2,
            TransferingData = 3
        }
    }
}
