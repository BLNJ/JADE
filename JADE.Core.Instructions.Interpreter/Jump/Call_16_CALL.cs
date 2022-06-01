using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Jump
{
    public static class Call_16_CALL
    {
        [Instruction(0xCD, "CALL nn")]
        [Instruction(0xC4, "CALL NZ, nn")]
        [Instruction(0xCC, "CALL Z, nn")]
        [Instruction(0xD4, "CALL NC, nn")]
        [Instruction(0xDC, "CALL C, nn")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);

                if (opCode != 0xCD)
                {
                    switch (opCode)
                    {
                        case 0xC4: //NotZero
                        case 0xCC: //Zero
                            parametersList.AddRegisterFlag(ParameterFlag.Flag_Zero);
                            break;
                        case 0xD4: //NotCarry
                        case 0xDC: //Carry
                            parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort value = (ushort)parametersList[0].Value;
                bool doCall = true;

                if (opCode != 0xCD)
                {
                    bool flag = (bool)parametersList[1].Value;

                    switch (opCode)
                    {
                        case 0xC4: //NotZero
                        case 0xD4: //NotCarry
                            if (!flag)
                            {
                                doCall = true;
                            }
                            break;
                        case 0xCC: //Zero
                        case 0xDC: //Carry
                            if (flag)
                            {
                                doCall = true;
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                if (doCall)
                {
                    InstructionMethods.Call(value);
                }

                return 12;
            }
        }
    }
}
