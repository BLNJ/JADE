using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_XOR
    {
        [Instruction(0xAF, "SBC A, A")]
        [Instruction(0xA8, "SBC A, B")]
        [Instruction(0xA9, "SBC A, C")]
        [Instruction(0xAA, "SBC A, D")]
        [Instruction(0xAB, "SBC A, E")]
        [Instruction(0xAC, "SBC A, H")]
        [Instruction(0xAD, "SBC A, L")]
        [Instruction(0xAE, "SBC A, (HL)")]
        [Instruction(0xEE, "SBC A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0xAF:
                        register = ParameterRegister.A;
                        break;
                    case 0xA8:
                        register = ParameterRegister.B;
                        break;
                    case 0xA9:
                        register = ParameterRegister.C;
                        break;
                    case 0xAA:
                        register = ParameterRegister.D;
                        break;
                    case 0xAB:
                        register = ParameterRegister.E;
                        break;
                    case 0xAC:
                        register = ParameterRegister.H;
                        break;
                    case 0xAD:
                        register = ParameterRegister.L;
                        break;

                    case 0xAE:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xEE:
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                        break;
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
                InstructionMethods.Xor(registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0xAE || opCode == 0xEE)
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
