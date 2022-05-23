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
            byte? value = null;

            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);

                if(opCode == 0xE0)
                {
                    parametersList.AddRegister(ParameterRegister.A);
                    return true;
                }
                else
                {
                    if(value == null)
                    {
                        return false;
                    }
                    else
                    {
                        parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, (0xFF00 + value.Value));
                        return true;
                    }
                }
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                if (opCode == 0xE0)
                {
                    byte registerA = (byte)parametersList[1].Value;
                    changesList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, (0xFF00 + value), registerA);
                }
                else
                {
                    if(this.value == null)
                    {
                        this.value = value;
                    }
                    else
                    {
                        byte memoryValue = (byte)parametersList[1].Value;
                        changesList.AddRegister(ParameterRegister.A, memoryValue);
                    }
                }

                return 12;
            }
        }
    }
}
