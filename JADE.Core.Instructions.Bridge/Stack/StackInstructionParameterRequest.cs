using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Stack
{
    public class StackInstructionParameterRequest : InstructionParameterRequestBase
    {
        public ParameterType Type
        {
            get;
            private set;
        }

        public ParameterValueType ValueType
        {
            get;
            private set;
        }

        public StackInstructionParameterRequest(ParameterType type, ParameterValueType valueType) : base(ParameterRequestSource.Stack)
        {
            this.Type = type;
            this.ValueType = valueType;
        }
    }
}
