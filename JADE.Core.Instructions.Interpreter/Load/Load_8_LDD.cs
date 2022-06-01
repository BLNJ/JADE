using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_8_LDD
    {
        [Instruction(0x32, "LDD (HL), A")]
        [Instruction(0x3A, "LDD A, (HL)")]
        public class nn_nn : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                switch(opCode)
                {
                    case 0x32:
                        parametersList.AddRegister(ParameterRegister.A);
                        break;
                    case 0x3A:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                parametersList.AddRegister(ParameterRegister.HL);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;
                ushort registerHL = (ushort)parametersList[1].Value;

                switch (opCode)
                {
                    case 0x32:
                        changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                        break;
                    case 0x3A:
                        changesList.AddRegister(ParameterRegister.A, value);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                registerHL -= 1;
                changesList.AddRegister(ParameterRegister.HL, registerHL);

                return 8;
            }
        }
    }
}
