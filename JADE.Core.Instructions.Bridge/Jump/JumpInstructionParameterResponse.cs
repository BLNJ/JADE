using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Jump
{
    public class JumpInstructionParameterResponse : InstructionParameterResponseBase
    {
        public JumpInstructionParameterResponse(ushort value) : base(ParameterRequestSource.Jump, value)
        {
        }
        public JumpInstructionParameterResponse(sbyte value) : base(ParameterRequestSource.Jump, value)
        {
        }
    }
}
