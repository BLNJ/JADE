using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class InstructionAttribute : Attribute
    {
        public ushort OpCode
        {
            get;
            private set;
        }
        public bool IsExtendedInstruction
        {
            get;
            private set;
        }
        public string Mnemoric
        {
            get;
            private set;
        }

        public InstructionAttribute(ushort opCode, string mnemoric, bool isExtendedInstruction = false)
        {
            this.OpCode = opCode;
            this.Mnemoric = mnemoric;
            this.IsExtendedInstruction = isExtendedInstruction;
        }
    }
}
