using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_16_LD
    {
        [Instruction(0x01, "LD BC, nn")]
        [Instruction(0x11, "LD DE, nn")]
        [Instruction(0x21, "LD HL, nn")]
        [Instruction(0x31, "LD SP, nn")]
        public class xx_nn : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort value = (ushort)parametersList[0].Value;

                ParameterRegister register;

                switch (opCode)
                {
                    case 0x01:
                        register = ParameterRegister.BC;
                        break;
                    case 0x11:
                        register = ParameterRegister.DE;
                        break;
                    case 0x21:
                        register = ParameterRegister.HL;
                        break;
                    case 0x31:
                        register = ParameterRegister.SP; //TODO this will throw an exception in the current design
                        break;

                    default:
                        throw new NotImplementedException();
                }

                changesList.AddRegister(register, value);

                return 12;
            }
        }

        [Instruction(0xF9, "LD SP, HL")]
        public class SP_HL : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.HL);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort value = (ushort)parametersList[0].Value;

                changesList.AddRegister(ParameterRegister.SP, value); //TODO this will throw an exception in the current design

                return 8;
            }
        }

        [Instruction(0xF8, "LD SP, HL + n")]
        public class HL_SP_N : IInstruction
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
                ushort registerSP = (ushort)parametersList[1].Value;

                RegisterCommit registerCommit = new RegisterCommit();
                ushort address = (ushort)(registerSP + value);
                registerCommit.HL = address;

                registerCommit.Flag_Zero = false;
                registerCommit.Flag_Negation = false;

                if (((registerSP ^ value ^ address) & 0x100) == 0x100)
                {
                    registerCommit.Flag_Carry = true;
                }
                else
                {
                    registerCommit.Flag_Carry = false;
                }

                if (((registerSP ^ value ^ address) & 0x10) == 0x10)
                {
                    registerCommit.Flag_HalfCarry = true;
                }
                else
                {
                    registerCommit.Flag_HalfCarry = false;
                }

                changesList.AddRegisterCommit(registerCommit);

                return 12;
            }
        }

        [Instruction(0x08, "LD (nn), SP")]
        public class nn_SP : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);
                parametersList.AddRegister(ParameterRegister.SP);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ushort address = (ushort)parametersList[0].Value;
                ushort registerSP = (ushort)parametersList[1].Value;

                changesList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort, address, registerSP);

                return 20;
            }
        }

        [Instruction(0xEA, "LD (nn), A")]
        [Instruction(0xFA, "LD A, (nn)")]
        public class a_nn : IInstruction
        {
            ushort? address = null;

            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                if (address == null)
                {
                    parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedShort);
                    return false;
                }
                else
                {
                    switch(opCode)
                    {
                        case 0xEA:
                            parametersList.AddRegister(ParameterRegister.A);
                            break;
                        case 0xFA:
                            parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, this.address.Value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    return true;
                }
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                if(address == null)
                {
                    this.address = (ushort)parametersList[0].Value;
                }
                else
                {
                    byte value = (byte)parametersList[1].Value;

                    switch (opCode)
                    {
                        case 0xEA:
                            changesList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, this.address.Value, value);
                            break;
                        case 0xFA:
                            changesList.AddRegister(ParameterRegister.A, value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                return 16;
            }
        }
    }
}
