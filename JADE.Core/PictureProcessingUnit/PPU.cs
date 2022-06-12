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
                byte value = this.device.MMU.Stream.ReadByte(0xFF46, jumpBack: true);
                ushort address = (ushort)(value * 0x100);

                return address;
            }
            set
            {
                byte address = (byte)(value / 0x100);
                this.device.MMU.Stream.WriteByte(0xFF46, address, jumpBack: true);
            }
        }
        /// <summary>
        /// LY
        /// </summary>
        public byte LCDC_YCoordinate
        {
            get
            {
                byte value = this.device.MMU.Stream.ReadByte(0xFF44, jumpBack: true);
                return value;
            }
            set
            {
                this.device.MMU.Stream.WriteByte(0xFF44, value, jumpBack: true);
            }
        }
        /// <summary>
        /// LYC
        /// </summary>
        public byte LineYCompare
        {
            get
            {
                byte value = this.device.MMU.Stream.ReadByte(0xFF45, jumpBack: true);
                return value;
            }
            set
            {
                this.device.MMU.Stream.WriteByte(0xFF45, value, jumpBack: true);
            }
        }

        public byte CurrentScanline
        {
            get;
            private set;
        }

        internal TileData[] tileData;
        private SpriteAttributeTable spriteAttributeTable;

        private Bitmap backgroundBitmap;
        private Bitmap windowBitmap;

        internal MemoryStream VRAM;
        MemoryManagementUnit.MappedMemoryRegion VRAMRegion;
        internal MemoryStream OAM;
        MemoryManagementUnit.MappedMemoryRegion OAMRegion;

        public TileTable OBJTileTable;
        public TileTable NotOBJTileTable;
        public TileTable EverythingTileTable;

        public PPU(Device device)
        {
            this.device = device;
            this.LCDControlRegisters = new Registers.LCDControlRegisters(this.device.MMU);
            this.LCDStatusRegisters = new Registers.LCDStatusRegisters(this.device.MMU);
            this.LCDPosition = new Registers.LCDPositionScrolling(this.device.MMU);

            this.OBJTileTable = new TileTable(this, TileTable.Purpose.OBJ);
            this.NotOBJTileTable = new TileTable(this, TileTable.Purpose.LiterallyEverythingElse);
            this.EverythingTileTable = new TileTable(this, TileTable.Purpose.All);

            this.BGPaletteData = new ColorPalette(this, 0xFF47, lowestTransparent: false);
            this.ObjectPaletteData0 = new ColorPalette(this, 0xFF48);
            this.ObjectPaletteData1 = new ColorPalette(this, 0xFF49);
        }

        public void Reset()
        {
            this.tileData = new TileData[384];
            for (int i = 0; i < this.tileData.Length; i++)
            {
                this.tileData[i] = new TileData(this, i);
            }

            this.spriteAttributeTable = new SpriteAttributeTable(this);

            //VRAM
            this.VRAM = new IO.FilledMemoryStream(0x2000, random: false); //TODO for dev purposes random is set to OFF (the bootstrap takes too long for the loop)
            VRAMRegion = this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.VRAM, 0x8000, this.VRAM);

            //OAM
            this.OAM = new IO.FilledMemoryStream(0xA0, random: false);
            OAMRegion = this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.OAM, 0xFE00, this.OAM);

            this.LCDControlRegisters.Reset();
            this.LCDStatusRegisters.Reset();

            //TODO this is temporary for Debugging
            this.LCDControlRegisters.LCDEnabled = false;

            this.CurrentScanline = 0;
            this.cycleProgress = 0;
        }

        int cycleProgress = 0;
        public void Cycle()
        {
            //Viewport?

            if (LCDControlRegisters.LCDEnabled)
            {
                switch (this.LCDStatusRegisters.Mode)
                {
                    case Registers.LCDStatusRegisters.ModeFlag.SearchingOAM:
                        if(cycleProgress < 80)
                        {
                            cycleProgress++;
                            return;
                        }
                        else
                        {
                            cycleProgress = 0;
                            this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.TransferingData;
                        }
                        break;
                    case Registers.LCDStatusRegisters.ModeFlag.TransferingData:
                        if(cycleProgress < 289)
                        {
                            cycleProgress++;
                        }
                        else
                        {
                            this.OnPictureDrawn(null);
                            cycleProgress = 0;
                            this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.HBlank;
                        }
                        break;
                    case Registers.LCDStatusRegisters.ModeFlag.HBlank:
                        if(cycleProgress < 87)
                        {
                            cycleProgress++;
                        }
                        else
                        {
                            cycleProgress = 0;
                            this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.VBlank;
                        }
                        break;
                    case Registers.LCDStatusRegisters.ModeFlag.VBlank:
                        if(cycleProgress < 4560)
                        {
                            cycleProgress++;
                        }
                        else
                        {
                            cycleProgress = 0;
                            this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.SearchingOAM;
                        }
                        break;


                    default:
                        throw new NotImplementedException("Unimplemented PPU Mode: " + this.LCDStatusRegisters.Mode);
                }

                if (LCDControlRegisters.BackgroundDisplay)
                {
                }
            }
        }

        public Bitmap DrawBackground()
        {
            return DrawTileMap(this.LCDControlRegisters.BackgroundTileMapRegion);
        }

        public Bitmap DrawWindow()
        {
            return DrawTileMap(this.LCDControlRegisters.WindowTileTable);
        }

        public Bitmap DrawTileMap(Registers.LCDControlRegisters.MemoryRegion region)
        {
            Bitmap bitmap = new Bitmap(256, 256);
            Graphics graphics = Graphics.FromImage(bitmap);

            long position = region.Start;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    byte indexValue = this.device.MMU.Stream.ReadByte(position, jumpBack: true);

                    TileData tileData = null;

                    for(int i = 0; i < this.tileData.Length; i++)
                    {
                        if (this.tileData[i].Index == indexValue)
                        {
                            tileData = this.tileData[i];
                        }
                    }

                    if(tileData != null)
                    {
                        Bitmap tileBitmap = tileData.GenerateBitmap();
                        graphics.DrawImage(tileBitmap, (x * tileBitmap.Width), (y * tileBitmap.Height));

                        position++;

                    }
                    else
                    {
                        throw new Exception("TileData index not found: " + indexValue);
                    }
                }
            }

            return bitmap;
        }

        public Bitmap DrawTileData()
        {
            int rows = (this.tileData.Length / 16);

            Bitmap bitmap = new Bitmap(16 * 8, rows * 8);
            Graphics graphics = Graphics.FromImage(bitmap);

            int counter = 0;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    TileData tileData = this.tileData[counter];//[(y * 8) + x];

                    Bitmap tileBitmap = tileData.GenerateBitmap();
                    graphics.DrawImage(tileBitmap, (x * tileBitmap.Width), (y * tileBitmap.Height));
                    counter++;
                }
            }

            return bitmap;
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
