using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Misc
{
    public static class Misc_8_SWAP
    {
        [Instruction(0x30, "SWAP B", isExtendedInstruction: true)]
        [Instruction(0x31, "SWAP C", isExtendedInstruction: true)]
        [Instruction(0x32, "SWAP D", isExtendedInstruction: true)]
        [Instruction(0x33, "SWAP E", isExtendedInstruction: true)]
        [Instruction(0x34, "SWAP H", isExtendedInstruction: true)]
        [Instruction(0x35, "SWAP L", isExtendedInstruction: true)]
        [Instruction(0x36, "SWAP (HL)", isExtendedInstruction: true)]
        [Instruction(0x37, "SWAP A", isExtendedInstruction: true)]
        public class SWAP_n : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);

                if (sourceRegister == ParameterRegister.HL)
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                }
                else
                {
                    parametersList.AddRegister(sourceRegister);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                byte value = (byte)parametersList[0].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                byte valueSwap = InstructionMethods.Swap(registerCommit, value);

                changesList.AddRegisterCommit(registerCommit);
                if (sourceRegister == ParameterRegister.HL)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, valueSwap);
                    return 16;
                }
                else
                {
                    changesList.AddRegister(sourceRegister, valueSwap);
                    return 8;
                }
            }
        }
    }
}
