using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace JADE.Core.PictureProcessingUnit
{
    public class TileTable
    {
        PPU ppu;
        Purpose purpose;

        public TileTable(PPU ppu, Purpose purpose)
        {
            this.ppu = ppu;
            this.purpose = purpose;
        }

        public Bitmap DrawTileTable()
        {
            if (this.purpose != Purpose.All)
            {
                return DrawTileTable(16, 32);
            }
            else
            {
                int rows = (this.ppu.tileData.Length / 16);
                return DrawTileTable(16, rows);
            }
        }

        public Bitmap DrawTileTable(int tilesAmountX, int tilesAmountY)
        {
            TileData[] tileDatas = this.GenerateTileData();

            if(tileDatas.Length != (tilesAmountX * tilesAmountY))
            {
                throw new ArgumentException("tilesAmountX + tilesAmountY does not equal " + tileDatas.Length);
            }
            else
            {
                Bitmap bitmap = new Bitmap((tilesAmountX * TileData.SizeX), (tilesAmountY * TileData.SizeY));

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    for (int y = 0; y < tilesAmountY; y++)
                    {
                        for (int x = 0; x < tilesAmountX; x++)
                        {
                            TileData tileData = tileDatas[(y * tilesAmountX) + x];
                            Bitmap tileBitmap = tileData.GenerateBitmap();

                            graphics.DrawImage(tileBitmap, x * TileData.SizeX, y * TileData.SizeY);
                        }
                    }
                }

                return bitmap;
            }
        }

        public TileData[] GenerateTileData()
        {
            byte[] blockOrder = new byte[2];

            switch(this.purpose)
            {
                case Purpose.OBJ:
                    blockOrder[0] = 0;
                    blockOrder[1] = 1;
                    break;
                case Purpose.LiterallyEverythingElse:
                    if(this.ppu.LCDControlRegisters.TileSetZero)
                    {
                        blockOrder[0] = 2;
                        blockOrder[1] = 1;
                    }
                    else
                    {
                        blockOrder[0] = 0;
                        blockOrder[1] = 1;
                    }
                    break;
                case Purpose.All:
                    blockOrder = new byte[3]
                    {
                        0,
                        1,
                        2
                    };
                    break;

                default:
                    throw new Exception("Unknown purpose: " + purpose);
            }

            //TileData[] tileData = new TileData[blockOrder.Length * 128];
            List<TileData> tileData = new List<TileData>();
            for(int i = 0; i < blockOrder.Length; i++)
            {
                TileData[] buffer = getTileDataBlock(blockOrder[i]);
                //Array.Copy(buffer, 0, tileData, i * 128, 128);
                tileData.AddRange(buffer);
            }
            return tileData.ToArray();
            //return tileData;
        }

        private TileData[] getTileDataBlock(byte block)
        {
            if(block > 2)
            {
                throw new Exception("There are only 3 blocks!");
            }
            else
            {
                TileData[] tileData = new TileData[128];

                ushort startIndex;
                switch(block)
                {
                    case 0:
                        startIndex = 0;
                        break;
                    case 1:
                        startIndex = 128;
                        break;
                    case 2:
                        startIndex = 256;
                        break;

                    default:
                        throw new Exception("Unknown block: " + block);
                }

                for(int i = 0; i < tileData.Length; i++)
                {
                    tileData[i] = this.ppu.tileData[startIndex + i];
                }

                return tileData;
            }
        }

        public TileData GetTileDataByIndex(int index)
        {
            return this.GenerateTileData()[index];
        }

        public enum Purpose
        {
            OBJ,
            LiterallyEverythingElse,
            All
        }
    }
}
