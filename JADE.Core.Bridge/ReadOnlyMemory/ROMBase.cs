using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JADE.Core.Bridge.ReadOnlyMemory
{
    public abstract class ROMBase : DeviceBaseComponent
    {
        public Stream Stream
        {
            get;
            private set;
        }

        public ROMBase(DeviceBase device) : base(device)
        {
        }

        public void Open(Stream stream)
        {
            this.Stream = stream;
        }

        public void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }
            else
            {
                FileStream fs = File.OpenRead(filePath);
                this.Open(fs);
            }
        }

        public abstract void Read();
    }
}
