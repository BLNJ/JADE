using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JADE.Core.PictureProcessingUnit
{
    public class ColorPalette
    {
        private PictureProcessingUnit.PPU ppu;
        private MemoryManagementUnit.MMU mmu
        {
            get
            {
                return this.ppu.device.MMU;
            }
        }

        private ushort baseAddress;
        private bool lowestColorTransparent;

        private byte colorRaw
        {
            get
            {
                return this.mmu.Stream.ReadByte(this.baseAddress);
            }
            set
            {
                this.mmu.Stream.WriteByte(this.baseAddress, value);
            }
        }

        public ColorPalette(PPU ppu, ushort baseAddress, bool lowestTransparent = true)
        {
            this.ppu = ppu;
            this.baseAddress = baseAddress;
            this.lowestColorTransparent = lowestTransparent;
        }

        public byte GetColorByte(byte index)
        {
            bool lower = this.colorRaw.GetBit((index * 2) + 0);
            bool upper = this.colorRaw.GetBit((index * 2) + 1);

            byte color = 0;
            color = color.SetBit(0, lower);
            color = color.SetBit(1, upper);

            return color;
        }
        public Color GetColor(byte index)
        {
            byte colorRaw = GetColorByte(index);
            switch (colorRaw)
            {
                case 0:
                    if (this.lowestColorTransparent)
                    {
                        return Color.Transparent;
                    }
                    else
                    {
                        return Color.White;
                    }
                case 1:
                    return Color.LightGray;
                case 2:
                    return Color.DarkGray;
                case 3:
                    return Color.Black;
                default:
                    throw new ArgumentException("Unknown color: " + colorRaw.ToString());
            }
        }
    }
}
