using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Rotate
{
    public static class RotateLeft_8_RL
    {
        [Instruction(0x10, "RL B", isExtendedInstruction: true)]
        [Instruction(0x11, "RL C", isExtendedInstruction: true)]
        [Instruction(0x12, "RL D", isExtendedInstruction: true)]
        [Instruction(0x13, "RL E", isExtendedInstruction: true)]
        [Instruction(0x14, "RL H", isExtendedInstruction: true)]
        [Instruction(0x15, "RL L", isExtendedInstruction: true)]
        [Instruction(0x16, "RL (HL)", isExtendedInstruction: true)]
        [Instruction(0x17, "RL A", isExtendedInstruction: true)]
        public class RL_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister registerSource = Helpers.BitHelper.OpCodeUpperNibbleToRegister(opCode);

                if(registerSource == ParameterRegister.HL)
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
                ParameterRegister registerSource = Helpers.BitHelper.OpCodeUpperNibbleToRegister(opCode);
                byte value = (byte)parametersList[0].Value;
                bool flagCarry = (bool)parametersList[1].Value;

                RegisterCommit commit = new RegisterCommit();
                byte rotatedValue = InstructionMethods.RotateLeft(commit, flagCarry, value);

                changesList.AddRegisterCommit(commit);

                if(registerSource == ParameterRegister.HL)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, rotatedValue);
                    return 16;
                }
                else
                {
                    changesList.AddRegister(registerSource, rotatedValue);
                    return 8;
                }
            }
        }
    }
}
