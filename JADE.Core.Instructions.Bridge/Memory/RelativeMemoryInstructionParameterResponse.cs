using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Memory
{
    public class RelativeMemoryInstructionParameterResponse : InstructionParameterResponseBase
    {
        public ParameterRequestType RequestType
        {
            get;
            private set;
        }
        public Register.ParameterRegister BaseAddressRegister
        {
            get;
            private set;
        }
        public long Address
        {
            get;
            private set;
        }

        public RelativeMemoryInstructionParameterResponse(ParameterRequestType requestType, Register.ParameterRegister baseAddressRegister, long address, object value) : base(ParameterRequestSource.MemoryRelative, value)
        {
            this.RequestType = requestType;
            this.BaseAddressRegister = baseAddressRegister;
            this.Address = address;
        }
    }
}
