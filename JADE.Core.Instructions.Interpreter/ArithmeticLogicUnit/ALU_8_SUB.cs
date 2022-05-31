using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_SUB
    {
        [Instruction(0x97, "SBC A, A")]
        [Instruction(0x90, "SBC A, B")]
        [Instruction(0x91, "SBC A, C")]
        [Instruction(0x92, "SBC A, D")]
        [Instruction(0x93, "SBC A, E")]
        [Instruction(0x94, "SBC A, H")]
        [Instruction(0x95, "SBC A, L")]
        [Instruction(0x96, "SBC A, (HL)")]
        [Instruction(0xD6, "SBC A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x97:
                        register = ParameterRegister.A;
                        break;
                    case 0x90:
                        register = ParameterRegister.B;
                        break;
                    case 0x91:
                        register = ParameterRegister.C;
                        break;
                    case 0x92:
                        register = ParameterRegister.D;
                        break;
                    case 0x93:
                        register = ParameterRegister.E;
                        break;
                    case 0x94:
                        register = ParameterRegister.H;
                        break;
                    case 0x95:
                        register = ParameterRegister.L;
                        break;
                    case 0x96:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xD6:
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
                InstructionMethods.Subtract(ref registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0x96 || opCode == 0xD6)
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
