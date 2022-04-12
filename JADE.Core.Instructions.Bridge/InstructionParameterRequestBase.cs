using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public abstract class InstructionParameterRequestBase : InstructionParameterBase
    {
        public InstructionParameterRequestBase(ParameterRequestSource source) : base(source, ParameterMethod.Get)
        {
        }
    }
}
