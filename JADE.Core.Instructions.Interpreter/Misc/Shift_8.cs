using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using JADE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Misc
{
    public static class Shift_8
    {
        [Instruction(0x20, "SLA B", isExtendedInstruction: true)]
        [Instruction(0x21, "SLA C", isExtendedInstruction: true)]
        [Instruction(0x22, "SLA D", isExtendedInstruction: true)]
        [Instruction(0x23, "SLA E", isExtendedInstruction: true)]
        [Instruction(0x24, "SLA H", isExtendedInstruction: true)]
        [Instruction(0x25, "SLA L", isExtendedInstruction: true)]
        [Instruction(0x26, "SLA (HL)", isExtendedInstruction: true)]
        [Instruction(0x27, "SLA A", isExtendedInstruction: true)]

        [Instruction(0x28, "SRA B", isExtendedInstruction: true)]
        [Instruction(0x29, "SRA C", isExtendedInstruction: true)]
        [Instruction(0x2A, "SRA D", isExtendedInstruction: true)]
        [Instruction(0x2B, "SRA E", isExtendedInstruction: true)]
        [Instruction(0x2C, "SRA H", isExtendedInstruction: true)]
        [Instruction(0x2D, "SRA L", isExtendedInstruction: true)]
        [Instruction(0x2E, "SRA (HL)", isExtendedInstruction: true)]
        [Instruction(0x2F, "SRA A", isExtendedInstruction: true)]


        [Instruction(0x38, "SRL B", isExtendedInstruction: true)]
        [Instruction(0x39, "SRL C", isExtendedInstruction: true)]
        [Instruction(0x3A, "SRL D", isExtendedInstruction: true)]
        [Instruction(0x3B, "SRL E", isExtendedInstruction: true)]
        [Instruction(0x3C, "SRL H", isExtendedInstruction: true)]
        [Instruction(0x3D, "SRL L", isExtendedInstruction: true)]
        [Instruction(0x3E, "SRL (HL)", isExtendedInstruction: true)]
        [Instruction(0x3F, "SRL A", isExtendedInstruction: true)]
        public class ShiftArithmetic : IInstruction
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
                byte operationCode = opCode.GetLower();

                byte value = (byte)parametersList[0].Value;
                byte shiftedValue;
                RegisterCommit commit = new RegisterCommit();

                if (opCode >= 0x20 && opCode <= 0x2F)
                {
                    if (operationCode <= 0x7)
                    {
                        shiftedValue = InstructionMethods.ShiftLeftArithmetic(ref commit, value);
                    }
                    else
                    {
                        shiftedValue = InstructionMethods.ShiftRightArithmetic(ref commit, value);
                    }
                }
                else
                {
                    shiftedValue = InstructionMethods.ShiftRightLogical(ref commit, value);
                }

                changesList.AddRegisterCommit(commit);

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
