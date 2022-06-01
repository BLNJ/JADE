using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Jump
{
    public static class Jump_16_JP
    {
        [Instruction(0xC3, "JP nn")]
        [Instruction(0xE9, "JP (HL)")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                switch(opCode)
                {
                    case 0xC3:
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);
                        break;
                    case 0xE9:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedShort, ParameterRegister.HL);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort jpValue = (ushort)parametersList[0].Value;
                changesList.AddJump(jpValue);

                return 12;
            }
        }

        [Instruction(0xC2, "JP NZ, nn")]
        [Instruction(0xCA, "JP Z, nn")]
        [Instruction(0xD2, "JP NC, nn")]
        [Instruction(0xDA, "JP C, nn")]
        public class Flag_xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);

                ParameterFlag flag;
                switch(opCode)
                {
                    case 0xC2: // NotZero
                    case 0xCA: // Zero
                        flag = ParameterFlag.Flag_Zero;
                        break;

                    case 0xD2: // NotCarry
                    case 0xDA: // Carry
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
                ushort jpValue = (ushort)parametersList[0].Value;
                bool flag = (bool)parametersList[1].Value;

                bool process = false;

                switch(opCode)
                {
                    case 0xC2: // NotZero
                    case 0xD2: // NotCarry
                        if(!flag)
                        {
                            process = true;
                        }
                        break;

                    case 0xCA: // Zero
                    case 0xDA: // Carry
                        if(flag)
                        {
                            process = true;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if(process)
                {
                    changesList.AddJump(jpValue);
                }

                return 12;
            }
        }
    }
}
