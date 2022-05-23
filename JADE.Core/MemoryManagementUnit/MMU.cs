using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JADE.IO;

namespace JADE.Core.MemoryManagementUnit
{
/*
    [0000-3FFF] Cartridge ROM, bank 0
        The first 16,384 bytes of the cartridge program are always available at this point in the memory map. Special circumstances apply:
        [0000-00FF] BIOS
            When the CPU starts up, PC starts at 0000h, which is the start of the 256-byte GameBoy BIOS code. Once the BIOS has run, it is removed from the memory map, and this area of the cartridge rom becomes addressable.
        [0100-014F] Cartridge header
            This section of the cartridge contains data about its name and manufacturer, and must be written in a specific format.
    [4000-7FFF] Cartridge ROM, other banks
        Any subsequent 16k "banks" of the cartridge program can be made available to the CPU here, one by one; a chip on the cartridge is generally used to switch between banks, and make a particular area accessible. The smallest programs are 32k, which means that no bank-selection chip is required.
    [8000-9FFF] Graphics RAM
        Data required for the backgrounds and sprites used by the graphics subsystem is held here, and can be changed by the cartridge program. This region will be examined in further detail in part 3 of this series.
    [A000-BFFF] Cartridge (External) RAM
        There is a small amount of writeable memory available in the GameBoy; if a game is produced that requires more RAM than is available in the hardware, additional 8k chunks of RAM can be made addressable here.
    [C000-DFFF] Working RAM: 
        The GameBoy's internal 8k of RAM, which can be read from or written to by the CPU.
    [E000-FDFF] Working RAM (shadow): 
        Due to the wiring of the GameBoy hardware, an exact copy of the working RAM is available 8k higher in the memory map. This copy is available up until the last 512 bytes of the map, where other areas are brought into access.
    [FE00-FE9F] Graphics: 
        sprite information: Data about the sprites rendered by the graphics chip are held here, including the sprites' positions and attributes.
    [FF00-FF7F] Memory-mapped I/O: 
        Each of the GameBoy's subsystems (graphics, sound, etc.) has control values, to allow programs to create effects and use the hardware. These values are available to the CPU directly on the address bus, in this area.
    [FF80-FFFF] Zero-page RAM: 
        A high-speed area of 128 bytes of RAM is available at the top of memory. Oddly, though this is "page" 255 of the memory, it is referred to as page zero, since most of the interaction between the program and the GameBoy hardware occurs through use of this page of memory.
*/

/// <summary>
/// Memory Management Unit
/// </summary>
    public class MMU
    {
        public List<MappedMemoryRegion> MappedMemory
        {
            get;
            private set;
        }
        public MemoryManagementUnitStream Stream
        {
            get;
            private set;
        }

        public MMU(Device device)
        {
            this.MappedMemory = new List<MappedMemoryRegion>();
            this.Stream = new MemoryManagementUnitStream(this, 0x10000);
        }

        public void Reset()
        {
            for (int i = 0; i < this.MappedMemory.Count; i++)
            {
                //TODO unmap everything
                MappedMemoryRegion memory = this.MappedMemory[i];
                memory.Close();
            }
            this.MappedMemory.Clear();

            //TODO Move Memory Regions to corresponding devices

            //Bootstrap
            AddMappedStream(MappedMemoryRegion.Name.Bootstrap, 0x0, new MemoryStream(Properties.Resources.bootstrap, false));

            //VRAM
            //AddMappedStream(MemoryManagementUnit.MappedMemory.Name.VRAM, 0x8000, 0x9FFF, random: true);
            //RAM
            //AddMappedStream(MemoryManagementUnit.MappedMemory.Name.RAM, 0xC000, 0xDFFF, true);
            //OAM
            //AddMappedStream(MemoryManagementUnit.MappedMemory.Name.OAM, 0xFE00, 0xFE9F, random: false);
            //IO
            //AddMappedStream(MemoryManagementUnit.MappedMemory.Name.IO, 0xFF00, 0xFF7F, random: false);

            //Unusable
            AddMappedStream(MappedMemoryRegion.Name.Unusable, 0xFEA0, 0x60, random: false);

            //Zero-Page
            AddMappedStream(MappedMemoryRegion.Name.ZeroPage, 0xFF80, 0x80, random: false);

            this.Stream.Position = 0;
        }

        public MappedMemoryRegion FindMappedMemory(MappedMemoryRegion.Name name)
        {
            for (int i = 0; i < this.MappedMemory.Count; i++)
            {
                MappedMemoryRegion mappedIO = this.MappedMemory[i];

                if (mappedIO.RegionName == name)
                {
                    return mappedIO;
                }
            }

            return null;
        }
        public MappedMemoryRegion FindMappedMemory(ushort position)
        {
            for (int i = 0; i < this.MappedMemory.Count; i++)
            {
                MappedMemoryRegion mappedIO = this.MappedMemory[i];

                if (mappedIO.Start <= position)
                {
                    if (mappedIO.End - 1 >= position)
                    {
                        return mappedIO;
                    }
                }
            }

            return null;
        }

        public void AddMappedStream(MappedMemoryRegion.Name name, ushort start, ushort length, bool random = false)
        {
            FilledMemoryStream stream = new FilledMemoryStream(length, random);
            AddMappedStream(name, start, length, stream, 0);
        }
        public void AddMappedStream(MappedMemoryRegion.Name name, ushort start, Stream externalStream)
        {
            this.AddMappedStream(name, start, (ushort)externalStream.Length, externalStream, 0);
        }
        public void AddMappedStream(MappedMemoryRegion.Name name, ushort start, ushort length, Stream externalStream, long externalBaseAddress)
        {
            MappedMemoryRegion mappedIO = FindMappedMemory(start);

            if (mappedIO != null)
            {
                throw new Exception(string.Format("mappedIO already existing: start:{0}, end:{1}", start, (start + length)));
            }
            else
            {
                if (this.MappedMemory.Find(map => map.RegionName == name) != null)
                {
                    throw new Exception("Name already exists: " + name);
                }

                ExternalMemory stream = new ExternalMemory(externalStream, externalBaseAddress, length, writable: externalStream.CanWrite);
                mappedIO = new MappedMemoryRegion(name, start, length, stream);

                this.MappedMemory.Add(mappedIO);
            }
        }

        public void RemoveMappedStream(MappedMemoryRegion.Name name)
        {
            MappedMemoryRegion mappedMemory = FindMappedMemory(name);
            RemoveMappedStream(mappedMemory);
        }
        public void RemoveMappedStream(MappedMemoryRegion mappedMemory)
        {
            this.MappedMemory.Remove(mappedMemory);
            mappedMemory.Close();
        }

        public void DumpMMU()
        {
            using (FileStream fs = File.Create(@"D:\Development\GameBoy_Emulator\Files\Debugging\mmu.bin"))
            {
                fs.Position = 0x0;
                for (int i = 0; i < this.Stream.Length; i++)
                {
                    byte buffer = this.Stream.ReadByte(i);
                    fs.WriteByte(buffer);
                }
            }
        }
    }
}
