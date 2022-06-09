using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.MemoryManagementUnit
{
    public class MappedMemoryRegion
    {
        public ushort Start
        {
            get;
            private set;
        }
        public int End
        {
            get
            {
                return (Start + Length);
            }
        }
        public ushort Length
        {
            get;
            private set;
        }
        public bool TopMost
        {
            get;
            private set;
        } = false;
        public Name RegionName
        {
            get;
            private set;
        }
        public int RegionIteration = 0;

        public IO.ExternalMemory ExternalMemory
        {
            get;
            private set;
        }

        public MappedMemoryRegion(Name regionName, ushort start, ushort length, IO.ExternalMemory externalMemory, bool topMost)
        {
            //TODO add error handling
            this.RegionName = regionName;
            this.Start = start;
            this.Length = length;

            this.ExternalMemory = externalMemory;
            this.TopMost = topMost;
        }

        public void Close()
        {
            this.ExternalMemory.Close();
        }

        public override string ToString()
        {
            return string.Format("{0}:{4}: {1}-{2} ({3})", RegionName, Start, End, Length, RegionIteration);
        }

        public enum Name
        {
            Bootstrap,
            CartridgeROM_Bank0,
            CartridgeROM_BankX,
            VRAM,
            CartridgeRAM,
            RAM,
            ShadowRAM,
            OAM,
            Unusable,
            IO,
            ZeroPage
        }
    }
}
