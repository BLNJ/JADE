using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.CentralProcessingUnit
{
    public class Stack : CPUComponent
    {
        internal override ushort Value
        {
            get => base.Registers.SP;
            set => base.Registers.SP = value;
        }

        public Stack(CPU cpu) : base(cpu)
        {
        }

        public void PushUShort(ushort value)
        {
            byte upper = value.GetUpper();
            byte lower = value.GetLower();

            PushByte((byte)upper);
            PushByte((byte)lower);
        }

        public void PushByte(byte value)
        {
            this.Value--;
            base.MMU.Stream.WriteByte(this.Value, value, jumpBack: true);
        }

        public ushort PopUShort()
        {
            byte lower = PopByte();
            byte upper = PopByte();

            ushort value = 0;
            value = value.SetLower(lower);
            value = value.SetUpper(upper);

            return value;
        }

        public byte PopByte()
        {
            byte value = base.MMU.Stream.ReadByte(this.Value, jumpBack: true);
            this.Value++;

            return value;
        }
    }
}
