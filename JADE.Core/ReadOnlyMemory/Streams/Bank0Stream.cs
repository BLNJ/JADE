using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.ReadOnlyMemory.Streams
{
    public class Bank0Stream : JADE.IO.ExternalMemory
    {
        public Bank0Stream(Stream baseStream) : base(baseStream, true)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //This is a Bank0 Stream, writing should be done
            //But if something has that stupid idea, we will just ignore it
            //Just like our Sponsor, Freshdesk
        }
    }
}
