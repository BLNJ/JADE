using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.Core.MemoryManagementUnit
{
    public class MemoryManagementUnitStream : Stream
    {
        static readonly object locker = new object();

        private MMU mmu;

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        private long length;
        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        long position = 0;
        public override long Position
        {
            get
            {
                lock (locker)
                {
                    return this.position;
                }
            }
            set
            {
                lock (locker)
                {
                    this.position = value;
                }
            }
        }

        public MemoryManagementUnitStream(MMU mmu, long length) : base()
        {
            this.mmu = mmu;
            this.length = length;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        #region Read
        public ushort ReadUShort()
        {
            return ReadUShort(this.Position);
        }
        public ushort ReadUShort(long offset, bool jumpBack = false)
        {
            byte[] buffer = ReadBytes(offset, 2, jumpBack: jumpBack);
            ushort value = BitConverter.ToUInt16(buffer, 0);
            return value;
        }
        public short ReadShort()
        {
            return ReadShort(this.Position);
        }
        public short ReadShort(long offset, bool jumpBack = false)
        {
            byte[] buffer = ReadBytes(offset, 2, jumpBack: jumpBack);
            short value = BitConverter.ToInt16(buffer, 0);
            return value;
        }
        public byte[] ReadBytes(int count)
        {
            return ReadBytes(this.Position, count);
        }
        public byte[] ReadBytes(long offset, int count, bool jumpBack = false)
        {
            long oldPosition = this.Position;

            byte[] buffer = new byte[count];
            for (int i = 0; i < count; i++)
            {
                buffer[i] = ReadByte(offset + i);
            }

            if(jumpBack)
            {
                this.Position = oldPosition;
            }

            return buffer;
        }
        public sbyte ReadSByte()
        {
            return ReadSByte(this.Position);
        }
        public sbyte ReadSByte(long offset, bool jumpBack = false)
        {
            byte buffer = ReadByte(offset, jumpBack);
            sbyte converted = (sbyte)buffer;

            //if (buffer > sbyte.MaxValue)
            //{
            //    converted = (sbyte)(buffer - 256);
            //}
            //else
            //{
            //    converted = (sbyte)buffer;
            //}

            //sbyte converted = Convert.ToSByte(buffer);
            return converted;
        }
        public new byte ReadByte()
        {
            return ReadByte(this.Position);
        }
        public byte ReadByte(long offset, bool jumpBack = false)
        {
            byte[] buffer = new byte[1];

            int ret;
            lock (locker)
            {
                long oldPosition = this.Position;

                this.Position = offset;
                ret = Read(buffer, 0, 1);

                if(jumpBack)
                {
                    this.Position = oldPosition;
                }
            }

            if (ret != 1)
            {
                throw new Exception("ehm");
            }
            else
            {
                return buffer[0];
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            else
            {
                if (buffer.Length < count)
                {
                    throw new ArgumentException("count is larger then buffer");
                }
                if (buffer.Length < offset)
                {
                    throw new ArgumentException("");
                }

                int internalCount = 0;
                while (internalCount < count)
                {
                    if (this.Position >= this.Length)
                    {
                        throw new EndOfStreamException();
                    }

                    lock (locker)
                    {
                        MappedMemoryRegion mappedMemory = this.mmu.FindMappedMemory((ushort)(this.Position));
                        if (mappedMemory == null)
                        {
                            throw new Exception("oh boy");
                        }
                        else
                        {
                            mappedMemory.ExternalMemory.Position = (this.Position - mappedMemory.Start);
                            mappedMemory.ExternalMemory.Read(buffer, offset + internalCount, 1);
                            internalCount++;
                            Position++;
                        }
                    }
                }

                return internalCount;
            }
        }
        #endregion

        #region Write
        public void WriteUShort(ushort value)
        {
            this.WriteUShort(this.Position, value);
        }
        public void WriteUShort(long offset, ushort value, bool jumpBack = false)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            this.WriteBytes(offset, buffer, jumpBack: jumpBack);
        }
        public void WriteShort(short value)
        {
            this.WriteShort(this.Position, value);
        }
        public void WriteShort(long offset, short value, bool jumpBack = false)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            this.WriteBytes(offset, buffer, jumpBack: jumpBack);
        }
        public void WriteBytes(byte[] value)
        {
            this.WriteBytes(this.Position, value);
        }
        public void WriteBytes(long offset, byte[] value, bool jumpBack = false)
        {
            long oldPosition = this.Position;

            for (int i = 0; i < value.Length; i++)
            {
                this.WriteByte(offset + i, value[i]);
            }

            if(jumpBack)
            {
                this.Position = oldPosition;
            }
        }
        public new void WriteByte(byte value)
        {
            this.WriteByte(this.Position, value);
        }
        public void WriteByte(long offset, byte value, bool jumpBack = false)
        {
            long oldPosition = this.Position;

            this.Position = offset;
            this.Write(new byte[] { value }, 0, 1);

            if(jumpBack)
            {
                this.Position = oldPosition;
            }
        }
        public void Write(object value)
        {
            Write(this.Position, value);
        }
        public void Write(long offset, object value)
        {
            Type valueType = value.GetType();
            if(valueType == typeof(ushort))
            {
                this.WriteUShort(offset, (ushort)value);
            }
            else if(valueType == typeof(short))
            {
                this.WriteShort(offset, (short)value);
            }
            else if(valueType == typeof(byte[]))
            {
                this.WriteBytes(offset, (byte[])value);
            }
            else if(valueType == typeof(byte))
            {
                this.WriteByte(offset, (byte)value);
            }
            else
            {
                throw new NotImplementedException("Unimplemented type: " + valueType);
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            else
            {
                if (buffer.Length < count)
                {
                    throw new ArgumentException("count is larger then buffer");
                }
                if (buffer.Length < offset)
                {
                    throw new ArgumentException("");
                }

                int internalCount = 0;
                while (internalCount < count)
                {
                    if (this.Position >= this.Length)
                    {
                        throw new EndOfStreamException();
                    }
                    lock (locker)
                    {
                        MappedMemoryRegion mappedMemory = this.mmu.FindMappedMemory((ushort)(this.Position));
                        if (mappedMemory == null)
                        {
                            throw new Exception("oh boy");
                        }
                        else
                        {
                            mappedMemory.ExternalMemory.Position = (this.Position - mappedMemory.Start);
                            mappedMemory.ExternalMemory.Write(buffer, offset + internalCount, 1);
                            internalCount++;
                            Position++;
                        }
                    }
                }
            }
        }
        #endregion

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newOffset = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newOffset = (0 + offset);
                    break;
                case SeekOrigin.Current:
                    newOffset = (Position + offset);
                    break;
                case SeekOrigin.End:
                    newOffset = (Length - offset);
                    break;
            }
            this.Position = newOffset;

            return newOffset;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
