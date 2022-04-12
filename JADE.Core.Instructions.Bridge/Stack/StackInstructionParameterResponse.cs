using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Stack
{
    public class StackInstructionParameterResponse : InstructionParameterResponseBase
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

        public StackInstructionParameterResponse(ParameterType type, ParameterValueType valueType, object value) : base(ParameterRequestSource.Stack, value)
        {
            this.Type = type;
            this.ValueType = valueType;
        }
    }
}
