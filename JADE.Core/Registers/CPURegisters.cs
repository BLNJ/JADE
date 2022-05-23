using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Registers
{
    public class CPURegisters : INotifyPropertyChanged
    {
        ushort pc;
        /// <summary>
        /// Program Counter
        /// </summary>
        public ushort PC
        {
            get
            {
                return this.pc;
            }
            set
            {
                this.pc = value;
                OnPropertyChanged();
            }
        }

        ushort sp;
        /// <summary>
        /// Stack Pointer
        /// Points to top of stack (grows from top to bottom)
        /// </summary>
        public ushort SP
        {
            get
            {
                return this.sp;
            }
            set
            {
                this.sp = value;
                OnPropertyChanged();
            }
        }

        #region 16-Bit Registers
        public ushort AF
        {
            get
            {
                ushort value = 0;
                value = value.CombineUpperLower(this.a, this.f);
                return value;
            }
            internal set
            {
                this.a = value.GetUpper();
                this.f = value.GetLower();
                OnPropertyChanged();
            }
        }
        public ushort BC
        {
            get
            {
                ushort value = 0;
                value = value.CombineUpperLower(this.b, this.c);
                return value;
            }
            internal set
            {
                this.b = value.GetUpper();
                this.c = value.GetLower();
                OnPropertyChanged();
            }
        }
        public ushort DE
        {
            get
            {
                ushort value = 0;
                value = value.CombineUpperLower(this.d, this.e);
                return value;
            }
            internal set
            {
                this.d = value.GetUpper();
                this.e = value.GetLower();
                OnPropertyChanged();
            }
        }
        public ushort HL
        {
            get
            {
                ushort value = 0;
                value = value.CombineUpperLower(this.h, this.l);
                return value;
            }
            internal set
            {
                this.h = value.GetUpper();
                this.l = value.GetLower();
                OnPropertyChanged();
            }
        }
        #endregion


        #region 8-Bit Registers
        /// <summary>
        /// Accumulator
        /// </summary>
        public byte A
        {
            get
            {
                return this.a;
            }
            internal set
            {
                this.a = value;
                OnPropertyChanged();
            }
        }
        private byte a;

        public byte B
        {
            get
            {
                return this.b;
            }
            internal set
            {
                this.b = value;
                OnPropertyChanged();
            }
        }
        private byte b;

        public byte C
        {
            get
            {
                return this.c;
            }
            internal set
            {
                this.c = value;
                OnPropertyChanged();
            }
        }
        private byte c;

        public byte D
        {
            get
            {
                return this.d;
            }
            internal set
            {
                this.d = value;
                OnPropertyChanged();
            }
        }
        private byte d;

        public byte E
        {
            get
            {
                return this.e;
            }
            internal set
            {
                this.e = value;
                OnPropertyChanged();
            }
        }
        private byte e;

        public byte F
        {
            get
            {
                return this.f;
            }
            internal set
            {
                this.f = value;
                OnPropertyChanged();
            }
        }

        public byte H
        {
            get
            {
                return this.h;
            }
            internal set
            {
                this.h = value;
                OnPropertyChanged();
            }
        }
        private byte h;

        public byte L
        {
            get
            {
                return this.l;
            }
            internal set
            {
                this.l = value;
                OnPropertyChanged();
            }
        }
        private byte l;
        #endregion


        #region Flags
        /// <summary>
        /// Aritghmetic Flag Register
        /// </summary>
        private byte f;
        public bool Flag_Zero
        {
            get
            {
                return (this.f & (byte)Flag.Zero) != 0;
            }
            internal set
            {
                if (value)
                    this.f |= (byte)Flag.Zero;
                else
                    this.f &= (byte)Flag.NotZero;

                OnPropertyChanged();
            }
        }
        public bool Flag_Negation
        {
            get
            {
                return (this.f & (byte)Flag.Negation) != 0;
            }
            internal set
            {
                if (value)
                    this.f |= (byte)Flag.Negation;
                else
                    this.f &= (byte)Flag.NotNegation;

                OnPropertyChanged();
            }
        }
        public bool Flag_HalfCarry
        {
            get
            {
                return (this.f & (byte)Flag.HalfCarry) != 0;
            }
            internal set
            {
                if (value)
                    this.f |= (byte)Flag.HalfCarry;
                else
                    this.f &= (byte)Flag.NotHalfCarry;

                OnPropertyChanged();
            }
        }
        public bool Flag_Carry
        {
            get
            {
                return (this.f & (byte)Flag.Carry) != 0;
            }
            internal set
            {
                if (value)
                    this.f |= (byte)Flag.Carry;
                else
                    this.f &= (byte)Flag.NotCarry;

                OnPropertyChanged();
            }
        }
        public int Flag_Carry_int
        {
            get
            {
                if (Flag_Carry)
                    return 1;
                else
                    return 0;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public CPURegisters(MemoryManagementUnit.MMU mmu)
        {
            Reset();
        }
        public void Reset()
        {
            this.AF = 0x0;
            this.BC = 0x0;
            this.DE = 0x0;
            this.HL = 0x0;

            //this.A = 0x0;
            //this.B = 0x0;
            //this.C = 0x0;
            //this.D = 0x0;
            //this.E = 0x0;
            //this.H = 0x0;
            //this.L = 0x0;
            //this.F = 0x0;

            this.PC = 0x0; //0x100 ?
            this.SP = 0x0; //0xFFFE ?

            this.F = 0x0;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public enum Flag : byte
        {
            /// <summary>
            /// Z
            /// </summary>
            Zero = 0x80,
            /// <summary>
            /// Not Z
            /// </summary>
            NotZero = 0x70,

            /// <summary>
            /// N
            /// </summary>
            Negation = 0x40,
            /// <summary>
            /// Not N
            /// </summary>
            NotNegation = 0xB0,

            /// <summary>
            /// H
            /// </summary>
            HalfCarry = 0x20,
            /// <summary>
            /// Not H
            /// </summary>
            NotHalfCarry = 0xD0,

            /// <summary>
            /// C
            /// </summary>
            Carry = 0x10,
            /// <summary>
            /// Not C
            /// </summary>
            NotCarry = 0xE0
        }
    }
}
