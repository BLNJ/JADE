using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.CentralProcessingUnit
{
    public class ProgramCounter : CPUComponent
    {
        internal override ushort Value
        {
            get => base.Registers.PC;
            set => base.Registers.PC = value;
        }

        public ProgramCounter(CPU cpu) : base(cpu)
        {
        }

        public byte ReadByte()
        {
            byte result = base.MMU.Stream.ReadByte(this.Value);
            this.Value++;

            return result;
        }
        public sbyte ReadSByte()
        {
            sbyte result = base.MMU.Stream.ReadSByte(this.Value);
            this.Value++;

            return result;
        }

        public ushort ReadUShort()
        {
            ushort result = base.MMU.Stream.ReadUShort(this.Value);
            this.Value += 2;

            return result;
        }
        public short ReadShort()
        {
            short result = base.MMU.Stream.ReadShort(this.Value);
            this.Value += 2;

            return result;
        }
    }
}
