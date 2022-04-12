using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Register
{
    public class RegisterFlagInstructionParameterRequest : InstructionParameterRequestBase
    {
        public ParameterFlag Flag
        {
            get;
            private set;
        }

        public RegisterFlagInstructionParameterRequest(ParameterFlag flag) : base(ParameterRequestSource.Flag)
        {
            this.Flag = flag;
        }
    }
}
