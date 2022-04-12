using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Register
{
    public class RegisterFlagInstructionParameterResponse : InstructionParameterResponseBase
    {
        public ParameterFlag Flag
        {
            get;
            private set;
        }

        public RegisterFlagInstructionParameterResponse(ParameterFlag flag, object value) : base(ParameterRequestSource.Flag, value)
        {
            this.Flag = flag;
        }
    }
}
