using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Register
{
    public class RegisterInstructionParameterCommitResponse : InstructionParameterResponseBase
    {
        public new RegisterCommit Value
        {
            get
            {
                return (RegisterCommit)base.Value;
            }
        }

        public RegisterInstructionParameterCommitResponse(RegisterCommit commit) : base(ParameterRequestSource.RegisterCommit, commit)
        {
        }
    }
}
