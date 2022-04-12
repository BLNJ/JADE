using System;
using System.Collections.Generic;
using System.Text;

namespace JADE.Core.Instructions.Bridge
{
    public abstract class InstructionParameterBase
    {
        public virtual ParameterRequestSource Source
        {
            get;
            private set;
        }

        public ParameterMethod Method
        {
            get;
            private set;
        }

        public InstructionParameterBase(ParameterRequestSource source, ParameterMethod method)
        {
            this.Source = source;
            this.Method = method;
        }

        public enum ParameterMethod
        {
            Get,
            Set
        }

        public enum ParameterRequestSource
        {
            Register,
            RegisterCommit,
            Flag,
            Memory,
            MemoryRelative,
            Stack
        }
    }
}
