using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Memory
{
    public class MemoryInstructionParameterResponse : InstructionParameterResponseBase
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

        public MemoryInstructionParameterResponse(ParameterRequestType requestType, object value) : base(ParameterRequestSource.Memory, value)
        {
            this.RequestType = requestType;
            this.Address = null;
        }

        public MemoryInstructionParameterResponse(ParameterRequestType requestType, long address, object value) : base(ParameterRequestSource.Memory, value)
        {
            this.RequestType = requestType;
            this.Address = address;
        }
    }
}
