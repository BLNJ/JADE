using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Jump
{
    public class CallInstructionParameterResponse : InstructionParameterResponseBase
    {
        public CallInstructionParameterResponse(ushort value) : base(ParameterRequestSource.Call, value)
        {
        }
    }
}
