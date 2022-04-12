using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Register
{
    public class RegisterInstructionParameterResponse : InstructionParameterResponseBase
    {
        public ParameterRegister Register
        {
            get;
            private set;
        }

        public RegisterInstructionParameterResponse(ParameterRegister register, object value) : base(ParameterRequestSource.Register, value)
        {
            this.Register = register;
        }
    }
}
