using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.IO
{
    public class FilledMemoryStream : MemoryStream
    {
        public FilledMemoryStream(byte[] byteArray, bool random = false) : base(byteArray)
        {
            if (random)
            {
                FillRandom();
            }
            else
            {
                FillZero();
            }
        }
        public FilledMemoryStream(ushort length, bool random = false) : base(length)
        {
            if (random)
            {
                FillRandom();
            }
            else
            {
                FillZero();
            }
        }

        private void FillZero()
        {
            base.Position = 0;

            for (int i = 0; i < this.Capacity; i++)
            {
                this.WriteByte(0);
            }

            base.Position = 0;
        }

        private void FillRandom()
        {
            Random rnd = new Random();
            for (int i = 0; i < this.Capacity; i++)
            {
                this.WriteByte((byte)rnd.Next(byte.MinValue, byte.MaxValue));
            }
        }
    }
}
