using System;
using System.Collections.Generic;
using System.Text;
using JADE.IO;
using System.IO;

namespace JADE.Core.MemoryManagementUnit
{
    public class MappedMemory
    {
        public List<MappedMemoryRegion> MemoryRegions
        {
            get;
            private set;
        }

        public MappedMemory()
        {
            this.MemoryRegions = new List<MappedMemoryRegion>();
        }

        public void Reset()
        {
            this.MemoryRegions.Clear();
        }

        public MappedMemoryRegion FindMappedMemory(MappedMemoryRegion.Name name)
        {
            for (int i = 0; i < this.MemoryRegions.Count; i++)
            {
                MappedMemoryRegion mappedIO = this.MemoryRegions[i];

                if (mappedIO.RegionName == name)
                {
                    return mappedIO;
                }
            }

            return null;
        }
        public MappedMemoryRegion FindMappedMemory(ushort position)
        {
            for (int i = 0; i < this.MemoryRegions.Count; i++)
            {
                MappedMemoryRegion mappedIO = this.MemoryRegions[i];

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
        public void AddMappedStream(MappedMemoryRegion.Name name, ushort start, ushort length, Stream externalStream, long externalBaseAddress, bool topMost = false)
        {
            MappedMemoryRegion mappedIO = FindMappedMemory(start);

            if (mappedIO != null)
            {
                throw new Exception(string.Format("mappedIO already existing: start:{0}, end:{1}", start, (start + length)));
            }
            else
            {
                if (this.MemoryRegions.Find(map => map.RegionName == name) != null)
                {
                    throw new Exception("Name already exists: " + name);
                }

                ExternalMemory stream = new ExternalMemory(externalStream, externalBaseAddress, length, writable: externalStream.CanWrite);
                mappedIO = new MappedMemoryRegion(name, start, length, stream, topMost);

                this.MemoryRegions.Add(mappedIO);
            }
        }

        public void RemoveMappedStream(MappedMemoryRegion.Name name)
        {
            MappedMemoryRegion mappedMemory = FindMappedMemory(name);
            RemoveMappedStream(mappedMemory);
        }
        public void RemoveMappedStream(MappedMemoryRegion mappedMemory)
        {
            this.MemoryRegions.Remove(mappedMemory);
            mappedMemory.Close();
        }
    }
}
