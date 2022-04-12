using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JADE.Core.InputOutput
{
    public class Timer
    {
        CentralProcessingUnit.CPU cpu;
        MemoryManagementUnit.MMU mmu
        {
            get
            {
                return cpu.MMU;
            }
        }

        public byte Divider
        {
            get
            {
                return ReadData(0xFF04);
            }
            set
            {
                mmu.Stream.WriteByte(0xFF04, 0);
                throw new NotImplementedException();
            }
        }
        public byte Counter
        {
            get
            {
                return ReadData(0xFF05);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public byte Modulo
        {
            get
            {
                return ReadData(0xFF06);
            }
            set
            {
                WriteData(0xFF06, value);
            }
        }
        public byte Control
        {
            get
            {
                return ReadData(0xFF07);
            }
            set
            {
                WriteData(0xFF07, value);
            }
        }
        public Speed Frequency
        {
            get
            {
                bool bit0 = this.Control.GetBit(0);
                bool bit1 = this.Control.GetBit(1);

                byte lower = 0;
                lower.SetBit(0, bit0);
                lower.SetBit(1, bit1);

                return (Speed)lower;
            }
            set
            {
                this.Control.SetBit(0, ((byte)value).GetBit(0));
                this.Control.SetBit(1, ((byte)value).GetBit(1));
            }
        }
        public bool Running
        {
            get
            {
                return this.Control.GetBit(2);
            }
            set
            {
                this.Control.SetBit(2, value);
            }
        }

        internal void Tick(byte cycles)
        {
            throw new NotImplementedException();
        }

        private byte ReadData(long offset)
        {
            return this.mmu.Stream.ReadByte(offset);
        }
        private void WriteData(long offset, byte value)
        {
            this.mmu.Stream.WriteByte(offset, value);
        }

        public enum Speed : byte
        {
            Hz4096 = 0,
            Hz262144 = 1,
            Hz65536 = 2,
            Hz16384 = 3
        }
    }
}
