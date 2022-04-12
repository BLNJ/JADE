using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.CentralProcessingUnit
{
    public abstract class CPUComponent
    {
        private CPU cpu;

        internal Registers.CPURegisters Registers
        {
            get
            {
                return this.cpu.Registers;
            }
        }
        internal MemoryManagementUnit.MMU MMU
        {
            get
            {
                return cpu.MMU;
            }
        }

        internal abstract ushort Value
        {
            get;
            set;
        }

        public CPUComponent(CPU cpu)
        {
            if (cpu == null)
            {
                throw new ArgumentNullException("cpu");
            }
            else
            {
                this.cpu = cpu;
            }
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
