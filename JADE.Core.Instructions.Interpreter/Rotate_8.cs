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
    public static class Rotate_8
    {
        [Instruction(0x00, "RLC B", isExtendedInstruction: true)]
        [Instruction(0x01, "RLC C", isExtendedInstruction: true)]
        [Instruction(0x02, "RLC D", isExtendedInstruction: true)]
        [Instruction(0x03, "RLC E", isExtendedInstruction: true)]
        [Instruction(0x04, "RLC H", isExtendedInstruction: true)]
        [Instruction(0x05, "RLC L", isExtendedInstruction: true)]
        [Instruction(0x06, "RLC (HL)", isExtendedInstruction: true)]
        [Instruction(0x07, "RLC A", isExtendedInstruction: true)]

        [Instruction(0x08, "RRC B", isExtendedInstruction: true)]
        [Instruction(0x09, "RRC C", isExtendedInstruction: true)]
        [Instruction(0x0A, "RRC D", isExtendedInstruction: true)]
        [Instruction(0x0B, "RRC E", isExtendedInstruction: true)]
        [Instruction(0x0C, "RRC H", isExtendedInstruction: true)]
        [Instruction(0x0D, "RRC L", isExtendedInstruction: true)]
        [Instruction(0x0E, "RRC (HL)", isExtendedInstruction: true)]
        [Instruction(0x0F, "RRC A", isExtendedInstruction: true)]


        [Instruction(0x10, "RL B", isExtendedInstruction: true)]
        [Instruction(0x11, "RL C", isExtendedInstruction: true)]
        [Instruction(0x12, "RL D", isExtendedInstruction: true)]
        [Instruction(0x13, "RL E", isExtendedInstruction: true)]
        [Instruction(0x14, "RL H", isExtendedInstruction: true)]
        [Instruction(0x15, "RL L", isExtendedInstruction: true)]
        [Instruction(0x16, "RL (HL)", isExtendedInstruction: true)]
        [Instruction(0x17, "RL A", isExtendedInstruction: true)]

        [Instruction(0x18, "RR B", isExtendedInstruction: true)]
        [Instruction(0x19, "RR C", isExtendedInstruction: true)]
        [Instruction(0x1A, "RR D", isExtendedInstruction: true)]
        [Instruction(0x1B, "RR E", isExtendedInstruction: true)]
        [Instruction(0x1C, "RR H", isExtendedInstruction: true)]
        [Instruction(0x1D, "RR L", isExtendedInstruction: true)]
        [Instruction(0x1E, "RR (HL)", isExtendedInstruction: true)]
        [Instruction(0x1F, "RR A", isExtendedInstruction: true)]
        public class Rotate : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister registerSource = Helpers.BitHelper.OpCodeUpperNibbleToRegister(opCode);

                if (registerSource == ParameterRegister.HL)
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                }
                else
                {
                    parametersList.AddRegister(registerSource);
                }

                parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte operationCode = Helpers.BitHelper.OpCodeUpperNibbleToBitPosition(opCode); //TODO Function needs to be renamed
                byte operationLowerNibble = opCode.GetLower();
                ParameterRegister registerSource = Helpers.BitHelper.OpCodeUpperNibbleToRegister(opCode);
                byte value = (byte)parametersList[0].Value;
                bool flagCarry = (bool)parametersList[1].Value;

                RegisterCommit commit = new RegisterCommit();
                byte? rotatedValue = null;

                switch (operationCode)
                {
                    case 0:
                        if (operationLowerNibble > 0x7)
                        {
                            rotatedValue = InstructionMethods.RotateRightCarry(commit, value);
                        }
                        else
                        {
                            rotatedValue = InstructionMethods.RotateLeftCarry(commit, value);
                        }
                        break;
                    case 1:
                        if (operationLowerNibble > 0x7)
                        {
                            rotatedValue = InstructionMethods.RotateRight(commit, flagCarry, value);
                        }
                        else
                        {
                            rotatedValue = InstructionMethods.RotateLeft(commit, flagCarry, value);
                        }
                        break;
                }

                if (rotatedValue == null)
                {
                    throw new Exception("rotatedValue is null");
                }

                changesList.AddRegisterCommit(commit);

                if (registerSource == ParameterRegister.HL)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, rotatedValue.Value);
                    return 16;
                }
                else
                {
                    changesList.AddRegister(registerSource, rotatedValue.Value);
                    return 8;
                }
            }
        }
    }
}
