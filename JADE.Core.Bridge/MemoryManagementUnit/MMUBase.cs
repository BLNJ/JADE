using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JADE.IO;

namespace JADE.Core.Bridge.MemoryManagementUnit
{
    public abstract class MMUBase : DeviceBaseComponent
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

        public MMUBase(DeviceBase device, long length) : base(device)
        {
            this.MappedMemory = new List<MappedMemoryRegion>();
            this.Stream = new MemoryManagementUnitStream(this, length);
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
    }
}
