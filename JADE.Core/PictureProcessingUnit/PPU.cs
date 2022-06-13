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

        public byte CurrentScanline
        {
            get;
            private set;
        }

        internal TileData[] tileData;
        private SpriteAttributeTable spriteAttributeTable;

        private Bitmap backgroundBitmap;
        private Bitmap windowBitmap;

        internal byte[] VRAMRaw;
        internal MemoryStream VRAM;
        MemoryManagementUnit.MappedMemoryRegion VRAMRegion;
        internal byte[] OAMRaw;
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
            this.VRAMRaw = new byte[0x2000];
            this.VRAM = new MemoryStream(this.VRAMRaw);
            //this.VRAM = new IO.FilledMemoryStream(0x2000, random: false); //TODO for dev purposes random is set to OFF (the bootstrap takes too long for the loop)
            VRAMRegion = this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.VRAM, 0x8000, this.VRAM);

            //OAM
            this.OAMRaw = new byte[0xA0];
            this.OAM = new MemoryStream(this.OAMRaw);
            //this.OAM = new IO.FilledMemoryStream(0xA0, random: false);
            OAMRegion = this.device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.OAM, 0xFE00, this.OAM);

            this.LCDControlRegisters.Reset();
            this.LCDStatusRegisters.Reset();

            //TODO this is temporary for Debugging
            this.LCDControlRegisters.LCDEnabled = false;

            this.CurrentScanline = 0;
            this.cycleProgress = 0;
        }

        int cycleProgress = 0;
        public void Cycle(byte usedCPUCycles)
        {
            //Viewport?

            byte internalCount = 0;
            bool drawFrame = false;

            while (internalCount < usedCPUCycles)
            {
                if (LCDControlRegisters.LCDEnabled)
                {
                    switch (this.LCDStatusRegisters.Mode)
                    {
                        case Registers.LCDStatusRegisters.ModeFlag.SearchingOAM:
                            if (cycleProgress < 80)
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
                            if (cycleProgress < 289)
                            {
                                cycleProgress++;
                            }
                            else
                            {
                                drawFrame = true;
                                //this.OnPictureDrawn(null);
                                cycleProgress = 0;
                                this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.HBlank;
                            }
                            break;
                        case Registers.LCDStatusRegisters.ModeFlag.HBlank:
                            if (cycleProgress == 0)
                            {
                                this.CurrentScanline = 0;
                                //this.LCDC_YCoordinate = 0;
                                this.LCDPosition.LY = 0;
                            }

                            if (cycleProgress < 204)
                            {
                                this.CurrentScanline++;
                                //this.LCDC_YCoordinate++;
                                this.LCDPosition.LY++;

                                //if (LCDC_YCoordinate == LineYCompare)
                                //{
                                //    this.LCDStatusRegisters.CoincidenceFlag = true;
                                //}
                                if (this.LCDPosition.LY == this.LCDPosition.LYC)
                                {
                                    this.LCDStatusRegisters.CoincidenceFlag = true;
                                }

                                cycleProgress++;
                            }
                            else
                            {
                                cycleProgress = 0;
                                this.LCDStatusRegisters.Mode = Registers.LCDStatusRegisters.ModeFlag.VBlank;
                            }
                            break;
                        case Registers.LCDStatusRegisters.ModeFlag.VBlank:
                            if (cycleProgress == 0)
                            {
                                this.device.CPU.InterruptEnabled.VBlank = true;
                            }

                            if (cycleProgress < 4560)
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

                internalCount++;
            }

            if(drawFrame)
            {
                this.OnPictureDrawn(null);
            }
        }

        public Bitmap DrawBackground()
        {
            Bitmap bitmap = DrawTileMap(this.LCDControlRegisters.BackgroundTileMapRegion, ref this.backgroundBitmap);
            DrawWindowBorder(bitmap);
            return bitmap;
        }

        public Bitmap DrawWindow()
        {
            Bitmap bitmap = DrawTileMap(this.LCDControlRegisters.WindowTileTable, ref this.windowBitmap);
            DrawWindowBorder(bitmap);

            return bitmap;
        }

        public Bitmap DrawTileMap(Registers.LCDControlRegisters.MemoryRegion region, ref Bitmap bitmap)
        {
            //if(bitmap == null)
            //{
                bitmap = new Bitmap(256, 256);
            //}

            using (Graphics graphics = Graphics.FromImage(backgroundBitmap))
            {
                long position = (region.Start - 0x8000);
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        this.VRAM.Position = position;
                        byte indexValue = (byte)this.VRAM.ReadByte(); //this.device.MMU.Stream.ReadByte(position, jumpBack: true);

                        TileData tileData = null;

                        for (int i = 0; i < this.tileData.Length; i++)
                        {
                            if (this.tileData[i].Index == indexValue)
                            {
                                tileData = this.tileData[i];
                            }
                        }

                        if (tileData != null)
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

        public void DrawWindowBorder(Bitmap bitmap)
        {
            using(Graphics graphics = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(Color.Pink);
                //Upper line
                graphics.DrawLine(pen, this.LCDPosition.WindowX + this.LCDPosition.ScrollX, this.LCDPosition.WindowY + this.LCDPosition.ScrollY, (this.LCDPosition.WindowX + this.LCDPosition.ScrollX + LCDWidth), this.LCDPosition.WindowY + this.LCDPosition.ScrollY);
                //Left line
                graphics.DrawLine(pen, this.LCDPosition.WindowX + this.LCDPosition.ScrollX, this.LCDPosition.WindowY + this.LCDPosition.ScrollY, this.LCDPosition.WindowX + this.LCDPosition.ScrollX, (this.LCDPosition.WindowY + this.LCDPosition.ScrollY + LCDHeight));
                //Right line
                graphics.DrawLine(pen, (this.LCDPosition.WindowX + this.LCDPosition.ScrollX + LCDWidth), this.LCDPosition.WindowY + this.LCDPosition.ScrollY, (this.LCDPosition.WindowX + this.LCDPosition.ScrollX + LCDWidth), (this.LCDPosition.WindowY + this.LCDPosition.ScrollY + LCDHeight));
                //Bottom line
                graphics.DrawLine(pen, this.LCDPosition.WindowX + this.LCDPosition.ScrollX, (this.LCDPosition.WindowY + this.LCDPosition.ScrollY + LCDHeight), (this.LCDPosition.WindowX + this.LCDPosition.ScrollX + LCDWidth), (this.LCDPosition.WindowY + this.LCDPosition.ScrollY + LCDHeight));

                graphics.DrawString(string.Format("{0}:{1}", this.LCDPosition.ScrollY, this.LCDPosition.ScrollX), new Font("Arial", 6), Brushes.Pink, (this.LCDPosition.WindowX + this.LCDPosition.ScrollX), (this.LCDPosition.WindowY + this.LCDPosition.ScrollY));
            }
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
