using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.ArithmeticLogicUnit
{
    public static class ALU_16_ADD
    {
        [Instruction(0x09, "ADD HL, BC")]
        [Instruction(0x19, "ADD HL, DE")]
        [Instruction(0x29, "ADD HL, HL")]
        [Instruction(0x39, "ADD HL, SP")]
        public class HL_xx : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister register;
                switch(opCode)
                {
                    case 0x09:
                        register = ParameterRegister.BC;
                        break;
                    case 0x19:
                        register = ParameterRegister.DE;
                        break;
                    case 0x29:
                        register = ParameterRegister.HL;
                        break;
                    case 0x39:
                        register = ParameterRegister.SP;
                        break;

                    default:
                        throw new Exception();
                }

                //parametersList.Add(new RegisterInstructionParameterRequest(ParameterRegister.A));
                //parametersList.Add(new RegisterInstructionParameterRequest(register));

                parametersList.AddRegister(ParameterRegister.A);
                parametersList.AddRegister(register);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;
                ushort registerCalc = (ushort)parametersList[1].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                InstructionMethods.AddHL(registerCommit, registerA, registerCalc);

                //changesList.Add(new Bridge.Register.RegisterInstructionParameterCommitResponse(registerCommit));
                parametersList.AddRegisterCommit(registerCommit);

                return 8;
            }
        }

        [Instruction(0xE8, "ADD SP, n")]
        public class SP_n : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.SignedByte);
                parametersList.AddRegister(ParameterRegister.SP);
                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                sbyte value = (sbyte)parametersList[0].Value;
                ushort register = (ushort)parametersList[1].Value;

                RegisterCommit registerCommit = new RegisterCommit();

                ushort result = (ushort)(register + value);

                registerCommit.Flag_Zero = false;
                registerCommit.Flag_Negation = false;
                //TODO H and C Flag?

                if (((register ^ value ^ (result & 0xFFFF)) & 0x10) == 0x10)
                {
                    registerCommit.Flag_HalfCarry = true;
                }
                else
                {
                    registerCommit.Flag_HalfCarry = false;
                }
                if (((register ^ value ^ (result & 0xFFFF)) & 0x100) == 0x100)
                {
                    registerCommit.Flag_Carry = true;
                }
                else
                {
                    registerCommit.Flag_Carry = false;
                }

                //cpu.Stack.StackPointer = result;
                changesList.AddRegister(ParameterRegister.SP, result);

                return 16;
            }
        }
    }
}