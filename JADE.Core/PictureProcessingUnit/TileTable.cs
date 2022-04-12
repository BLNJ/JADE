using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.Core.PictureProcessingUnit
{
    public class TileTable
    {
        Registers.LCDControlRegisters.MemoryRegion memoryRegion;

        public TileTable(ref Registers.LCDControlRegisters.MemoryRegion memoryRegion)
        {
            this.memoryRegion = memoryRegion;
        }

        //public Tile GetTile(byte index)
        //{

        //}

        //public Tile GetTile(sbyte index)
        //{

        //}

        public class Tile
        {

        }
    }
}
