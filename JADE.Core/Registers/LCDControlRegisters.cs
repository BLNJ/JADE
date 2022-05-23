using JADE.Core.Bridge.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Registers
{
    public class LCDControlRegisters : MemoryRegistersBase
    {
        private byte lcd_control
        {
            get
            {
                byte value = base.mmu.Stream.ReadByte(0xFF40);
                return value;
            }
            set
            {
                base.mmu.Stream.WriteByte(0xFF40, value);
            }
        }
        public bool LCDEnabled
        {
            get
            {
                return this.lcd_control.GetBit(7);
            }
            set
            {
                this.lcd_control = this.lcd_control.SetBit(7, value);
            }
        }
        public bool WindowEnabled
        {
            get
            {
                return this.lcd_control.GetBit(5);
            }
            set
            {
                this.lcd_control = this.lcd_control.SetBit(5, value);
            }
        }

        public MemoryRegion SpriteTileTable
        {
            get
            {
                return new MemoryRegion()
                {
                    Start = 0x8000,
                    End = 0x9000
                };
            }
        }

        public MemoryRegion WindowTileTable
        {
            get
            {
                bool value = this.lcd_control.GetBit(6);
                MemoryRegion region = new MemoryRegion();
                region.IsFirst = value;

                if (value)
                {
                    region.Start = 0x9C00;
                    region.End = 0x9FFF;
                }
                else
                {
                    region.Start = 0x9800;
                    region.End = 0x9BFF;
                }
                return region;
            }
        }

        public bool TileSetZero
        {
            get
            {
                bool value = this.lcd_control.GetBit(4);
                return value;
            }
            set
            {
                this.lcd_control.SetBit(4, value);
            }
        }

        public MemoryRegion TileSetRegion
        {
            get
            {
                bool value = TileSetZero;
                MemoryRegion region = new MemoryRegion();
                region.IsFirst = value;

                if (value)
                {
                    region.Start = 0x8000;
                    region.End = 0x8FFF;
                }
                else
                {
                    region.Start = 0x8800;
                    region.End = 0x97FF;
                }
                return region;
            }
        }
        public MemoryRegion BackgroundTileMapRegion
        {
            get
            {
                bool value = this.lcd_control.GetBit(3);
                MemoryRegion region = new MemoryRegion();
                region.IsFirst = value;

                if (value)
                {
                    region.Start = 0x9C00;
                    region.End = 0x9FFF;
                }
                else
                {
                    region.Start = 0x9800;
                    region.End = 0x9BFF;
                }
                return region;
            }
        }
        public byte SpriteSize_Width
        {
            get
            {
                return 8;
            }
        }
        public byte SpriteSize_Heigth
        {
            get
            {
                bool value = this.lcd_control.GetBit(2);
                if(value)
                {
                    return 16;
                }
                else
                {
                    return 8;
                }
            }
            set
            {
                if(value == 16)
                {
                    this.lcd_control = this.lcd_control.SetBit(2, true);
                }
                else if(value == 8)
                {
                    this.lcd_control = this.lcd_control.SetBit(2, false);
                }
                else
                {
                    throw new Exception(string.Format("Unknown Heigth: {0}", value));
                }
            }
        }
        public bool SpriteDisplayEnable
        {
            get
            {
                return this.lcd_control.GetBit(1);
            }
            set
            {
                this.lcd_control = this.lcd_control.SetBit(1, value);
            }
        }
        public bool BackgroundDisplay
        {
            get
            {
                return this.lcd_control.GetBit(0);
            }
            set
            {
                this.lcd_control = this.lcd_control.SetBit(0, value);
            }
        }


        public LCDControlRegisters(MemoryManagementUnit.MMU mmu) : base(mmu)
        {

        }

        public void Reset()
        {
            this.lcd_control = 0x91;
        }

        public class MemoryRegion
        {
            public bool IsFirst;
            public int Start;
            public int End;
        }
    }
}
