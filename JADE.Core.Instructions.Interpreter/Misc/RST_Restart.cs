using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Misc
{
    public static class RST_Restart
    {
        [Instruction(0xC7, "RST 0x0")]
        [Instruction(0xCF, "RST 0x8")]
        [Instruction(0xD7, "RST 0x10")]
        [Instruction(0xDF, "RST 0x18")]
        [Instruction(0xE7, "RST 0x20")]
        [Instruction(0xEF, "RST 0x28")]
        [Instruction(0xF7, "RST 0x30")]
        [Instruction(0xFF, "RST 0x38")]
        public class nn_relative : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte pcLocation = opCodeToLocation(opCode);
                InstructionMethods.Call(pcLocation);

                return 32;
            }

            private byte opCodeToLocation(byte opCode)
            {
                switch (opCode)
                {
                    case 0xC7:
                        return 0x0;
                    case 0xCF:
                        return 0x8;
                    case 0xD7:
                        return 0x10;
                    case 0xDF:
                        return 0x18;
                    case 0xE7:
                        return 0x20;
                    case 0xEF:
                        return 0x28;
                    case 0xF7:
                        return 0x30;
                    case 0xFF:
                        return 0x38;


                    default:
                        throw new NotImplementedException("Unknown opCode to jump to: " + opCode.ToString("X2"));
                }
            }
        }
    }
}
