using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_8_LDH
    {
        [Instruction(0xE0, "LDH (FF00 + n), A")]
        [Instruction(0xF0, "LDH A, (FF00 + n)")]
        public class nn_relative : IInstruction
        {
            byte? lowAddress = null;

            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                if (lowAddress == null)
                {
                    parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                    return false;
                }
                else
                {
                    if (opCode == 0xE0)
                    {
                        parametersList.AddRegister(ParameterRegister.A);
                        return true;
                    }
                    else
                    {
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, (0xFF00 + lowAddress.Value));
                        return true;

                    }
                }
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                if (lowAddress == null)
                {
                    lowAddress = (byte)parametersList[0].Value;
                }
                else
                {
                    if (opCode == 0xE0)
                    {
                        byte registerA = (byte)parametersList[0].Value;
                        changesList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, (0xFF00 + lowAddress.Value), registerA);
                    }
                    else
                    {
                        byte memoryValue = (byte)parametersList[0].Value;
                        changesList.AddRegister(ParameterRegister.A, memoryValue);
                    }
                }

                return 12;
            }
        }
    }
}
