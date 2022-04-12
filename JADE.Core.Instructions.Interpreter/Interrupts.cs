using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter
{
    [Instruction(0xF3, "DI")]
    public class DI : IInstruction
    {
        public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
        {
            throw new NotImplementedException();
            return true;
        }

        public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
        {
            throw new NotImplementedException();
            return 4;
        }
    }

    [Instruction(0xFB, "EI")]
    public class EI : IInstruction
    {
        public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
        {
            throw new NotImplementedException();
            return true;
        }

        public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
        {
            throw new NotImplementedException();
            return 4;
        }
    }
}
