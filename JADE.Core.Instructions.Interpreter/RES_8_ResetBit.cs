using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter
{
    public static class RES_8_ResetBit
    {
        [Instruction(0x80, "BIT 0, B", isExtendedInstruction: true)]
        [Instruction(0x81, "BIT 0, C", isExtendedInstruction: true)]
        [Instruction(0x82, "BIT 0, D", isExtendedInstruction: true)]
        [Instruction(0x83, "BIT 0, E", isExtendedInstruction: true)]
        [Instruction(0x84, "BIT 0, H", isExtendedInstruction: true)]
        [Instruction(0x85, "BIT 0, L", isExtendedInstruction: true)]
        [Instruction(0x86, "BIT 0, (HL)", isExtendedInstruction: true)]
        [Instruction(0x87, "BIT 0, A", isExtendedInstruction: true)]

        [Instruction(0x88, "BIT 1, B", isExtendedInstruction: true)]
        [Instruction(0x89, "BIT 1, C", isExtendedInstruction: true)]
        [Instruction(0x8A, "BIT 1, D", isExtendedInstruction: true)]
        [Instruction(0x8B, "BIT 1, E", isExtendedInstruction: true)]
        [Instruction(0x8C, "BIT 1, H", isExtendedInstruction: true)]
        [Instruction(0x8D, "BIT 1, L", isExtendedInstruction: true)]
        [Instruction(0x8E, "BIT 1, (HL)", isExtendedInstruction: true)]
        [Instruction(0x8F, "BIT 1, A", isExtendedInstruction: true)]


        [Instruction(0x90, "BIT 2, B", isExtendedInstruction: true)]
        [Instruction(0x91, "BIT 2, C", isExtendedInstruction: true)]
        [Instruction(0x92, "BIT 2, D", isExtendedInstruction: true)]
        [Instruction(0x93, "BIT 2, E", isExtendedInstruction: true)]
        [Instruction(0x94, "BIT 2, H", isExtendedInstruction: true)]
        [Instruction(0x95, "BIT 2, L", isExtendedInstruction: true)]
        [Instruction(0x96, "BIT 2, (HL)", isExtendedInstruction: true)]
        [Instruction(0x97, "BIT 2, A", isExtendedInstruction: true)]

        [Instruction(0x98, "BIT 3, B", isExtendedInstruction: true)]
        [Instruction(0x99, "BIT 3, C", isExtendedInstruction: true)]
        [Instruction(0x9A, "BIT 3, D", isExtendedInstruction: true)]
        [Instruction(0x9B, "BIT 3, E", isExtendedInstruction: true)]
        [Instruction(0x9C, "BIT 3, H", isExtendedInstruction: true)]
        [Instruction(0x9D, "BIT 3, L", isExtendedInstruction: true)]
        [Instruction(0x9E, "BIT 3, (HL)", isExtendedInstruction: true)]
        [Instruction(0x9F, "BIT 3, A", isExtendedInstruction: true)]


        [Instruction(0xA0, "BIT 4, B", isExtendedInstruction: true)]
        [Instruction(0xA1, "BIT 4, C", isExtendedInstruction: true)]
        [Instruction(0xA2, "BIT 4, D", isExtendedInstruction: true)]
        [Instruction(0xA3, "BIT 4, E", isExtendedInstruction: true)]
        [Instruction(0xA4, "BIT 4, H", isExtendedInstruction: true)]
        [Instruction(0xA5, "BIT 4, L", isExtendedInstruction: true)]
        [Instruction(0xA6, "BIT 4, (HL)", isExtendedInstruction: true)]
        [Instruction(0xA7, "BIT 4, A", isExtendedInstruction: true)]

        [Instruction(0xA8, "BIT 5, B", isExtendedInstruction: true)]
        [Instruction(0xA9, "BIT 5, C", isExtendedInstruction: true)]
        [Instruction(0xAA, "BIT 5, D", isExtendedInstruction: true)]
        [Instruction(0xAB, "BIT 5, E", isExtendedInstruction: true)]
        [Instruction(0xAC, "BIT 5, H", isExtendedInstruction: true)]
        [Instruction(0xAD, "BIT 5, L", isExtendedInstruction: true)]
        [Instruction(0xAE, "BIT 5, (HL)", isExtendedInstruction: true)]
        [Instruction(0xAF, "BIT 5, A", isExtendedInstruction: true)]


        [Instruction(0xB0, "BIT 6, B", isExtendedInstruction: true)]
        [Instruction(0xB1, "BIT 6, C", isExtendedInstruction: true)]
        [Instruction(0xB2, "BIT 6, D", isExtendedInstruction: true)]
        [Instruction(0xB3, "BIT 6, E", isExtendedInstruction: true)]
        [Instruction(0xB4, "BIT 6, H", isExtendedInstruction: true)]
        [Instruction(0xB5, "BIT 6, L", isExtendedInstruction: true)]
        [Instruction(0xB6, "BIT 6, (HL)", isExtendedInstruction: true)]
        [Instruction(0xB7, "BIT 6, A", isExtendedInstruction: true)]

        [Instruction(0xB8, "BIT 7, B", isExtendedInstruction: true)]
        [Instruction(0xB9, "BIT 7, C", isExtendedInstruction: true)]
        [Instruction(0xBA, "BIT 7, D", isExtendedInstruction: true)]
        [Instruction(0xBB, "BIT 7, E", isExtendedInstruction: true)]
        [Instruction(0xBC, "BIT 7, H", isExtendedInstruction: true)]
        [Instruction(0xBD, "BIT 7, L", isExtendedInstruction: true)]
        [Instruction(0xBE, "BIT 7, (HL)", isExtendedInstruction: true)]
        [Instruction(0xBF, "BIT 7, A", isExtendedInstruction: true)]
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
                byte valueReset = InstructionMethods.ResetBit(bitPosition, value);

                if (register != ParameterRegister.HL)
                {
                    changesList.AddRegister(register, valueReset);
                    return 8;
                }
                else
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, valueReset);
                    return 16;
                }
            }
        }
    }
}
