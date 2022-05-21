using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public class Load_8_LDI
    {
        [Instruction(0x2A, "LDI A, (HL)")]
        [Instruction(0x22, "LDI (HL), A")]
        public class nn_nn : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                if(opCode == 0x22)
                {
                    parametersList.AddRegister(ParameterRegister.A);
                }
                else
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                }

                parametersList.AddRegister(ParameterRegister.HL);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;
                ushort registerHL = (ushort)parametersList[1].Value;

                if (opCode == 0x22)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                }
                else
                {
                    changesList.AddRegister(ParameterRegister.A, value);
                }

                registerHL += 1;
                changesList.AddRegister(ParameterRegister.HL, registerHL);

                return 8;
            }
        }
    }
}
