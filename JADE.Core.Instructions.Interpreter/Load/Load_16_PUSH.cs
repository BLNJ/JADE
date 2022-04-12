using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_16_PUSH
    {
        [Instruction(0xF5, "PUSH AF")]
        [Instruction(0xC5, "PUSH BC")]
        [Instruction(0xD5, "PUSH DE")]
        [Instruction(0xE5, "PUSH HL")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register;
                switch (opCode)
                {
                    case 0xF5:
                        register = ParameterRegister.AF;
                        break;
                    case 0xC5:
                        register = ParameterRegister.BC;
                        break;
                    case 0xD5:
                        register = ParameterRegister.DE;
                        break;
                    case 0xE5:
                        register = ParameterRegister.HL;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                parametersList.AddRegister(register);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort registerValue = (ushort)parametersList[0].Value;
                changesList.AddStackPush(registerValue);

                return 12;
            }
        }
    }
}
