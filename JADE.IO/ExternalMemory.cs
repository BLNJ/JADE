using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.IO
{
    public class ExternalMemory : Stream
    {
        Stream baseStream;

        long baseAddress;
        long length = 0;


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

        bool canWrite;
        public override bool CanWrite
        {
            get
            {
                return canWrite;
            }
        }

        public override long Length
        {
            get
            {
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return (this.baseStream.Position - this.baseAddress);
            }
            set
            {
                this.baseStream.Position = (this.baseAddress + value);
            }
        }

        public ExternalMemory(Stream baseStream, bool writable = false) : this(baseStream, 0, writable: writable)
        {
        }

        public ExternalMemory(Stream baseStream, long baseAddress, bool writable = false) : this(baseStream, baseAddress, baseStream.Length, writable: writable)
        {
        }

        public ExternalMemory(Stream baseStream, long baseAddress, long length, bool writable = false)
        {
            //this.baseStream = Stream.Synchronized(baseStream);
            this.baseStream = baseStream;
            this.baseStream.Position = baseAddress;
            this.baseAddress = baseAddress;
            this.length = length;
            this.canWrite = writable;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //TODO add error handling
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

                //TODO add Range check
                int ret = 0;
                while (ret < count)
                {
                    if (this.Position >= Length)
                    {
                        throw new EndOfStreamException();
                    }
                    else
                    {
                        ret += baseStream.Read(buffer, offset + ret, 1);
                        //this.Position++;
                    }
                }

                return ret;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!this.CanWrite)
            {
                throw new NotSupportedException();
            }
            else
            {
                baseStream.Write(buffer, offset, count);
                ////TODO add error handling
                //if (buffer == null)
                //{
                //    throw new ArgumentNullException("buffer");
                //}
                //else
                //{
                //    if (buffer.Length < count)
                //    {
                //        throw new ArgumentException("count is larger then buffer");
                //    }
                //    if (buffer.Length < offset)
                //    {
                //        throw new ArgumentException("");
                //    }

                //    //TODO add range check
                //    int rem = 0;
                //    while (rem < count)
                //    {
                //        if (this.Position >= Length)
                //        {
                //            throw new EndOfStreamException();
                //        }
                //        else
                //        {
                //            baseStream.Write(buffer, offset + rem, 1);
                //            //this.Position++;
                //            rem++;
                //        }
                //    }
                //}
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newOffset = 0;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    newOffset = (0 + offset);
                    break;
                case SeekOrigin.Current:
                    newOffset = (this.Position + offset);
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
