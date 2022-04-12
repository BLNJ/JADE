using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Memory
{
    public class MemoryInstructionParameterRequest : InstructionParameterRequestBase
    {
        public ParameterRequestType RequestType
        {
            get;
            private set;
        }

        public long? Address
        {
            get;
            private set;
        }

        public MemoryInstructionParameterRequest(ParameterRequestType requestType) : base(ParameterRequestSource.Memory)
        {
            this.RequestType = requestType;
            this.Address = null;
        }

        public MemoryInstructionParameterRequest(ParameterRequestType requestType, long address) : base(ParameterRequestSource.Memory)
        {
            this.RequestType = requestType;
            this.Address = address;
        }
    }
}
