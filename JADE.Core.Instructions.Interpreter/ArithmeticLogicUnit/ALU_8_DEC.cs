using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_DEC
    {
        [Instruction(0x3D, "DEC A")]
        [Instruction(0x05, "DEC B")]
        [Instruction(0x0D, "DEC C")]
        [Instruction(0x15, "DEC D")]
        [Instruction(0x1D, "DEC E")]
        [Instruction(0x25, "DEC H")]
        [Instruction(0x2D, "DEC L")]
        [Instruction(0x35, "DEC (HL)")]
        //[Instruction(0xC6, "ADD A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                //parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x3D:
                        register = ParameterRegister.A;
                        break;
                    case 0x05:
                        register = ParameterRegister.B;
                        break;
                    case 0x0D:
                        register = ParameterRegister.C;
                        break;
                    case 0x15:
                        register = ParameterRegister.D;
                        break;
                    case 0x1D:
                        register = ParameterRegister.E;
                        break;
                    case 0x25:
                        register = ParameterRegister.H;
                        break;
                    case 0x2D:
                        register = ParameterRegister.L;
                        break;
                    case 0x35:
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
                byte newValue = InstructionMethods.Decrement(ref registerCommit, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0x35)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, newValue);
                    return 12;
                }
                else
                {
                    RegisterInstructionParameterResponse registerResponse = (RegisterInstructionParameterResponse)parametersList[0];

                    changesList.AddRegister(registerResponse.Register, newValue);
                    return 4;
                }
            }
        }
    }
}
