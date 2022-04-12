using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge.CentralProcessingUnit
{
    public abstract class CPUBaseComponent<T> where T : struct, IComparable, IConvertible
    {
        private CPUBase cpu;

        internal MemoryManagementUnit.MMUBase mmu
        {
            get
            {
                return this.cpu.MMU;
            }
        }

        public abstract T Value
        {
            get;
            internal set;
        }

        public CPUBaseComponent(CPUBase cpu)
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
