using JADE.Core.Instructions.Bridge.Memory;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public static class ExtensionsResponse
    {
        public static void AddRegister(this List<InstructionParameterResponseBase> changesList, ParameterRegister register, object value)
        {
            Type valueType = value.GetType();
            if(valueType == typeof(byte))
            {
                AddRegister(changesList, register, (byte)value);
            }
            else if(valueType == typeof(ushort))
            {
                AddRegister(changesList, register, (ushort)value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void AddRegister(this List<InstructionParameterResponseBase> changesList, ParameterRegister register, byte value)
        {
            switch(register)
            {
                case ParameterRegister.AF:
                case ParameterRegister.DE:
                case ParameterRegister.HL:

                case ParameterRegister.PC:
                case ParameterRegister.SP:
                    throw new Exception();
            }

            changesList.Add(new RegisterInstructionParameterResponse(register, value));
        }

        public static void AddRegister(this List<InstructionParameterResponseBase> changesList, ParameterRegister register, ushort value)
        {
            switch (register)
            {
                case ParameterRegister.A:
                case ParameterRegister.F:
                case ParameterRegister.B:
                case ParameterRegister.C:
                case ParameterRegister.D:
                case ParameterRegister.E:
                case ParameterRegister.H:
                case ParameterRegister.L:
                    throw new Exception();
            }

            changesList.Add(new RegisterInstructionParameterResponse(register, value));
        }

        public static void AddRegisterCommit(this List<InstructionParameterResponseBase> changesList, RegisterCommit registerCommit)
        {
            changesList.Add(new RegisterInstructionParameterCommitResponse(registerCommit));
        }

        public static void AddRegisterFlag(this List<InstructionParameterResponseBase> changesList, ParameterFlag flag, object value)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(bool))
            {
                AddRegisterFlag(changesList, flag, (bool)value);
            }
            else if (valueType == typeof(int))
            {
                AddRegisterFlag(changesList, flag, (int)value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void AddRegisterFlag(this List<InstructionParameterResponseBase> changesList, ParameterFlag flag, bool value)
        {
            if(flag == ParameterFlag.Flag_Carry_int)
            {
                throw new Exception();
            }

            changesList.Add(new RegisterFlagInstructionParameterResponse(flag, value));
        }

        public static void AddRegisterFlag(this List<InstructionParameterResponseBase> changesList, ParameterFlag flag, int value)
        {
            if(flag != ParameterFlag.Flag_Carry_int)
            {
                throw new Exception();
            }

            changesList.Add(new RegisterFlagInstructionParameterResponse(flag, value));
        }

        public static void AddRelativeMemory(this List<InstructionParameterResponseBase> changesList, ParameterRequestType requestType, ParameterRegister baseAddressRegister, object value)
        {
            AddRelativeMemory(changesList, requestType, baseAddressRegister, 0, value);
        }

        public static void AddRelativeMemory(this List<InstructionParameterResponseBase> changesList, ParameterRequestType requestType, ParameterRegister baseAddressRegister, long address, object value)
        {
            changesList.Add(new RelativeMemoryInstructionParameterResponse(requestType, baseAddressRegister, address, value));
        }

        public static void AddMemory(this List<InstructionParameterResponseBase> changesList, ParameterRequestType requestType, object value)
        {
            changesList.Add(new MemoryInstructionParameterResponse(requestType, value));
        }

        public static void AddMemory(this List<InstructionParameterResponseBase> changesList, ParameterRequestType requestType, long address, object value)
        {
            changesList.Add(new MemoryInstructionParameterResponse(requestType, address, value));
        }

        public static void AddStackPop(this List<InstructionParameterResponseBase> changesList, object value)
        {
            Type valueType = value.GetType();
            if (valueType == typeof(byte))
            {
                AddStackPop(changesList, (byte)value);
            }
            else if (valueType == typeof(ushort))
            {
                AddStackPop(changesList, (ushort)value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static void AddStackPop(this List<InstructionParameterResponseBase> changesList, ushort value)
        {
            changesList.Add(new Stack.StackInstructionParameterResponse(Bridge.Stack.ParameterType.POP, Bridge.Stack.ParameterValueType.UnsignedShort, value));
        }
        public static void AddStackPop(this List<InstructionParameterResponseBase> changesList, byte value)
        {
            changesList.Add(new Stack.StackInstructionParameterResponse(Bridge.Stack.ParameterType.POP, Bridge.Stack.ParameterValueType.UnsignedByte, value));
        }

        public static void AddStackPush(this List<InstructionParameterResponseBase> changesList, ushort value)
        {
            changesList.Add(new Stack.StackInstructionParameterResponse(Bridge.Stack.ParameterType.PUSH, Bridge.Stack.ParameterValueType.UnsignedShort, value));
        }
        public static void AddStackPush(this List<InstructionParameterResponseBase> changesList, byte value)
        {
            changesList.Add(new Stack.StackInstructionParameterResponse(Bridge.Stack.ParameterType.PUSH, Bridge.Stack.ParameterValueType.UnsignedByte, value));
        }
        
        public static void AddCall(this List<InstructionParameterResponseBase> changesList, ushort value)
        {
            changesList.Add(new Jump.CallInstructionParameterResponse(value));
        }
    }
}
