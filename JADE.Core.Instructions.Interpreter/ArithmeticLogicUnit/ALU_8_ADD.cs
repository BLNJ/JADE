using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_ADD
    {
        [Instruction(0x87, "ADD A, A")]
        [Instruction(0x80, "ADD A, B")]
        [Instruction(0x81, "ADD A, C")]
        [Instruction(0x82, "ADD A, D")]
        [Instruction(0x83, "ADD A, E")]
        [Instruction(0x84, "ADD A, H")]
        [Instruction(0x85, "ADD A, L")]
        [Instruction(0x86, "ADD A, (HL)")]
        [Instruction(0xC6, "ADD A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0x87:
                        register = ParameterRegister.A;
                        break;
                    case 0x80:
                        register = ParameterRegister.B;
                        break;
                    case 0x81:
                        register = ParameterRegister.C;
                        break;
                    case 0x82:
                        register = ParameterRegister.D;
                        break;
                    case 0x83:
                        register = ParameterRegister.E;
                        break;
                    case 0x84:
                        register = ParameterRegister.H;
                        break;
                    case 0x85:
                        register = ParameterRegister.L;
                        break;
                    case 0x86:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xC6:
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                        break;
                }

                if(register.HasValue)
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
                InstructionMethods.AddA(registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if(opCode == 0x86 || opCode == 0xC6)
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
