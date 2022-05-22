using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using JADE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter
{
    public static class SetBit_8_SET
    {
        [Instruction(0xC0, "SET 0, B", isExtendedInstruction: true)]
        [Instruction(0xC1, "SET 0, C", isExtendedInstruction: true)]
        [Instruction(0xC2, "SET 0, D", isExtendedInstruction: true)]
        [Instruction(0xC3, "SET 0, E", isExtendedInstruction: true)]
        [Instruction(0xC4, "SET 0, H", isExtendedInstruction: true)]
        [Instruction(0xC5, "SET 0, L", isExtendedInstruction: true)]
        [Instruction(0xC6, "SET 0, (HL)", isExtendedInstruction: true)]
        [Instruction(0xC7, "SET 0, A", isExtendedInstruction: true)]

        [Instruction(0xC8, "SET 1, B", isExtendedInstruction: true)]
        [Instruction(0xC9, "SET 1, C", isExtendedInstruction: true)]
        [Instruction(0xCA, "SET 1, D", isExtendedInstruction: true)]
        [Instruction(0xCB, "SET 1, E", isExtendedInstruction: true)]
        [Instruction(0xCC, "SET 1, H", isExtendedInstruction: true)]
        [Instruction(0xCD, "SET 1, L", isExtendedInstruction: true)]
        [Instruction(0xCE, "SET 1, (HL)", isExtendedInstruction: true)]
        [Instruction(0xCF, "SET 1, A", isExtendedInstruction: true)]


        [Instruction(0xD0, "SET 2, B", isExtendedInstruction: true)]
        [Instruction(0xD1, "SET 2, C", isExtendedInstruction: true)]
        [Instruction(0xD2, "SET 2, D", isExtendedInstruction: true)]
        [Instruction(0xD3, "SET 2, E", isExtendedInstruction: true)]
        [Instruction(0xD4, "SET 2, H", isExtendedInstruction: true)]
        [Instruction(0xD5, "SET 2, L", isExtendedInstruction: true)]
        [Instruction(0xD6, "SET 2, (HL)", isExtendedInstruction: true)]
        [Instruction(0xD7, "SET 2, A", isExtendedInstruction: true)]

        [Instruction(0xD8, "SET 3, B", isExtendedInstruction: true)]
        [Instruction(0xD9, "SET 3, C", isExtendedInstruction: true)]
        [Instruction(0xDA, "SET 3, D", isExtendedInstruction: true)]
        [Instruction(0xDB, "SET 3, E", isExtendedInstruction: true)]
        [Instruction(0xDC, "SET 3, H", isExtendedInstruction: true)]
        [Instruction(0xDD, "SET 3, L", isExtendedInstruction: true)]
        [Instruction(0xDE, "SET 3, (HL)", isExtendedInstruction: true)]
        [Instruction(0xDF, "SET 3, A", isExtendedInstruction: true)]


        [Instruction(0xE0, "SET 4, B", isExtendedInstruction: true)]
        [Instruction(0xE1, "SET 4, C", isExtendedInstruction: true)]
        [Instruction(0xE2, "SET 4, D", isExtendedInstruction: true)]
        [Instruction(0xE3, "SET 4, E", isExtendedInstruction: true)]
        [Instruction(0xE4, "SET 4, H", isExtendedInstruction: true)]
        [Instruction(0xE5, "SET 4, L", isExtendedInstruction: true)]
        [Instruction(0xE6, "SET 4, (HL)", isExtendedInstruction: true)]
        [Instruction(0xE7, "SET 4, A", isExtendedInstruction: true)]

        [Instruction(0xE8, "SET 5, B", isExtendedInstruction: true)]
        [Instruction(0xE9, "SET 5, C", isExtendedInstruction: true)]
        [Instruction(0xEA, "SET 5, D", isExtendedInstruction: true)]
        [Instruction(0xEB, "SET 5, E", isExtendedInstruction: true)]
        [Instruction(0xEC, "SET 5, H", isExtendedInstruction: true)]
        [Instruction(0xED, "SET 5, L", isExtendedInstruction: true)]
        [Instruction(0xEE, "SET 5, (HL)", isExtendedInstruction: true)]
        [Instruction(0xEF, "SET 5, A", isExtendedInstruction: true)]


        [Instruction(0xF0, "SET 6, B", isExtendedInstruction: true)]
        [Instruction(0xF1, "SET 6, C", isExtendedInstruction: true)]
        [Instruction(0xF2, "SET 6, D", isExtendedInstruction: true)]
        [Instruction(0xF3, "SET 6, E", isExtendedInstruction: true)]
        [Instruction(0xF4, "SET 6, H", isExtendedInstruction: true)]
        [Instruction(0xF5, "SET 6, L", isExtendedInstruction: true)]
        [Instruction(0xF6, "SET 6, (HL)", isExtendedInstruction: true)]
        [Instruction(0xF7, "SET 6, A", isExtendedInstruction: true)]

        [Instruction(0xF8, "SET 7, B", isExtendedInstruction: true)]
        [Instruction(0xF9, "SET 7, C", isExtendedInstruction: true)]
        [Instruction(0xFA, "SET 7, D", isExtendedInstruction: true)]
        [Instruction(0xFB, "SET 7, E", isExtendedInstruction: true)]
        [Instruction(0xFC, "SET 7, H", isExtendedInstruction: true)]
        [Instruction(0xFD, "SET 7, L", isExtendedInstruction: true)]
        [Instruction(0xFE, "SET 7, (HL)", isExtendedInstruction: true)]
        [Instruction(0xFF, "SET 7, A", isExtendedInstruction: true)]
        public class x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                if (register == ParameterRegister.HL)
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                }
                else
                {
                    parametersList.AddRegister(register);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ParameterRegister register = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                byte bitPosition = Helpers.BitHelper.OpCodeUpperNibbleToBitPosition(opCode);

                byte value = (byte)parametersList[0].Value;

                InstructionMethods.SetBit(bitPosition, value);

                if (register != ParameterRegister.HL)
                {
                    changesList.AddRegister(register, value);
                    return 8;
                }
                else
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                    return 16;
                }
            }
        }
    }
}
