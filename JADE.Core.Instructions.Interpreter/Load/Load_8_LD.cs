using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_8_LD
    {
        [Instruction(0x06, "LOAD B, n")]
        [Instruction(0x0E, "LOAD C, n")]
        [Instruction(0x16, "LOAD D, n")]
        [Instruction(0x1E, "LOAD E, n")]
        [Instruction(0x26, "LOAD H, n")]
        [Instruction(0x2E, "LOAD L, n")]
        public class x_n : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                ParameterRegister register;
                switch (opCode)
                {
                    case 0x06:
                        register = ParameterRegister.B;
                        break;
                    case 0x0E:
                        register = ParameterRegister.C;
                        break;
                    case 0x16:
                        register = ParameterRegister.D;
                        break;
                    case 0x1E:
                        register = ParameterRegister.E;
                        break;
                    case 0x26:
                        register = ParameterRegister.H;
                        break;
                    case 0x2E:
                        register = ParameterRegister.L;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                changesList.AddRegister(register, value);

                return 8;
            }
        }
    }
}
