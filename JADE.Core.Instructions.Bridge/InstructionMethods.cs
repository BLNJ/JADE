using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JADE.Helpers;

namespace JADE.Core.Instructions.Bridge
{
    public static class InstructionMethods
    {
        private static void ZeroCheck(ref RegisterCommit registerCommit, int value)
        {
            if (value == 0)
            {
                registerCommit.Flag_Zero = true;
            }
            else
            {
                registerCommit.Flag_Zero = false;
            }
        }

        #region Arithmetic Operations
        public static void AddA(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            var result = (registerA + value);

            ZeroCheck(ref registerCommit, registerA);

            registerCommit.Flag_Negation = false;
            if (((result & 0xF) + (value & 0xF)) > 0xF)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            if ((result & 0x1000) != 0)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }

            registerCommit.A = (byte)result;
        }

        public static void AddHL(ref RegisterCommit registerCommit, ushort registerHL, ushort value)
        {
            ushort register = registerHL;
            var result = (register + value);

            registerCommit.Flag_Negation = false;
            if (((register & 0xFFF) + (value & 0xFFF)) > 0xFFF)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            if ((result & 0x10000) != 0)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }

            registerCommit.HL = (ushort)result;
        }

        public static void AddCarry(ref RegisterCommit registerCommit, byte registerA, int registerFlagCarryInt, byte value)
        {
            byte register = registerA;
            int carry = registerFlagCarryInt;

            var result_full = (register + value + carry);
            byte result = (byte)result_full;

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = false;
            if (((register & 0xF) + (value & 0xF) + carry) > 0xf)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            if (result_full > 0xFF)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }

            registerCommit.A = result;
        }

        public static void Subtract(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            byte register = registerA;
            byte result = (byte)(register - value);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = true;
            if (((register & 0xF) - (value & 0xF)) < 0)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            if (register < value)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }

            registerCommit.A = result;
        }

        public static void SubtractCarry(ref RegisterCommit registerCommit, byte registerA, int registerClagCarryInt, byte value)
        {
            var carry = registerClagCarryInt;
            byte register = registerA;

            var result_full = (register - value - carry);
            byte result = (byte)result_full;

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = true;
            if (result_full < 0)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }

            if (((register & 0xF) - (value & 0xF) - carry) < 0)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            registerCommit.A = result;
        }

        public static byte Increment(ref RegisterCommit registerCommit, byte value)
        {
            var result = (byte)(value + 1);

            ZeroCheck(ref registerCommit, result);

            registerCommit.Flag_Negation = false;

            if ((result & 0x0F) == 0x0F)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            return result;
        }

        public static byte Decrement(ref RegisterCommit registerCommit, byte value)
        {
            byte result = (byte)(value - 1);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = true;

            if ((result & 0x0F) == 0x0F)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }

            return result;
        }
        #endregion
        #region Logical Operations
        public static void And(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            byte register = registerA;
            byte result = (byte)(register & value);

            registerCommit.A = result;

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_HalfCarry = true;
            registerCommit.Flag_Carry = false;
            registerCommit.Flag_Negation = false;
        }

        public static void Or(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            byte register = registerA;
            byte result = (byte)(register | value);

            registerCommit.A = result;

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Carry = false;
            registerCommit.Flag_Negation = false;
        }

        public static void Xor(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            byte register = registerA;
            byte result = (byte)(register ^ value);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = false;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Carry = false;

            registerCommit.A = result;
        }

        public static void Cp(ref RegisterCommit registerCommit, byte registerA, byte value)
        {
            byte register = registerA;
            byte result = (byte)(register - value);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = true;

            if (((register & 0xF) - (value & 0xF)) < 0)
            {
                registerCommit.Flag_HalfCarry = true;
            }
            else
            {
                registerCommit.Flag_HalfCarry = false;
            }
            if (register < value)
            {
                registerCommit.Flag_Carry = true;
            }
            else
            {
                registerCommit.Flag_Carry = false;
            }
        }
        #endregion
        #region Rotate Operations
        public static byte RotateLeft(ref RegisterCommit registerCommit, bool registerFlagCarry, byte register)
        {
            bool carry = registerFlagCarry;
            bool willCarry = register.GetBit(7);
            registerCommit.Flag_Carry = willCarry;

            byte result = (byte)(register << 1);
            if (carry)
            {
                result = (byte)(result | 1);
            }
            else
            {
                result = (byte)(result | 0);
            }

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = false;
            registerCommit.Flag_HalfCarry = false;

            return result;
        }
        public static byte RotateLeftCarry(ref RegisterCommit registerCommit, byte register)
        {
            bool carry = register.GetBit(7);
            byte result = (byte)(register << 1);

            if (carry)
            {
                result = (byte)(result | 1);
            }
            else
            {
                result = (byte)(result | 0);
            }

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Carry = carry;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }

        public static byte RotateRight(ref RegisterCommit registerCommit, bool registerFlagCarry, byte register)
        {
            bool carry = registerFlagCarry;
            bool willCarry = register.GetBit(0);

            registerCommit.Flag_Carry = willCarry;

            byte result = (byte)(register >> 1);
            if (carry)
            {
                result = (byte)(result | (1 << 7));
            }
            else
            {
                result = (byte)(result | (0 << 7));
            }

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }

        public static byte RotateRightCarry(ref RegisterCommit registerCommit, byte register)
        {
            bool carry = register.GetBit(0);
            byte result = (byte)(register << 1);

            if (carry)
            {
                result = (byte)(result | (1 >> 7));
            }
            else
            {
                result = (byte)(result | (0 >> 7));
            }

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Carry = carry;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }

        public static byte ShiftRightLogical(ref RegisterCommit registerCommit, byte register)
        {
            bool lowestNibble = register.GetBit(0);
            byte result = (byte)(register >> 1);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Carry = lowestNibble;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }

        public static byte ShiftRightArithmetic(ref RegisterCommit registerCommit, byte register)
        {
            bool lowestNibble = register.GetBit(0);
            bool highestNibble = register.GetBit(7);

            byte result = (byte)(register >> 1);
            result = result.SetBit(7, highestNibble);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Carry = lowestNibble;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }

        public static byte ShiftLeftArithmetic(ref RegisterCommit registerCommit, byte register)
        {
            bool carryBit = register.GetBit(7);
            byte result = (byte)(register << 1);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Carry = carryBit;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Negation = false;

            return result;
        }
        #endregion

        #region Bit Operations
        public static void Bit(ref RegisterCommit registerCommit, int bitpos, byte register)
        {
            bool bit = register.GetBit(bitpos);

            if (bit)
            {
                registerCommit.Flag_Zero = false;
            }
            else
            {
                registerCommit.Flag_Zero = true;
            }

            registerCommit.Flag_HalfCarry = true;
            registerCommit.Flag_Negation = false;
        }
        public static byte ResetBit(int bitpos, byte register)
        {
            byte value = register.SetBit(bitpos, false);
            return value;
        }
        public static byte SetBit(int bitpos, byte register)
        {
            byte value = register.SetBit(bitpos, true);
            return value;
        }
        #endregion

        #region Call / Return
        public static void Call(ushort address)
        {
            //registers.SP--;
            //byte upper = registers.PC.GetUpper();
            //cpu.MMU.Stream.WriteByte(registers.SP, upper);

            //registers.SP--;
            //byte lower = registers.PC.GetLower();
            //cpu.MMU.Stream.WriteByte(registers.SP, lower);


            throw new NotImplementedException();
            //cpu.Stack.PushUShort(registers.PC);

            //registers.PC = address;
        }

        public static void Ret()
        {
            //TODO this could be fucking wrong, its a short
            //byte lower = cpu.MMU.Stream.ReadByte(registers.SP);
            //registers.SP++;

            //byte upper = cpu.MMU.Stream.ReadByte(registers.SP);
            //registers.SP++;

            //ushort address = 0;
            //address = address.SetUpper(upper);
            //address = address.SetLower(lower);



            throw new NotImplementedException();
            //ushort address = cpu.Stack.PopUShort();

            //registers.PC = address;
        }
        #endregion

        public static byte Swap(ref RegisterCommit registerCommit, byte value)
        {
            //byte result = (byte)(((value & 0x0F) << 4) | ((value & 0xF0) >> 4));

            //Flag_Z_Check(result);
            //return result;

            byte lowNibbles = value.GetLower();
            byte highNibbles = value.GetUpper();

            byte result = 0;
            result = result.SetUpper(lowNibbles);
            result = result.SetLower(highNibbles);

            ZeroCheck(ref registerCommit, result);
            registerCommit.Flag_Negation = false;
            registerCommit.Flag_HalfCarry = false;
            registerCommit.Flag_Carry = false;

            return result;
        }
    }
}
