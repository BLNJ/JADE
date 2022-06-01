using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_SBC
    {
        [Instruction(0x9F, "SBC A, A")]
        [Instruction(0x98, "SBC A, B")]
        [Instruction(0x99, "SBC A, C")]
        [Instruction(0x9A, "SBC A, D")]
        [Instruction(0x9B, "SBC A, E")]
        [Instruction(0x9C, "SBC A, H")]
        [Instruction(0x9D, "SBC A, L")]
        [Instruction(0x9E, "SBC A, (HL)")]
        [Instruction(0xDE, "SBC A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);
                parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry_int);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x9F:
                        register = ParameterRegister.A;
                        break;
                    case 0x98:
                        register = ParameterRegister.B;
                        break;
                    case 0x99:
                        register = ParameterRegister.C;
                        break;
                    case 0x9A:
                        register = ParameterRegister.D;
                        break;
                    case 0x9B:
                        register = ParameterRegister.E;
                        break;
                    case 0x9C:
                        register = ParameterRegister.H;
                        break;
                    case 0x9D:
                        register = ParameterRegister.L;
                        break;
                    case 0x9E:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xDE:
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
                int registerFlagCarryInt = (int)parametersList[1].Value;
                byte value = (byte)parametersList[2].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                InstructionMethods.SubtractCarry(ref registerCommit, registerA, registerFlagCarryInt, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0x9E)
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
