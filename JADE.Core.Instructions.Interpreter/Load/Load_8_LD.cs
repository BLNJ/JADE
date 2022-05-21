﻿using JADE.Core.Instructions.Bridge;
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
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);

                if(sourceRegister == ParameterRegister.HL)
                {
                    parametersList.AddRelativeMemory(Bridge.Memory.ParameterRequestType.UnsignedByte, ParameterRegister.HL);
                }
                else
                {
                    parametersList.AddMemory(Bridge.Memory.ParameterRequestType.UnsignedByte);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                ParameterRegister destinationRegister = Helpers.BitHelper.OpCodeUpperNibbleToRegister(opCode);
                ParameterRegister sourceRegister = Helpers.BitHelper.OpCodeLowerNibbleToRegister(opCode);
                byte value = (byte)parametersList[0].Value;

                if(destinationRegister == ParameterRegister.HL)
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
    }
}
