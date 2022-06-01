using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Load
{
    public static class Load_8_LD
    {
        [Instruction(0x40, "LOAD B, B")]
        [Instruction(0x41, "LOAD B, C")]
        [Instruction(0x42, "LOAD B, D")]
        [Instruction(0x43, "LOAD B, E")]
        [Instruction(0x44, "LOAD B, H")]
        [Instruction(0x45, "LOAD B, L")]
        [Instruction(0x46, "LOAD B, (HL)")]
        [Instruction(0x47, "LOAD B, A")]

        [Instruction(0x48, "LOAD C, B")]
        [Instruction(0x49, "LOAD C, C")]
        [Instruction(0x4A, "LOAD C, D")]
        [Instruction(0x4B, "LOAD C, E")]
        [Instruction(0x4C, "LOAD C, H")]
        [Instruction(0x4D, "LOAD C, L")]
        [Instruction(0x4E, "LOAD C, (HL)")]
        [Instruction(0x4F, "LOAD C, A")]


        [Instruction(0x50, "LOAD D, B")]
        [Instruction(0x51, "LOAD D, C")]
        [Instruction(0x52, "LOAD D, D")]
        [Instruction(0x53, "LOAD D, E")]
        [Instruction(0x54, "LOAD D, H")]
        [Instruction(0x55, "LOAD D, L")]
        [Instruction(0x56, "LOAD D, (HL)")]
        [Instruction(0x57, "LOAD D, A")]

        [Instruction(0x58, "LOAD E, B")]
        [Instruction(0x59, "LOAD E, C")]
        [Instruction(0x5A, "LOAD E, D")]
        [Instruction(0x5B, "LOAD E, E")]
        [Instruction(0x5C, "LOAD E, H")]
        [Instruction(0x5D, "LOAD E, L")]
        [Instruction(0x5E, "LOAD E, (HL)")]
        [Instruction(0x5F, "LOAD E, A")]


        [Instruction(0x60, "LOAD H, B")]
        [Instruction(0x61, "LOAD H, C")]
        [Instruction(0x62, "LOAD H, D")]
        [Instruction(0x63, "LOAD H, E")]
        [Instruction(0x64, "LOAD H, H")]
        [Instruction(0x65, "LOAD H, L")]
        [Instruction(0x66, "LOAD H, (HL)")]
        [Instruction(0x67, "LOAD H, A")]

        [Instruction(0x68, "LOAD L, B")]
        [Instruction(0x69, "LOAD L, C")]
        [Instruction(0x6A, "LOAD L, D")]
        [Instruction(0x6B, "LOAD L, E")]
        [Instruction(0x6C, "LOAD L, H")]
        [Instruction(0x6D, "LOAD L, L")]
        [Instruction(0x6E, "LOAD L, (HL)")]
        [Instruction(0x6F, "LOAD L, A")]


        [Instruction(0x70, "LOAD (HL), B")]
        [Instruction(0x71, "LOAD (HL), C")]
        [Instruction(0x72, "LOAD (HL), D")]
        [Instruction(0x73, "LOAD (HL), E")]
        [Instruction(0x74, "LOAD (HL), H")]
        [Instruction(0x75, "LOAD (HL), L")]
        //[Instruction(0x76, "LOAD (HL), (HL)")] "HALT" instead of "LOAD (HL), (HL)"
        [Instruction(0x77, "LOAD (HL), A")]

        [Instruction(0x78, "LOAD A, B")]
        [Instruction(0x79, "LOAD A, C")]
        [Instruction(0x7A, "LOAD A, D")]
        [Instruction(0x7B, "LOAD A, E")]
        [Instruction(0x7C, "LOAD A, H")]
        [Instruction(0x7D, "LOAD A, L")]
        [Instruction(0x7E, "LOAD A, (HL)")]
        [Instruction(0x7F, "LOAD A, A")]
        public class x_n : IInstruction
        {
            private static ParameterRegister opCodeDestinationRegister(byte opCode)
            {
                if(opCode >= 0x40 && opCode <= 0x47)
                {
                    return ParameterRegister.B;
                }
                else if(opCode >= 0x48 && opCode <= 0x4F)
                {
                    return ParameterRegister.C;
                }

                else if (opCode >= 0x50 && opCode <= 0x57)
                {
                    return ParameterRegister.D;
                }
                else if (opCode >= 0x58 && opCode <= 0x5F)
                {
                    return ParameterRegister.E;
                }

                else if (opCode >= 0x60 && opCode <= 0x67)
                {
                    return ParameterRegister.H;
                }
                else if (opCode >= 0x68 && opCode <= 0x6F)
                {
                    return ParameterRegister.L;
                }

                else if (opCode >= 0x70 && opCode <= 0x77)
                {
                    return ParameterRegister.HL;
                }
                else if (opCode >= 0x78 && opCode <= 0x7F)
                {
                    return ParameterRegister.A;
                }

                else
                {
                    throw new NotImplementedException();
                }
            }

            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);

                if(sourceRegister == ParameterRegister.HL)
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
                ParameterRegister destinationRegister = opCodeDestinationRegister(opCode);
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                byte value = (byte)parametersList[0].Value;

                //TODO I doubt theres a cleaner way, since thats a unique edge-case, but future me will find a way... surely
                if (opCode >= 0x70 && opCode <= 0x77)
                {
                    destinationRegister = ParameterRegister.HL;
                }

                if (destinationRegister == ParameterRegister.HL)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                    return 8;
                }
                else
                {
                    changesList.AddRegister(destinationRegister, value);

                    if (sourceRegister == ParameterRegister.HL)
                    {
                        return 8;
                    }
                    else
                    {
                        return 4;
                    }
                }
            }
        }

        [Instruction(0x02, "LD (BC), A")]
        [Instruction(0x12, "LD (DE), A")]
        public class rr_A : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);
                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;

                ParameterRegister destinationRegister;
                if(opCode == 0x02)
                {
                    destinationRegister = ParameterRegister.BC;
                }
                else
                {
                    destinationRegister = ParameterRegister.DE;
                }

                changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, destinationRegister, registerA);
                return 8;
            }
        }

        [Instruction(0x06, "LD B, n")]
        [Instruction(0x16, "LD D, n")]
        [Instruction(0x26, "LD H, n")]
        [Instruction(0x36, "LD (HL), n")]
        public class r_n : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                if(opCode == 0x36)
                {
                    changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL, value);
                    return 12;
                }
                else
                {
                    ParameterRegister destinationRegister;

                    switch(opCode)
                    {
                        case 0x06:
                            destinationRegister = ParameterRegister.B;
                            break;
                        case 0x16:
                            destinationRegister = ParameterRegister.D;
                            break;
                        case 0x26:
                            destinationRegister = ParameterRegister.H;
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    changesList.AddRegister(destinationRegister, value);

                    return 8;
                }
            }
        }

        [Instruction(0x0A, "LD A, (BC)")]
        [Instruction(0x1A, "LD A, (DE)")]
        public class A_rr : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                if(opCode == 0x0A)
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.BC);
                }
                else
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.DE);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                changesList.AddRegister(ParameterRegister.A, value);

                return 8;
            }
        }

        [Instruction(0x0E, "LD C, n")]
        [Instruction(0x1E, "LD E, n")]
        [Instruction(0x2E, "LD L, n")]
        [Instruction(0x3E, "LD A, n")]
        public class R_n : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                ParameterRegister destinationRegister;
                switch (opCode)
                {
                    case 0x0E:
                        destinationRegister = ParameterRegister.C;
                        break;
                    case 0x1E:
                        destinationRegister = ParameterRegister.E;
                        break;
                    case 0x2E:
                        destinationRegister = ParameterRegister.L;
                        break;
                    case 0x3E:
                        destinationRegister = ParameterRegister.A;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                changesList.AddRegister(destinationRegister, value);

                return 8;
            }
        }

        [Instruction(0xE2, "LD (FF00 + C), A")]
        [Instruction(0xF2, "LD A, (FF00 + C)")]
        public class rn_a : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                switch(opCode)
                {
                    case 0xE2:
                        parametersList.AddRegister(ParameterRegister.A);
                        break;
                    case 0xF2:
                        parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.C, 0xFF00);
                        break;
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte value = (byte)parametersList[0].Value;

                switch (opCode)
                {
                    case 0xE2:
                        changesList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.C, 0xFF00, value);
                        break;
                    case 0xF2:
                        changesList.AddRegister(ParameterRegister.A, value);
                        break;
                }

                return 8;
            }
        }
    }
}
