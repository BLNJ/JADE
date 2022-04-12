using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Memory
{
    public class RelativeMemoryInstructionParameterRequest : InstructionParameterRequestBase
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

        public RelativeMemoryInstructionParameterRequest(ParameterRequestType requestType, Register.ParameterRegister baseAddressRegister) : this(requestType, baseAddressRegister, 0)
        {
        }

        public RelativeMemoryInstructionParameterRequest(ParameterRequestType requestType, Register.ParameterRegister baseAddressRegister, long address) : base(ParameterRequestSource.MemoryRelative)
        {
            this.RequestType = requestType;
            this.BaseAddressRegister = baseAddressRegister;
            this.Address = address;
        }
    }
}
