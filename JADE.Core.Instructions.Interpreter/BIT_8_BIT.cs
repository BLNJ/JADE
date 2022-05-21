using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter
{
    public static class BIT_8_BIT
    {
        //I know this is really ugly, but its the smartest way I came up with at 5 in the morning
        [Instruction(0x47, "BIT 0, A", isExtendedInstruction: true)]
        [Instruction(0x40, "BIT 0, B", isExtendedInstruction: true)]
        [Instruction(0x41, "BIT 0, C", isExtendedInstruction: true)]
        [Instruction(0x42, "BIT 0, D", isExtendedInstruction: true)]
        [Instruction(0x43, "BIT 0, E", isExtendedInstruction: true)]
        [Instruction(0x44, "BIT 0, H", isExtendedInstruction: true)]
        [Instruction(0x45, "BIT 0, L", isExtendedInstruction: true)]
        [Instruction(0x46, "BIT 0, (HL)", isExtendedInstruction: true)]

        [Instruction(0x4F, "BIT 1, A", isExtendedInstruction: true)]
        [Instruction(0x48, "BIT 1, B", isExtendedInstruction: true)]
        [Instruction(0x49, "BIT 1, C", isExtendedInstruction: true)]
        [Instruction(0x4A, "BIT 1, D", isExtendedInstruction: true)]
        [Instruction(0x4B, "BIT 1, E", isExtendedInstruction: true)]
        [Instruction(0x4C, "BIT 1, H", isExtendedInstruction: true)]
        [Instruction(0x4D, "BIT 1, L", isExtendedInstruction: true)]
        [Instruction(0x4E, "BIT 1, (HL)", isExtendedInstruction: true)]

        [Instruction(0x57, "BIT 2, A", isExtendedInstruction: true)]
        [Instruction(0x50, "BIT 2, B", isExtendedInstruction: true)]
        [Instruction(0x51, "BIT 2, C", isExtendedInstruction: true)]
        [Instruction(0x52, "BIT 2, D", isExtendedInstruction: true)]
        [Instruction(0x53, "BIT 2, E", isExtendedInstruction: true)]
        [Instruction(0x54, "BIT 2, H", isExtendedInstruction: true)]
        [Instruction(0x55, "BIT 2, L", isExtendedInstruction: true)]
        [Instruction(0x56, "BIT 2, (HL)", isExtendedInstruction: true)]

        [Instruction(0x5F, "BIT 3, A", isExtendedInstruction: true)]
        [Instruction(0x58, "BIT 3, B", isExtendedInstruction: true)]
        [Instruction(0x59, "BIT 3, C", isExtendedInstruction: true)]
        [Instruction(0x5A, "BIT 3, D", isExtendedInstruction: true)]
        [Instruction(0x5B, "BIT 3, E", isExtendedInstruction: true)]
        [Instruction(0x5C, "BIT 3, H", isExtendedInstruction: true)]
        [Instruction(0x5D, "BIT 3, L", isExtendedInstruction: true)]
        [Instruction(0x5E, "BIT 3, (HL)", isExtendedInstruction: true)]

        [Instruction(0x67, "BIT 4, A", isExtendedInstruction: true)]
        [Instruction(0x60, "BIT 4, B", isExtendedInstruction: true)]
        [Instruction(0x61, "BIT 4, C", isExtendedInstruction: true)]
        [Instruction(0x62, "BIT 4, D", isExtendedInstruction: true)]
        [Instruction(0x63, "BIT 4, E", isExtendedInstruction: true)]
        [Instruction(0x64, "BIT 4, H", isExtendedInstruction: true)]
        [Instruction(0x65, "BIT 4, L", isExtendedInstruction: true)]
        [Instruction(0x66, "BIT 4, (HL)", isExtendedInstruction: true)]

        [Instruction(0x6F, "BIT 5, A", isExtendedInstruction: true)]
        [Instruction(0x68, "BIT 5, B", isExtendedInstruction: true)]
        [Instruction(0x69, "BIT 5, C", isExtendedInstruction: true)]
        [Instruction(0x6A, "BIT 5, D", isExtendedInstruction: true)]
        [Instruction(0x6B, "BIT 5, E", isExtendedInstruction: true)]
        [Instruction(0x6C, "BIT 5, H", isExtendedInstruction: true)]
        [Instruction(0x6D, "BIT 5, L", isExtendedInstruction: true)]
        [Instruction(0x6E, "BIT 5, (HL)", isExtendedInstruction: true)]

        [Instruction(0x77, "BIT 6, A", isExtendedInstruction: true)]
        [Instruction(0x70, "BIT 6, B", isExtendedInstruction: true)]
        [Instruction(0x71, "BIT 6, C", isExtendedInstruction: true)]
        [Instruction(0x72, "BIT 6, D", isExtendedInstruction: true)]
        [Instruction(0x73, "BIT 6, E", isExtendedInstruction: true)]
        [Instruction(0x74, "BIT 6, H", isExtendedInstruction: true)]
        [Instruction(0x75, "BIT 6, L", isExtendedInstruction: true)]
        [Instruction(0x76, "BIT 6, (HL)", isExtendedInstruction: true)]

        [Instruction(0x7F, "BIT 7, A", isExtendedInstruction: true)]
        [Instruction(0x78, "BIT 7, B", isExtendedInstruction: true)]
        [Instruction(0x79, "BIT 7, C", isExtendedInstruction: true)]
        [Instruction(0x7A, "BIT 7, D", isExtendedInstruction: true)]
        [Instruction(0x7B, "BIT 7, E", isExtendedInstruction: true)]
        [Instruction(0x7C, "BIT 7, H", isExtendedInstruction: true)]
        [Instruction(0x7D, "BIT 7, L", isExtendedInstruction: true)]
        [Instruction(0x7E, "BIT 7, (HL)", isExtendedInstruction: true)]
        public class x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                if(register == ParameterRegister.HL)
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

                RegisterCommit registerCommit = new RegisterCommit();
                InstructionMethods.Bit(registerCommit, bitPosition, value);

                if(register != ParameterRegister.HL)
                {
                    return 8;
                }
                else
                {
                    return 16;
                }
            }
        }
    }
}
