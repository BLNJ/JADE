using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using JADE.Core.Instructions.Bridge.Memory;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_16_INC
    {
        [Instruction(0x03, "INC BC")]
        [Instruction(0x13, "INC DE")]
        [Instruction(0x23, "INC HL")]
        [Instruction(0x33, "INC SP")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register;
                switch (opCode)
                {
                    case 0x03:
                        register = ParameterRegister.BC;
                        break;
                    case 0x13:
                        register = ParameterRegister.DE;
                        break;
                    case 0x23:
                        register = ParameterRegister.HL;
                        break;
                    case 0x33:
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
                value = (ushort)(value + 1);

                ParameterRegister register = response.Register;
                changesList.AddRegister(register, value);

                return 8;
            }
        }
    }
}
