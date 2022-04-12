using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_CP
    {
        [Instruction(0xBF, "CP A, A")]
        [Instruction(0xB8, "CP A, B")]
        [Instruction(0xB9, "CP A, C")]
        [Instruction(0xBA, "CP A, D")]
        [Instruction(0xBB, "CP A, E")]
        [Instruction(0xBC, "CP A, H")]
        [Instruction(0xBD, "CP A, L")]
        [Instruction(0xBE, "CP A, (HL)")]
        [Instruction(0xFE, "CP A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0xBF:
                        register = ParameterRegister.A;
                        break;
                    case 0xB8:
                        register = ParameterRegister.B;
                        break;
                    case 0xB9:
                        register = ParameterRegister.C;
                        break;
                    case 0xBA:
                        register = ParameterRegister.D;
                        break;
                    case 0xBB:
                        register = ParameterRegister.E;
                        break;
                    case 0xBC:
                        register = ParameterRegister.H;
                        break;
                    case 0xBD:
                        register = ParameterRegister.L;
                        break;
                    case 0xBE:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xFE:
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
                //InstructionMethods.And(registerCommit, registerA, value);
                InstructionMethods.Cp(registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0xBE || opCode == 0xFE)
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
