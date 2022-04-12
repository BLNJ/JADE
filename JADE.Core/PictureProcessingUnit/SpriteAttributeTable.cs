using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.Core.PictureProcessingUnit
{
    public class SpriteAttributeTable
    {
        internal PPU ppu;
        
        public MemoryStream OAMMemory
        {
            get
            {
                return ppu.OAM;
            }
        }

        public List<OAMEntry> SpriteAttributes;

        public SpriteAttributeTable(PPU ppu)
        {
            this.ppu = ppu;
            this.SpriteAttributes = new List<OAMEntry>();

            for (int i = 0; i < 40; i++)
            {
                SpriteAttributes.Add(new OAMEntry(this, i));
            }
        }

        internal byte ReadOAMByte(int index, int offset)
        {
            byte[] buffer = new byte[1];

            OAMMemory.Position = ((index * 4) + offset);
            int count = OAMMemory.Read(buffer, 0, 1);
            if (count == 1)
            {
                return buffer[0];
            }
            else
            {
                throw new Exception("fuck");
            }
        }
        internal void WriteOAMByte(int index, int offset, byte value)
        {
            OAMMemory.Position = ((index * 4) + offset);
            OAMMemory.Write(new byte[] { value }, 0, 1);
        }
    }
}
