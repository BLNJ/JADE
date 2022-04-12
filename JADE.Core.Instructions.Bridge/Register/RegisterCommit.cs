using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public struct RegisterCommit
    {
        public byte? A;
        public byte? F;
        public ushort? AF;

        public byte? B;
        public byte? C;
        public ushort? BC;

        public byte? D;
        public byte? E;
        public ushort? DE;

        public byte? H;
        public byte? L;
        public ushort? HL;

        public bool? Flag_Zero;
        public bool? Flag_Negation;
        public bool? Flag_HalfCarry;
        public bool? Flag_Carry;
        public int? Flag_Carry_int;
    }
}
