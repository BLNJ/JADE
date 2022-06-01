using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_AND
    {
        [Instruction(0xA7, "ADD A, A")]
        [Instruction(0xA0, "ADD A, B")]
        [Instruction(0xA1, "ADD A, C")]
        [Instruction(0xA2, "ADD A, D")]
        [Instruction(0xA3, "ADD A, E")]
        [Instruction(0xA4, "ADD A, H")]
        [Instruction(0xA5, "ADD A, L")]
        [Instruction(0xA6, "ADD A, (HL)")]
        [Instruction(0xE6, "ADD A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0xA7:
                        register = ParameterRegister.A;
                        break;
                    case 0xA0:
                        register = ParameterRegister.B;
                        break;
                    case 0xA1:
                        register = ParameterRegister.C;
                        break;
                    case 0xA2:
                        register = ParameterRegister.D;
                        break;
                    case 0xA3:
                        register = ParameterRegister.E;
                        break;
                    case 0xA4:
                        register = ParameterRegister.H;
                        break;
                    case 0xA5:
                        register = ParameterRegister.L;
                        break;

                    case 0xA6:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xE6:
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (register.HasValue)
                {
                    parametersList.AddRegister(register.Value);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;
                byte value = (byte)parametersList[1].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                InstructionMethods.And(ref registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0xA6 || opCode == 0xE6)
                {
                    return 8;
                }
                else
                {
                    return 4;
                }
            }
        }
    }
}
