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
        public const byte SizeX = 8;
        public const byte SizeY = 8;

        PPU ppu;
        MemoryManagementUnit.MMU mmu
        {
            get
            {
                return ppu.device.MMU;
            }
        }

        Bitmap cacheBitmap;

        public int Index
        {
            get;
            private set;
        }
        public ushort Address
        {
            get
            {
                return (ushort)(0x8000 + (this.Index * 16));
            }
        }

        private byte[] RawData
        {
            get
            {
                return this.ppu.VRAMRaw.AsMemory(this.Index * 16, 16).ToArray();
                //return mmu.Stream.ReadBytes(Address, 16, jumpBack: true);
            }
        }

        public TileData(PPU ppu, int index)
        {
            this.ppu = ppu;
            this.Index = index;
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
            byte[] lineData = GetLineData(y);

            bool upper = lineData[0].GetBit(7 - x);
            bool lower = lineData[1].GetBit(7 - x);

            byte color = 0;
            color = color.SetBit(0, upper);
            color = color.SetBit(1, lower);

            return color;
        }

        public byte[] GetLineData(int line)
        {
            byte[] data = new byte[2];
            data[0] = this.RawData[(line * 2)];
            data[1] = this.RawData[(line * 2) + 1];

            return data;
        }

        public Bitmap GenerateBitmap()
        {
            this.cacheBitmap = new Bitmap(SizeX, SizeY);

            for (int y = 0; y < this.cacheBitmap.Height; y++)
            {
                for (int x = 0; x < this.cacheBitmap.Width; x++)
                {
                    Color pixelColor = GetPixelColor(x, y);
                    this.cacheBitmap.SetPixel(x, y, pixelColor);
                }
            }

            return this.cacheBitmap;
        }

        public Color[][] GenerateColorMap()
        {
            Color[][] data = new Color[8][];
            for(int y = 0; y < data.Length; y++)
            {
                data[y] = new Color[8];
                for(int x = 0; x < data[y].Length; x++)
                {
                    Color pixelColor = GetPixelColor(x, y);
                    data[y][x] = pixelColor;
                }
            }

            return data;
        }
    }
}
