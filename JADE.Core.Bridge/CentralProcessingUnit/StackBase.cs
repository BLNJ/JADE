using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge.CentralProcessingUnit
{
    public abstract class StackBase : CPUBaseComponent
    {
        public StackBase(CentralProcessingUnit.CPUBase cpu) : base(cpu)
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
            base.mmu.Stream.WriteByte(this.Value, value);
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
            byte value = base.mmu.Stream.ReadByte(this.Value);
            this.Value++;

            return value;
        }
    }
}
