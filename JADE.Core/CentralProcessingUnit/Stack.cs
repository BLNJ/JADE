using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JADE.Core.Bridge;

namespace JADE.Core.CentralProcessingUnit
{
    public class Stack : JADE.Core.Bridge.CentralProcessingUnit.StackBase
    {
        internal override ushort Value
        {
            get => base.Registers.SP;
            set => base.Registers.SP = value;
        }

        public Stack(CPU cpu) : base(cpu)
        {
        }
    }
}
