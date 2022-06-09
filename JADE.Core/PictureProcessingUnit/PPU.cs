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

        public void ParseTileMaps()
        {
            ParseTileMap(this.LCDControlRegisters.BackgroundTileMapRegion);
            ParseTileMap(this.LCDControlRegisters.WindowTileTable);
        }

        private void ParseTileMap(Registers.LCDControlRegisters.MemoryRegion region)
        {
            ushort currentAddress = 0x8000;

            List<TileData> tileDatas = new List<TileData>();
            for (int i = 0; i < (0x180); i++)
            {
                TileData tileData = new TileData(this, currentAddress);
                tileDatas.Add(tileData);
                currentAddress += 16;
            }

            Bitmap bitmap = new Bitmap(256, 256);

            this.device.MMU.Stream.Position = region.Start;


            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    byte indexValue = this.device.MMU.Stream.ReadByte();

                    TileData tileData = tileDatas[indexValue];
                    Bitmap tileBitmap = tileData.GetTile();

                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.DrawImage(tileBitmap, (x * tileBitmap.Width), (y * tileBitmap.Height));
                }
            }

            bitmap.Save(string.Format(@".\tilemap\tilemap_{0}.png", region.Start), System.Drawing.Imaging.ImageFormat.Png);
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
