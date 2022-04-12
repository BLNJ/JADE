using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Register
{
    public class RegisterInstructionParameterRequest : InstructionParameterRequestBase
    {
        public ParameterRegister Register
        {
            get;
            private set;
        }

        public RegisterInstructionParameterRequest(ParameterRegister register) : base(ParameterRequestSource.Register)
        {
            this.Register = register;
        }
    }
}
