using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace JADE.Core.PictureProcessingUnit
{
    public class PPU
    {
        public const byte LCDHeight = 144;
        public const byte LCDWidth = 160;

        internal Device device;

        Bitmap backgroundBuffer;
        Bitmap outputBuffer;

        public delegate void PPUDraw(object sender, Bitmap image);
        public event PPUDraw PictureDrawn;

        public Registers.LCDControlRegisters LCDControlRegisters
        {
            get;
            private set;
        }
        public Registers.LCDStatusRegisters LCDStatusRegisters
        {
            get;
            private set;
        }
        public Registers.LCDPositionScrolling LCDPosition
        {
            get;
            private set;
        }

        public ColorPalette BGPaletteData;
        public ColorPalette ObjectPaletteData0;
        public ColorPalette ObjectPaletteData1;
        public ushort DMATransferAddress
        {
            get
            {
                byte value = this.device.MMU.Stream.ReadByte(0xFF46);
                ushort address = (ushort)(value * 0x100);

                return address;
            }
            set
            {
                byte address = (byte)(value / 0x100);
                this.device.MMU.Stream.WriteByte(0xFF46, address);
            }
        }
        /// <summary>
        /// LY
        /// </summary>
        public byte LCDC_YCoordinate
        {
            get
            {
                byte value = this.device.MMU.Stream.ReadByte(0xFF44);
                return value;
            }
            set
            {
                this.device.MMU.Stream.WriteByte(0xFF44, value);
            }
        }
        /// <summary>
        /// LYC
        /// </summary>
        public byte LineYCompare
        {
            get
            {
                byte value = this.device.MMU.Stream.ReadByte(0xFF45);
                return value;
            }
            set
            {
                this.device.MMU.Stream.WriteByte(0xFF45, value);
            }
        }

        public byte CurrentScanline
        {
            get;
            private set;
        }

        internal MemoryStream VRAM;
        internal MemoryStream OAM;

        public PPU(Device device)
        {
            this.device = device;
            this.LCDControlRegisters = new Registers.LCDControlRegisters(this.device.MMU);
            this.LCDStatusRegisters = new Registers.LCDStatusRegisters(this.device.MMU);
            this.LCDPosition = new Registers.LCDPositionScrolling(this.device.MMU);

            this.BGPaletteData = new ColorPalette(this, 0xFF47, lowestTransparent: false);
            this.ObjectPaletteData0 = new ColorPalette(this, 0xFF48);
            this.ObjectPaletteData1 = new ColorPalette(this, 0xFF49);
        }

        public void Reset()
        {
            backgroundBuffer = new Bitmap(256, 256);
            outputBuffer = new Bitmap(LCDWidth, LCDHeight);

            //VRAM
            this.VRAM = new IO.FilledMemoryStream(0x2000, random: false); //TODO for dev purposes random is set to OFF (the bootstrap takes too long for the loop)
            this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.VRAM, 0x8000, this.VRAM);

            //OAM
            this.OAM = new IO.FilledMemoryStream(0xA0, random: false);
            this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.OAM, 0xFE00, this.OAM);

            this.LCDControlRegisters.Reset();
            this.LCDStatusRegisters.Reset();

            this.CurrentScanline = 0;
        }

        public void Step()
        {
            //var range = this.LCDControlRegisters.BGTileMapDisplaySelect;
            //for (int y = 0; y < bitmap.Height; y++)
            //{
            //    for (int x = 0; x < bitmap.Width; x++)
            //    {
            //        //bitmap.SetPixel(x, (bitmap.Height - 1) - y, Color.White);

            //        if (this.LCDControlRegisters.BackgroundDisplay)
            //        {
            //            int xScroll = this.LCDPosition.ScrollX + x;
            //            int yScroll = this.LCDPosition.ScrollY + y;

            //            if (xScroll > 256)
            //            {
            //                xScroll -= 256;
            //            }
            //            if (yScroll > 256)
            //            {
            //                yScroll -= 256;
            //            }
            //        }
            //        if (this.LCDControlRegisters.SpriteDisplayEnable)
            //        {

            //        }
            //    }

            //    //H-Blank
            //    this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.HBlank;
            //}

            //OnPictureDrawn(bitmap);
            ////V-Blank
            //this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.VBlank;


            //Viewport?

            if (LCDControlRegisters.LCDEnabled)
            {
                if (LCDControlRegisters.BackgroundDisplay)
                {
                    //var range = this.LCDControlRegisters.BackgroundTileMapRegion;
                    //byte[] backgroundTileMap = this.device.MMU.Stream.ReadBytes(range.Start, (range.End - range.Start));
                    //byte[] test = this.device.MMU.Stream.ReadBytes(0x8000, 0x2000);
                }

                //OnPictureDrawn(GenerateBackground());
            }
        }

        private void UpdateBackground()
        {

        }

        public void DumpVRAM()
        {
            byte[] buffer = this.VRAM.ToArray();
            File.WriteAllBytes(@"D:\Development\GameBoy_Emulator\Files\Debugging\vram.bin", buffer);
        }

        public void DumpBackgroundTileMap()
        {
            int length = (this.LCDControlRegisters.BackgroundTileMapRegion.End - this.LCDControlRegisters.BackgroundTileMapRegion.Start);
            byte[] buffer = new byte[length];

            buffer = this.device.MMU.Stream.ReadBytes(this.LCDControlRegisters.BackgroundTileMapRegion.Start, length);
            File.WriteAllBytes(@"D:\Development\GameBoy_Emulator\Files\Debugging\backgroundTileMap.bin", buffer);
        }

        public Bitmap GenerateBackground()
        {
            var range = this.LCDControlRegisters.BackgroundTileMapRegion;
            for (int y = 0; y < backgroundBuffer.Height; y++)
            {
                for (int x = 0; x < backgroundBuffer.Width; x++)
                {
                    int tileX = x / 8;
                    int tileY = y / 8;

                    int tilePixelX = x - (tileX * 8);
                    int tilePixelY = y - (tileY * 8);

                    TileData tileData = new TileData(this, (ushort)(range.Start + (x + y)));
                    Color pixel = tileData.GetPixelColor(tilePixelX, tilePixelY);
                    backgroundBuffer.SetPixel(x, y, pixel);
                }
            }

            return backgroundBuffer;
        }

        public void SaveBackground()
        {
            Bitmap bitmap = GenerateBackground();

            bitmap.Save(@"D:\Development\GameBoy_Emulator\Files\test.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        protected virtual void OnPictureDrawn(Bitmap image)
        {
            PPUDraw handler = this.PictureDrawn;
            if (handler != null)
            {
                handler(this, image);
            }
        }
    }
}
