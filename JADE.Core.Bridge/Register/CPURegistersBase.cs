using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Bridge.Register
{
    public abstract class CPURegistersBase : MemoryRegistersBase
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

        public CPURegistersBase(MemoryManagementUnit.MMUBase mmu) : base(mmu)
        {
            
        }

        public override void Reset()
        {
            this.PC = 0x0;
            this.SP = 0x0;
        }
    }
}
