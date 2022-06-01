using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Jump
{
    public static class JumpRegister_8_JR
    {
        [Instruction(0x18, "JR n")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.SignedByte);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                sbyte jpValue = (sbyte)parametersList[0].Value;
                changesList.AddJump(jpValue);

                return 8;
            }
        }

        [Instruction(0x20, "JP NZ, nn")]
        [Instruction(0x28, "JP Z, nn")]
        [Instruction(0x30, "JP NC, nn")]
        [Instruction(0x38, "JP C, nn")]
        public class Flag_xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.SignedByte);

                ParameterFlag flag;
                switch (opCode)
                {
                    case 0x20: // NotZero
                    case 0x28: // Zero
                        flag = ParameterFlag.Flag_Zero;
                        break;

                    case 0x30: // NotCarry
                    case 0x38: // Carry
                        flag = ParameterFlag.Flag_Carry;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                parametersList.AddRegisterFlag(flag);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                sbyte jpValue = (sbyte)parametersList[0].Value;
                bool flag = (bool)parametersList[1].Value;

                bool process = false;

                switch (opCode)
                {
                    case 0x20: // NotZero
                    case 0x30: // NotCarry
                        if (!flag)
                        {
                            process = true;
                        }
                        break;

                    case 0x28: // Zero
                    case 0x38: // Carry
                        if (flag)
                        {
                            process = true;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (process)
                {
                    changesList.AddJump(jpValue);
                }

                return 8;
            }
        }
    }
}
