using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Misc
{
    [Instruction(0x00, "NOP")]
    public class NOP : IInstruction
    {
        public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
        {
            return true;
        }

        public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
        {
            return 4;
        }
    }
}
