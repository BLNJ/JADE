using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_16_POP
    {
        [Instruction(0xF1, "POP AF")]
        [Instruction(0xC1, "POP BC")]
        [Instruction(0xD1, "POP DE")]
        [Instruction(0xE1, "POP HL")]
        public class xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddStackPop(Bridge.Stack.ParameterValueType.UnsignedShort);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort stackPopValue = (ushort)parametersList[0].Value;

                ParameterRegister register;
                switch(opCode)
                {
                    case 0xF1:
                        register = ParameterRegister.AF;
                        break;
                    case 0xC1:
                        register = ParameterRegister.BC;
                        break;
                    case 0xD1:
                        register = ParameterRegister.DE;
                        break;
                    case 0xE1:
                        register = ParameterRegister.HL;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                changesList.AddRegister(register, stackPopValue);

                return 12;
            }
        }
    }
}
