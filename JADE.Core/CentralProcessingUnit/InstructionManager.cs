using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JADE.Core.Instructions.Bridge;
using JADE.Helpers;

namespace JADE.Core.CentralProcessingUnit
{
    public class InstructionManager
    {
        CPU cpu;

        Dictionary<JADE.Core.Instructions.Bridge.InstructionAttribute, Type> instructions = new Dictionary<Instructions.Bridge.InstructionAttribute, Type>();

        //This is fucking ugly
        //I think Im having a stroke as Im reeakding tiadjnsdkia
        //JADE.Helpers.InstructionHistory<List<Instructions.Bridge.InstructionParameterRequestBase>, List<Instructions.Bridge.InstructionParameterResponseBase>, List<Instructions.Bridge.InstructionParameterResponseBase>> instructionHistory = new Helpers.InstructionHistory<List<InstructionParameterRequestBase>, List<InstructionParameterResponseBase>, List<InstructionParameterResponseBase>>(1000);

        public InstructionManager(CPU cpu)
        {
            this.cpu = cpu;
        }

        public void Initialize()
        {
            this.instructions.Clear();

            Type instructionInterfaceType = typeof(JADE.Core.Instructions.Bridge.IInstruction);
            Type instructionAttributeType = typeof(JADE.Core.Instructions.Bridge.InstructionAttribute);

            Type bla = typeof(JADE.Core.Instructions.Interpreter.Load.Load_8_LD); //for some fucking reason I cant see the Assembly, if I dont access it in any way... .NET Core is strange dude

            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            System.Reflection.Assembly[] assemblies = currentAppDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                System.Reflection.Assembly assembly = assemblies[i];
                Type[] assemblyTypes = assembly.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    Type assemblyType = assemblyTypes[j];

                    if (instructionInterfaceType.IsAssignableFrom(assemblyType) &&
                        !assemblyType.IsAbstract)
                    {
                        object[] attributes = assemblyType.GetCustomAttributes(false);
                        for (int k = 0; k < attributes.Length; k++)
                        {
                            object attribute = attributes[k];

                            if (attribute.GetType() == instructionAttributeType)
                            {
                                JADE.Core.Instructions.Bridge.InstructionAttribute instructionAttribute = (JADE.Core.Instructions.Bridge.InstructionAttribute)attribute;
                                instructions.Add(instructionAttribute, assemblyType);
                            }
                        }
                    }
                }
            }
        }

        public JADE.Core.Instructions.Bridge.IInstruction FindInstruction(byte opCode, bool isExtended)
        {
            foreach(KeyValuePair<Instructions.Bridge.InstructionAttribute, Type> kvp in instructions)
            {
                if(kvp.Key.OpCode == opCode && kvp.Key.IsExtendedInstruction == isExtended)
                {
                    JADE.Core.Instructions.Bridge.IInstruction instance = (JADE.Core.Instructions.Bridge.IInstruction)System.Activator.CreateInstance(kvp.Value);
                    return instance;
                }
            }

            return null;
        }

        public byte CycleInstruction(byte opCode, bool isExtended, ushort opCodeProgramCounter)
        {
            Instructions.Bridge.IInstruction instruction = this.FindInstruction(opCode, isExtended);
            if (instruction == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                List<Instructions.Bridge.InstructionParameterRequestBase> parameters = new List<Instructions.Bridge.InstructionParameterRequestBase>();
                List<Instructions.Bridge.InstructionParameterResponseBase> preparedParameters = new List<InstructionParameterResponseBase>();
                List<Instructions.Bridge.InstructionParameterResponseBase> proposedChanges = new List<Instructions.Bridge.InstructionParameterResponseBase>();

                bool finishPrepare;
                byte cycles;
                do
                {
                    parameters.Clear();
                    preparedParameters.Clear();
                    proposedChanges.Clear();

                    finishPrepare = instruction.PrepareParameters(opCode, ref parameters);
                    preparedParameters = processRequestedParameters(parameters);
                    
                    cycles = instruction.Process(opCode, ref preparedParameters, ref proposedChanges);

                    //This doesnt work right now since I cant do my hacky memory copy method
                    //Thank you .NET Core
                    //this.instructionHistory.AddEntry(opCodeProgramCounter, isExtended, opCode, parameters, preparedParameters, proposedChanges);
                    //Easy solution would be to fuck everything thats not Windows... but at that point I could go back to the stoneage and use .NET Framework

                    ProcessProposedChanges(proposedChanges);
                }
                while (!finishPrepare);

                return cycles;
            }
        }

        public void ProcessProposedChanges(List<Instructions.Bridge.InstructionParameterResponseBase> proposedChanges)
        {
            for(int i = 0; i < proposedChanges.Count; i++)
            {
                Instructions.Bridge.InstructionParameterResponseBase proposedChange = proposedChanges[i];
                Type changeType = proposedChange.GetType();

                if(changeType == typeof(Instructions.Bridge.Register.RegisterInstructionParameterResponse))
                {
                    Instructions.Bridge.Register.RegisterInstructionParameterResponse change = (Instructions.Bridge.Register.RegisterInstructionParameterResponse)proposedChange;
                    valueIntoParameterRegister(change.Register, change.Value);
                }
                else if(changeType == typeof(Instructions.Bridge.Register.RegisterFlagInstructionParameterResponse))
                {
                    Instructions.Bridge.Register.RegisterFlagInstructionParameterResponse change = (Instructions.Bridge.Register.RegisterFlagInstructionParameterResponse)proposedChange;
                    switch (change.Flag)
                    {
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Zero:
                            this.cpu.Registers.Flag_Zero = (bool)change.Value;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Negation:
                            this.cpu.Registers.Flag_Negation = (bool)change.Value;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_HalfCarry:
                            this.cpu.Registers.Flag_HalfCarry = (bool)change.Value;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Carry:
                            this.cpu.Registers.Flag_Carry = (bool)change.Value;
                            break;

                        case Instructions.Bridge.Register.ParameterFlag.Flag_Carry_int:
                            throw new Exception("Cant write into Flag_Carry_int");
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if(changeType == typeof(Instructions.Bridge.Register.RegisterInstructionParameterCommitResponse))
                {
                    Instructions.Bridge.Register.RegisterInstructionParameterCommitResponse change = (Instructions.Bridge.Register.RegisterInstructionParameterCommitResponse)proposedChange;
                    
                    if(change.Value.A != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.A, change.Value.A);
                    }
                    if (change.Value.F != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.F, change.Value.F);
                    }
                    if (change.Value.AF != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.AF, change.Value.AF);
                    }

                    if (change.Value.B != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.B, change.Value.B);
                    }
                    if (change.Value.C != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.C, change.Value.C);
                    }
                    if (change.Value.BC != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.BC, change.Value.BC);
                    }

                    if (change.Value.D != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.D, change.Value.D);
                    }
                    if (change.Value.E != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.E, change.Value.E);
                    }
                    if (change.Value.DE != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.DE, change.Value.DE);
                    }

                    if (change.Value.H != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.H, change.Value.H);
                    }
                    if (change.Value.L != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.L, change.Value.L);
                    }
                    if (change.Value.HL != null)
                    {
                        valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister.HL, change.Value.HL);
                    }

                    if(change.Value.Flag_Carry != null)
                    {
                        this.cpu.Registers.Flag_Carry = change.Value.Flag_Carry.Value;
                    }
                    if (change.Value.Flag_HalfCarry != null)
                    {
                        this.cpu.Registers.Flag_HalfCarry = change.Value.Flag_HalfCarry.Value;
                    }
                    if (change.Value.Flag_Negation != null)
                    {
                        this.cpu.Registers.Flag_Negation = change.Value.Flag_Negation.Value;
                    }
                    if (change.Value.Flag_Zero != null)
                    {
                        this.cpu.Registers.Flag_Zero = change.Value.Flag_Zero.Value;
                    }
                    if (change.Value.Flag_Carry_int != null)
                    {
                        throw new NotImplementedException("Flag_Carry_int is readonly");
                    }
                }
                else if(changeType == typeof(Instructions.Bridge.Memory.MemoryInstructionParameterResponse))
                {
                    Instructions.Bridge.Memory.MemoryInstructionParameterResponse change = (Instructions.Bridge.Memory.MemoryInstructionParameterResponse)proposedChange;
                    
                    if(change.Address == null)
                    {
                        throw new Exception("Address is null");
                    }
                    else
                    {
                        Type valueType = change.Value.GetType();

                        if(valueType == typeof(byte))
                        {
                            this.cpu.MMU.Stream.WriteByte(change.Address.Value, (byte)change.Value);
                        }
                        else if(valueType == typeof(ushort))
                        {
                            this.cpu.MMU.Stream.WriteUShort(change.Address.Value, (ushort)change.Value);
                        }
                        else if (valueType == typeof(short))
                        {
                            this.cpu.MMU.Stream.WriteShort(change.Address.Value, (short)change.Value);
                        }
                        else
                        {
                            throw new NotImplementedException("Not implemented Memory ValueType: " + valueType);
                        }
                    }
                }
                else if (changeType == typeof(Instructions.Bridge.Memory.RelativeMemoryInstructionParameterResponse))
                {
                    Instructions.Bridge.Memory.RelativeMemoryInstructionParameterResponse change = (Instructions.Bridge.Memory.RelativeMemoryInstructionParameterResponse)proposedChange;

                    long address = change.Address;

                    long realAddress;

                    switch(change.BaseAddressRegister)
                    {
                        case Instructions.Bridge.Register.ParameterRegister.A:
                            realAddress = (this.cpu.Registers.A + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.F:
                            realAddress = (this.cpu.Registers.F + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.AF:
                            realAddress = (this.cpu.Registers.AF + change.Address);
                            break;

                        case Instructions.Bridge.Register.ParameterRegister.B:
                            realAddress = (this.cpu.Registers.B + change.Address);
                            this.cpu.MMU.Stream.Write(this.cpu.Registers.B + change.Address, change.Value);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.C:
                            realAddress = (this.cpu.Registers.C + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.BC:
                            realAddress = (this.cpu.Registers.BC + change.Address);
                            break;

                        case Instructions.Bridge.Register.ParameterRegister.D:
                            realAddress = (this.cpu.Registers.D + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.E:
                            realAddress = (this.cpu.Registers.E + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.DE:
                            realAddress = (this.cpu.Registers.DE + change.Address);
                            break;

                        case Instructions.Bridge.Register.ParameterRegister.H:
                            realAddress = (this.cpu.Registers.H + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.L:
                            realAddress = (this.cpu.Registers.L + change.Address);
                            break;
                        case Instructions.Bridge.Register.ParameterRegister.HL:
                            realAddress = (this.cpu.Registers.HL + change.Address);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    this.cpu.MMU.Stream.Write(realAddress, change.Value);
                }
                else if (changeType == typeof(Instructions.Bridge.Stack.StackInstructionParameterResponse))
                {
                    Instructions.Bridge.Stack.StackInstructionParameterResponse change = (Instructions.Bridge.Stack.StackInstructionParameterResponse)proposedChange;
                    switch (change.ValueType)
                    {
                        case Instructions.Bridge.Stack.ParameterValueType.UnsignedByte:
                            this.cpu.Stack.PushByte((byte)change.Value);
                            break;
                        case Instructions.Bridge.Stack.ParameterValueType.UnsignedShort:
                            this.cpu.Stack.PushUShort((ushort)change.Value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else if(changeType == typeof(Instructions.Bridge.Jump.CallInstructionParameterResponse))
                {
                    Instructions.Bridge.Jump.CallInstructionParameterResponse change = (Instructions.Bridge.Jump.CallInstructionParameterResponse)proposedChange;
                    this.cpu.Stack.PushUShort(this.cpu.ProgramCounter.Value);
                    this.cpu.ProgramCounter.Value = (ushort)change.Value;
                }
                else if(changeType == typeof(Instructions.Bridge.Jump.ReturnInstructionParameterResponse))
                {
                    ushort pc = this.cpu.Stack.PopUShort();
                    this.cpu.ProgramCounter.Value = pc;
                }
                else if(changeType == typeof(Instructions.Bridge.Jump.JumpInstructionParameterResponse))
                {
                    Instructions.Bridge.Jump.JumpInstructionParameterResponse change = (Instructions.Bridge.Jump.JumpInstructionParameterResponse)proposedChange;

                    Type valueType = change.Value.GetType();
                    if (valueType == typeof(ushort))
                    {
                        this.cpu.ProgramCounter.Value = (ushort)change.Value;
                    }
                    else if(valueType == typeof(sbyte))
                    {
                        this.cpu.ProgramCounter.Value = (ushort)(this.cpu.ProgramCounter.Value + ((sbyte)change.Value));
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown JumpInstructionParameterResponse ValueType: " + valueType.ToString());
                    }
                }
                else if(changeType == typeof(Instructions.Bridge.MasterInterrupt.MasterInterruptInstructionParameterResponse))
                {
                    bool status = (bool)proposedChange.Value;
                    this.cpu.InterruptMasterEnable = status;
                }
                else
                {
                    throw new NotImplementedException("Couldnt process unknown Type: " + changeType.ToString());
                }
            }
        }

        private List<InstructionParameterResponseBase> processRequestedParameters(List<InstructionParameterRequestBase> parameters)
        {
            List<Instructions.Bridge.InstructionParameterResponseBase> preparedParameters = new List<Instructions.Bridge.InstructionParameterResponseBase>();
            for (int i = 0; i < parameters.Count; i++)
            {
                Instructions.Bridge.InstructionParameterRequestBase parameter = parameters[i];
                Type parameterType = parameter.GetType();

                if (parameterType == typeof(Instructions.Bridge.Register.RegisterInstructionParameterRequest))
                {
                    Instructions.Bridge.Register.RegisterInstructionParameterRequest request = (Instructions.Bridge.Register.RegisterInstructionParameterRequest)parameter;

                    object value = parameterRegisterToValue(request.Register);

                    preparedParameters.AddRegister(request.Register, value);
                }
                else if (parameterType == typeof(Instructions.Bridge.Register.RegisterFlagInstructionParameterRequest))
                {
                    Instructions.Bridge.Register.RegisterFlagInstructionParameterRequest request = (Instructions.Bridge.Register.RegisterFlagInstructionParameterRequest)parameter;

                    object registerFlag;
                    switch (request.Flag)
                    {
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Zero:
                            registerFlag = this.cpu.Registers.Flag_Zero;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Negation:
                            registerFlag = this.cpu.Registers.Flag_Negation;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_HalfCarry:
                            registerFlag = this.cpu.Registers.Flag_HalfCarry;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Carry:
                            registerFlag = this.cpu.Registers.Flag_Carry;
                            break;
                        case Instructions.Bridge.Register.ParameterFlag.Flag_Carry_int:
                            registerFlag = this.cpu.Registers.Flag_Carry_int;
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    preparedParameters.AddRegisterFlag(request.Flag, registerFlag);
                }
                else if (parameterType == typeof(Instructions.Bridge.Memory.MemoryInstructionParameterRequest))
                {
                    Instructions.Bridge.Memory.MemoryInstructionParameterRequest request = (Instructions.Bridge.Memory.MemoryInstructionParameterRequest)parameter;

                    object value;
                    if (request.Address == null)
                    {
                        Type valueType;

                        switch (request.RequestType)
                        {
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedByte:
                                valueType = typeof(sbyte);
                                value = this.cpu.MMU.Stream.ReadSByte(this.cpu.Registers.PC);
                                this.cpu.Registers.PC++;
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedByte:
                                valueType = typeof(byte);
                                value = this.cpu.MMU.Stream.ReadByte(this.cpu.Registers.PC);
                                this.cpu.Registers.PC++;
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedShort:
                                valueType = typeof(short);
                                value = this.cpu.MMU.Stream.ReadShort(this.cpu.Registers.PC);
                                this.cpu.Registers.PC += 2;
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedShort:
                                valueType = typeof(ushort);
                                value = this.cpu.MMU.Stream.ReadUShort(this.cpu.Registers.PC);
                                this.cpu.Registers.PC += 2;
                                break;

                            default:
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedInteger:
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedInteger:
                                throw new NotImplementedException("Unknown RequestType: " + request.RequestType);
                        }

                        preparedParameters.AddMemory(request.RequestType, value);
                    }
                    else
                    {
                        switch (request.RequestType)
                        {
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedByte:
                                value = this.cpu.MMU.Stream.ReadSByte(request.Address.Value);
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedByte:
                                value = this.cpu.MMU.Stream.ReadByte(request.Address.Value);
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedShort:
                                value = this.cpu.MMU.Stream.ReadShort(request.Address.Value);
                                break;
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedShort:
                                value = this.cpu.MMU.Stream.ReadUShort(request.Address.Value);
                                break;

                            default:
                            case Instructions.Bridge.Memory.ParameterRequestType.SignedInteger:
                            case Instructions.Bridge.Memory.ParameterRequestType.UnsignedInteger:
                                throw new NotImplementedException("Unknown RequestType: " + request.RequestType);
                        }

                        preparedParameters.AddMemory(request.RequestType, request.Address.Value, value);
                    }
                }
                else if (parameterType == typeof(Instructions.Bridge.Memory.RelativeMemoryInstructionParameterRequest))
                {
                    Instructions.Bridge.Memory.RelativeMemoryInstructionParameterRequest request = (Instructions.Bridge.Memory.RelativeMemoryInstructionParameterRequest)parameter;

                    ushort baseAddress = (ushort)parameterRegisterToValue(request.BaseAddressRegister);

                    baseAddress = (ushort)(baseAddress + request.Address);

                    object value;
                    switch (request.RequestType)
                    {
                        case Instructions.Bridge.Memory.ParameterRequestType.SignedByte:
                            value = this.cpu.MMU.Stream.ReadSByte(baseAddress);
                            break;
                        case Instructions.Bridge.Memory.ParameterRequestType.UnsignedByte:
                            value = this.cpu.MMU.Stream.ReadByte(baseAddress);
                            break;
                        case Instructions.Bridge.Memory.ParameterRequestType.SignedShort:
                            value = this.cpu.MMU.Stream.ReadShort(baseAddress);
                            break;
                        case Instructions.Bridge.Memory.ParameterRequestType.UnsignedShort:
                            value = this.cpu.MMU.Stream.ReadUShort(baseAddress);
                            break;

                        default:
                        case Instructions.Bridge.Memory.ParameterRequestType.SignedInteger:
                        case Instructions.Bridge.Memory.ParameterRequestType.UnsignedInteger:
                            throw new NotImplementedException("Unknown RequestType: " + request.RequestType);
                    }

                    preparedParameters.AddRelativeMemory(request.RequestType, request.BaseAddressRegister, value);
                }
                else if (parameterType == typeof(Instructions.Bridge.Stack.StackInstructionParameterRequest))
                {
                    Instructions.Bridge.Stack.StackInstructionParameterRequest request = (Instructions.Bridge.Stack.StackInstructionParameterRequest)parameter;

                    object value;
                    switch (request.ValueType)
                    {
                        case Instructions.Bridge.Stack.ParameterValueType.UnsignedByte:
                            value = this.cpu.Stack.PopByte();
                            break;
                        case Instructions.Bridge.Stack.ParameterValueType.UnsignedShort:
                            value = this.cpu.Stack.PopUShort();
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    preparedParameters.AddStackPop(value);
                }
                else
                {
                    throw new NotImplementedException("Couldnt process unknown Type: " + parameterType.ToString());
                }
            }

            return preparedParameters;
        }

        private object parameterRegisterToValue(Instructions.Bridge.Register.ParameterRegister parameterRegister)
        {
            object value;
            switch (parameterRegister)
            {
                case Instructions.Bridge.Register.ParameterRegister.A:
                    value = this.cpu.Registers.A;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.F:
                    value = this.cpu.Registers.F;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.AF:
                    value = this.cpu.Registers.AF;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.B:
                    value = this.cpu.Registers.B;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.C:
                    value = this.cpu.Registers.C;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.BC:
                    value = this.cpu.Registers.BC;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.D:
                    value = this.cpu.Registers.D;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.E:
                    value = this.cpu.Registers.E;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.DE:
                    value = this.cpu.Registers.DE;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.H:
                    value = this.cpu.Registers.H;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.L:
                    value = this.cpu.Registers.L;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.HL:
                    value = this.cpu.Registers.HL;
                    break;

                case Instructions.Bridge.Register.ParameterRegister.PC:
                case Instructions.Bridge.Register.ParameterRegister.SP:
                default:
                    throw new Exception("Cant request register " + parameterRegister);
            }

            return value;
        }

        private void valueIntoParameterRegister(Instructions.Bridge.Register.ParameterRegister parameterRegister, object value)
        {
            switch (parameterRegister)
            {
                case Instructions.Bridge.Register.ParameterRegister.A:
                    this.cpu.Registers.A = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.F:
                    this.cpu.Registers.F = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.AF:
                    this.cpu.Registers.AF = (ushort)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.B:
                    this.cpu.Registers.B = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.C:
                    this.cpu.Registers.C = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.BC:
                    this.cpu.Registers.BC = (ushort)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.D:
                    this.cpu.Registers.D = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.E:
                    this.cpu.Registers.E = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.DE:
                    this.cpu.Registers.DE = (ushort)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.H:
                    this.cpu.Registers.H = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.L:
                    this.cpu.Registers.L = (byte)value;
                    break;
                case Instructions.Bridge.Register.ParameterRegister.HL:
                    this.cpu.Registers.HL = (ushort)value;
                    break;

                case Instructions.Bridge.Register.ParameterRegister.SP:
                    this.cpu.Registers.SP = (ushort)value;
                    break;

                case Instructions.Bridge.Register.ParameterRegister.PC:
                default:
                    throw new Exception("Cant request register " + parameterRegister);
            }
        }
    }
}
