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
    public static class ALU_8_ADC
    {
        [Instruction(0x8F, "ADC A, A")]
        [Instruction(0x88, "ADC A, B")]
        [Instruction(0x89, "ADC A, C")]
        [Instruction(0x8A, "ADC A, D")]
        [Instruction(0x8B, "ADC A, E")]
        [Instruction(0x8C, "ADC A, H")]
        [Instruction(0x8D, "ADC A, L")]

        [Instruction(0x8E, "ADC A, (HL)")]
        [Instruction(0xCE, "ADC A, n")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);
                parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry_int);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x8F:
                        register = ParameterRegister.A;
                        break;
                    case 0x88:
                        register = ParameterRegister.B;
                        break;
                    case 0x89:
                        register = ParameterRegister.C;
                        break;
                    case 0x8A:
                        register = ParameterRegister.D;
                        break;
                    case 0x8B:
                        register = ParameterRegister.E;
                        break;
                    case 0x8C:
                        register = ParameterRegister.H;
                        break;
                    case 0x8D:
                        register = ParameterRegister.L;
                        break;

                    case 0x8E:
                        //parametersList.AddRegister(ParameterRegister.HL);
                        parametersList.AddRelativeMemory(ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xCE:
                        parametersList.AddMemory(ParameterRequestType.UnsignedByte);
                        break;

                    default:
                        throw new Exception();
                }

                if (register.HasValue)
                {
                    parametersList.AddRegister(register.Value);

                    return true;
                }
                else
                {
                    if(opCode == 0xCE)
                    {
                        return true;
                    }
                    else if(opCode == 0x8E)
                    {
                        return true;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;
                int registerFlagCarryInt = (int)parametersList[1].Value;
                byte value = (byte)parametersList[2].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                InstructionMethods.AddCarry(registerCommit, registerA, registerFlagCarryInt, value);

                changesList.AddRegisterCommit(registerCommit);

                if(opCode == 0x8E || opCode == 0xCE)
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
