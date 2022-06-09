using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace JADE.Core.PictureProcessingUnit
{
    public class TileData
    {
        PPU ppu;
        MemoryManagementUnit.MMU mmu
        {
            get
            {
                return ppu.device.MMU;
            }
        }

        public ushort BaseAddress;
        private byte[] RawData
        {
            get
            {
                return mmu.Stream.ReadBytes(BaseAddress, 16, jumpBack: true);
            }
        }

        public TileData(PPU ppu, ushort baseAddress)
        {
            this.ppu = ppu;
            this.BaseAddress = baseAddress;
        }

        public Color GetPixelColor(int x, int y)
        {
            byte colorRaw = GetColorData(x, y);
            Color color = ppu.BGPaletteData.GetColor(colorRaw);
            return color;

            //switch (colorRaw)
            //{
            //    case 0:
            //        return Color.Transparent;
            //    case 1:
            //        return Color.LightGray;
            //    case 2:
            //        return Color.DarkGray;
            //    case 3:
            //        return Color.Black;
            //    default:
            //        throw new ArgumentException("Unknown color: " + colorRaw.ToString());
            //}
        }

        public byte GetColorData(int x, int y)
        {
            bool upper = RawData[y].GetBit(7 - x);
            bool lower = RawData[y + 1].GetBit(7 - x);

            byte color = 0;
            color = color.SetBit(0, upper);
            color = color.SetBit(1, lower);

            return color;
        }

        public Bitmap GetTile()
        {
            Bitmap bitmap = new Bitmap(8, 8);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = GetPixelColor(x, y);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap;
        }
    }
}
