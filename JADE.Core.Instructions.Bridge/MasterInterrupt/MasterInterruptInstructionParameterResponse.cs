using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.MasterInterrupt
{
    public class MasterInterruptInstructionParameterResponse : InstructionParameterResponseBase
    {
        public MasterInterruptInstructionParameterResponse(bool masterInterruptStatus) : base(ParameterRequestSource.MasterInterrupt, masterInterruptStatus)
        {
        }
    }
}
