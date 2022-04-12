using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Helpers
{
    public static class Extensions
    {
        public static byte SetBit(this byte b, int bit, bool on)
        {
            return (byte)SetBit((int)b, bit, on);
        }

        public static int SetBit(this int b, int bit, bool on)
        {
            if (on)
                return (b | (1 << bit));
            else
                return (b & ~(1 << bit));
        }

        public static bool GetBit(this byte b, int bit)
        {
            return GetBit((int)b, bit);
        }

        public static bool GetBit(this int b, int bit)
        {
            int mask = 1 << bit;
            return (b & mask) == mask;
        }
        public static byte GetUpper(this byte value)
        {
            return (byte)((value & 0xF0) >> 4);
        }
        public static byte GetLower(this byte value)
        {
            return (byte)(value & 0x0F);
        }
        public static byte SetUpper(this byte value, byte upper)
        {
            return (byte)((value & 0x0F) + (upper << 4));
        }
        public static byte SetLower(this byte value, byte lower)
        {
            return (byte)((value & 0xF0) + (lower & 0x0F));
        }

        public static byte GetUpper(this ushort value)
        {
            return (byte)(value >> 8);
        }
        public static byte GetLower(this ushort value)
        {
            return (byte)(value & 0xFF);
        }
        public static ushort SetUpper(this ushort value, byte upper)
        {
            return (ushort)((value & 0x00FF) + (upper << 8));
        }
        public static ushort SetLower(this ushort value, byte lower)
        {
            return (ushort)((value & 0xFF00) + (lower & 0x00FF));
        }
        public static ushort CombineUpperLower(this ushort value, byte upper, byte lower)
        {
            ushort newValue = value.SetUpper(upper);
            newValue = newValue.SetLower(lower);
            return newValue;
        }
    }
}
