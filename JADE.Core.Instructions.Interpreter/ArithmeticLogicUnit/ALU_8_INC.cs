using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_INC
    {
        [Instruction(0x3C, "INC A")]
        [Instruction(0x04, "INC B")]
        [Instruction(0x0C, "INC C")]
        [Instruction(0x14, "INC D")]
        [Instruction(0x1C, "INC E")]
        [Instruction(0x24, "INC H")]
        [Instruction(0x2C, "INC L")]
        [Instruction(0x34, "INC (HL)")]
        //[Instruction(0xC6, "ADD A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                //parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x3C:
                        register = ParameterRegister.A;
                        break;
                    case 0x04:
                        register = ParameterRegister.B;
                        break;
                    case 0x0C:
                        register = ParameterRegister.C;
                        break;
                    case 0x14:
                        register = ParameterRegister.D;
                        break;
                    case 0x1C:
                        register = ParameterRegister.E;
                        break;
                    case 0x24:
                        register = ParameterRegister.H;
                        break;
                    case 0x2C:
                        register = ParameterRegister.L;
                        break;
                    case 0x34:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                        //case 0xC6:
                        //    parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                        //    break;
                }

                if (register.HasValue)
                {
                    parametersList.AddRegister(register.Value);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                //byte registerA = (byte)parametersList[0].Value;
                byte value = (byte)parametersList[0].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                //InstructionMethods.AddA(registerCommit, registerA, value);
                InstructionMethods.Increment(ref registerCommit, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0x34)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                    return 12;
                }
                else
                {
                    RegisterInstructionParameterResponse registerResponse = (RegisterInstructionParameterResponse)parametersList[0];

                    changesList.AddRegister(registerResponse.Register, value);
                    return 4;
                }
            }
        }
    }
}
