using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public abstract class InstructionParameterResponseBase : InstructionParameterBase
    {
        public object Value
        {
            get;
            private set;
        }

        public InstructionParameterResponseBase(ParameterRequestSource source, object value) : base(source, ParameterMethod.Set)
        {
            this.Value = value;
        }
    }
}
