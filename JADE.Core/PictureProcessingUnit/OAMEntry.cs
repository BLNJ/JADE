using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JADE.Core.PictureProcessingUnit
{
    public class OAMEntry
    {
        private SpriteAttributeTable spriteAttributeTable;
        public int Index
        {
            get;
            private set;
        }

        public byte PositionY
        {
            get
            {
                return spriteAttributeTable.ReadOAMByte(this.Index, 0);
            }
            set
            {
                spriteAttributeTable.WriteOAMByte(this.Index, 0, value);
            }
        }
        public byte PositionX
        {
            get
            {
                return spriteAttributeTable.ReadOAMByte(this.Index, 1);
            }
            set
            {
                spriteAttributeTable.WriteOAMByte(this.Index, 1, value);
            }
        }
        public byte TileNumber
        {
            get
            {
                return spriteAttributeTable.ReadOAMByte(this.Index, 2);
            }
            set
            {
                spriteAttributeTable.WriteOAMByte(this.Index, 2, value);
            }
        }

        public byte Flags
        {
            get
            {
                return spriteAttributeTable.ReadOAMByte(this.Index, 3);
            }
            set
            {
                spriteAttributeTable.WriteOAMByte(this.Index, 3, value);
            }
        }

        public bool HasPriority
        {
            get
            {
                return this.Flags.GetBit(7);
            }
            set
            {
                this.Flags.SetBit(7, value);
            }
        }
        public bool FlipY
        {
            get
            {
                return this.Flags.GetBit(6);
            }
            set
            {
                this.Flags.SetBit(6, value);
            }
        }
        public bool FlipX
        {
            get
            {
                return this.Flags.GetBit(5);
            }
            set
            {
                this.Flags.SetBit(5, value);
            }
        }
        public Palette PaletteNumber
        {
            get
            {
                bool bit = this.Flags.GetBit(4);

                if (bit)
                {
                    return Palette.OBJ1PAL;
                }
                else
                {
                    return Palette.OBJ0PAL;
                }
            }
            set
            {
                if (value == Palette.OBJ1PAL)
                {
                    this.Flags.SetBit(4, true);
                }
                else if (value == Palette.OBJ0PAL)
                {
                    this.Flags.SetBit(4, false);
                }
                else
                {
                    throw new NotImplementedException("Unknown value:" + value.ToString());
                }
            }
        }

        public ColorPalette ColorPalette
        {
            get
            {
                switch (PaletteNumber)
                {
                    case Palette.OBJ0PAL:
                        return this.spriteAttributeTable.ppu.ObjectPaletteData0;
                    case Palette.OBJ1PAL:
                        return this.spriteAttributeTable.ppu.ObjectPaletteData1;
                    default:
                        throw new NotImplementedException(PaletteNumber.ToString());
                }
            }
        }

        public Point Position
        {
            get
            {
                return new Point(this.PositionX, this.PositionY);
            }
            set
            {
                this.PositionX = (byte)value.X;
                this.PositionY = (byte)value.Y;
            }
        }

        public OAMEntry(SpriteAttributeTable spriteAttributeTable, int index)
        {
            this.spriteAttributeTable = spriteAttributeTable;
            this.Index = index;
        }

        public enum Palette
        {
            OBJ0PAL,
            OBJ1PAL
        }
    }
}
