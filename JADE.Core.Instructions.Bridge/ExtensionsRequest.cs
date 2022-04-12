using JADE.Core.Instructions.Bridge.Memory;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public static class ExtensionsRequest
    {
        public static void AddRegister(this List<InstructionParameterRequestBase> parametersList, ParameterRegister register)
        {
            parametersList.Add(new RegisterInstructionParameterRequest(register));
        }
        public static void AddMemory(this List<InstructionParameterRequestBase> parametersList, ParameterRequestType requestType)
        {
            parametersList.Add(new MemoryInstructionParameterRequest(requestType));
        }
        public static void AddRelativeMemory(this List<InstructionParameterRequestBase> parametersList, ParameterRequestType requestType, ParameterRegister baseAddressRegister)
        {
            parametersList.Add(new RelativeMemoryInstructionParameterRequest(requestType, baseAddressRegister));
        }
        public static void AddRelativeMemory(this List<InstructionParameterRequestBase> parametersList, ParameterRequestType requestType, ParameterRegister baseAddressRegister, long address)
        {
            parametersList.Add(new RelativeMemoryInstructionParameterRequest(requestType, baseAddressRegister, address));
        }
        public static void AddRegisterFlag(this List<InstructionParameterRequestBase> parametersList, ParameterFlag flag)
        {
            parametersList.Add(new RegisterFlagInstructionParameterRequest(flag));
        }
        public static void AddStackPop(this List<InstructionParameterRequestBase> parametersList, Stack.ParameterValueType valueType)
        {
            parametersList.Add(new Stack.StackInstructionParameterRequest(Bridge.Stack.ParameterType.POP, valueType));
        }
    }
}
