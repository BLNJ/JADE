using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_8_OR
    {
        [Instruction(0xB7, "ADD A, A")]
        [Instruction(0xB0, "ADD A, B")]
        [Instruction(0xB1, "ADD A, C")]
        [Instruction(0xB2, "ADD A, D")]
        [Instruction(0xB3, "ADD A, E")]
        [Instruction(0xB4, "ADD A, H")]
        [Instruction(0xB5, "ADD A, L")]
        [Instruction(0xB6, "ADD A, (HL)")]
        [Instruction(0xF6, "ADD A, n")]
        public class x_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                ParameterRegister? register = null;
                switch (opCode)
                {
                    case 0xB7:
                        register = ParameterRegister.A;
                        break;
                    case 0xB0:
                        register = ParameterRegister.B;
                        break;
                    case 0xB1:
                        register = ParameterRegister.C;
                        break;
                    case 0xB2:
                        register = ParameterRegister.D;
                        break;
                    case 0xB3:
                        register = ParameterRegister.E;
                        break;
                    case 0xB4:
                        register = ParameterRegister.H;
                        break;
                    case 0xB5:
                        register = ParameterRegister.L;
                        break;
                    case 0xB6:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;
                    case 0xF6:
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
                InstructionMethods.Or(ref registerCommit, registerA, value);

                changesList.AddRegisterCommit(registerCommit);

                if (opCode == 0xB6 || opCode == 0xF6)
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
