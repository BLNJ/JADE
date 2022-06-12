using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using JADE.Core.Instructions.Bridge.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_16_DEC
    {
        [Instruction(0x0B, "DEC BC")]
        [Instruction(0x1B, "DEC DE")]
        [Instruction(0x2B, "DEC HL")]
        [Instruction(0x3B, "DEC SP")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register;
                switch (opCode)
                {
                    case 0x0B:
                        register = ParameterRegister.BC;
                        break;
                    case 0x1B:
                        register = ParameterRegister.DE;
                        break;
                    case 0x2B:
                        register = ParameterRegister.HL;
                        break;
                    case 0x3B:
                        register = ParameterRegister.SP;
                        break;

                    default:
                        throw new Exception();
                }

                parametersList.AddRegister(register);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                RegisterInstructionParameterResponse response = (RegisterInstructionParameterResponse)parametersList[0];

                ushort value = (ushort)response.Value;
                value = (ushort)(value - 1);

                ParameterRegister register = response.Register;
                changesList.AddRegister(register, value);

                return 8;
            }
        }
    }
}
