using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JADE.Helpers;

namespace JADE.Core.Instructions.Interpreter.Helpers
{
    //     Monke brain not good
    //     Monke brain not find gooder way
    //TODO Future Monke make better
    public static class BitHelper
    {
        private static byte getRegisterBitsValue(byte opCode)
        {
            return (byte)(opCode & 0x7);
        }
        private static byte getBitPositionBitsValue(byte opCode)
        {
            return (byte)((opCode & 0x38) >> 3);
        }

        private static JADE.Core.Instructions.Bridge.Register.ParameterRegister opCodePartToRegister(byte opCodePart)
        {
            switch(opCodePart)
            {
                case 7:
                case 0xF:
                    return Bridge.Register.ParameterRegister.A;
                case 0:
                case 0x8:
                    return Bridge.Register.ParameterRegister.B;
                case 1:
                case 0x9:
                    return Bridge.Register.ParameterRegister.C;
                case 2:
                case 0xA:
                    return Bridge.Register.ParameterRegister.D;
                case 3:
                case 0xB:
                    return Bridge.Register.ParameterRegister.E;
                case 4:
                case 0xC:
                    return Bridge.Register.ParameterRegister.H;
                case 5:
                case 0xD:
                    return Bridge.Register.ParameterRegister.L;
                case 6:
                case 0xE:
                    return Bridge.Register.ParameterRegister.HL;

                default:
                    throw new NotImplementedException("Unknown registerBitsValue: " + opCodePart);
            }
        }

        public static JADE.Core.Instructions.Bridge.Register.ParameterRegister OpCodeUpperNibbleToRegister(byte opCode)
        {
            byte upperNibble = opCode.GetUpper();
            Bridge.Register.ParameterRegister register = opCodePartToRegister(upperNibble);

            return register;
        }

        public static JADE.Core.Instructions.Bridge.Register.ParameterRegister OpCodeLowerNibbleToRegister(byte opCode)
        {
            byte lowerNibble = opCode.GetLower();
            Bridge.Register.ParameterRegister register = opCodePartToRegister(lowerNibble);

            return register;
        }

        public static byte OpCodeUpperNibbleToBitPosition(byte opCode)
        {
            byte bitPosition = getBitPositionBitsValue(opCode);
            return bitPosition;
        }
    }
}
