using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.IO
{
    public class TriggerStream : Stream
    {
        public delegate void OnTrigger(TriggerStream triggerStream, long offset, byte[] buffer);

        private List<Subscriber> subscribers;

        Stream baseStream;

        public override bool CanRead
        {
            get
            {
                return baseStream.CanRead;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return baseStream.CanSeek;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return baseStream.CanWrite;
            }
        }
        public override long Length
        {
            get
            {
                return baseStream.Length;
            }
        }
        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                baseStream.Position = value;
            }
        }

        public TriggerStream(Stream stream) : base()
        {
            this.baseStream = stream;
            this.subscribers = new List<Subscriber>();
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long position = this.Position;
            int read = baseStream.Read(buffer, offset, count);
            Notify(true, position, buffer, offset, count, read);

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long position = this.Position;
            baseStream.Write(buffer, offset, count);
            Notify(false, position, buffer, offset, count, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public void Notify(bool isRead, long position, byte[] buffer, int offset, int count, int copyCount)
        {
            byte[] arrayCopy = new byte[copyCount];
            Array.Copy(buffer, offset, arrayCopy, 0, copyCount);

            for (int i = 0; i < this.subscribers.Count; i++)
            {
                if (this.subscribers[i].Position == position)
                {
                    if (isRead && this.subscribers[i].ReadTrigger != null)
                    {
                        this.subscribers[i].ReadTrigger.Invoke(this, position, arrayCopy);
                    }
                    if (!isRead && this.subscribers[i].WriteTrigger != null)
                    {
                        this.subscribers[i].WriteTrigger.Invoke(this, position, arrayCopy);
                    }
                }
            }
        }

        public void Subscribe(long position, OnTrigger onRead = null, OnTrigger onWrite = null)
        {
            if (onRead == null && onWrite == null)
            {
                throw new ArgumentNullException("onRead & onWrite");
            }
            if (position <= 0)
            {
                throw new ArgumentOutOfRangeException("position", "Value 0 or negative");
            }

            bool exists = false;
            for (int i = 0; i < this.subscribers.Count; i++)
            {
                if (this.subscribers[i].Position == position)
                {
                    if (this.subscribers[i].ReadTrigger == onRead || this.subscribers[i].WriteTrigger == onWrite)
                    {
                        exists = true;
                    }
                }
            }

            if (!exists)
            {
                this.subscribers.Add(new Subscriber(position)
                {
                    ReadTrigger = onRead,
                    WriteTrigger = onWrite
                });
            }
            else
            {
                throw new Exception("Already subscribed");
            }
        }

        public void Unsubscibe(long position, OnTrigger onRead = null, OnTrigger onWrite = null)
        {
            throw new NotImplementedException();
        }

        public class Subscriber
        {
            public long Position
            {
                get;
                private set;
            }
            public OnTrigger ReadTrigger;
            public OnTrigger WriteTrigger;

            public Subscriber(long position)
            {
                this.Position = position;
            }
        }
    }
}
