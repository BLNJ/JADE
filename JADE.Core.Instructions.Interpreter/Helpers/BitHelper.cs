using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static JADE.Core.Instructions.Bridge.Register.ParameterRegister OpcodeToRegister(byte opCode)
        {
            byte registerBitsValue = getRegisterBitsValue(opCode);

            switch(registerBitsValue)
            {
                case 7:
                    return Bridge.Register.ParameterRegister.A;
                case 0:
                    return Bridge.Register.ParameterRegister.B;
                case 1:
                    return Bridge.Register.ParameterRegister.C;
                case 2:
                    return Bridge.Register.ParameterRegister.D;
                case 3:
                    return Bridge.Register.ParameterRegister.E;
                case 4:
                    return Bridge.Register.ParameterRegister.H;
                case 5:
                    return Bridge.Register.ParameterRegister.L;
                case 6:
                    return Bridge.Register.ParameterRegister.HL;

                default:
                    throw new NotImplementedException("Unknown registerBitsValue: " + registerBitsValue);
            }
        }

        public static byte OpCodeToBitPosition(byte opCode)
        {
            byte bitPosition = getBitPositionBitsValue(opCode);
            return bitPosition;
        }
    }
}
