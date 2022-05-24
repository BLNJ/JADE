using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JADE.Core.Registers;
using JADE.Core.Instructions.Bridge;

namespace JADE.Core.CentralProcessingUnit
{
    public class CPU
    {
        Device device;
        InstructionManager instructionManager;

        public CPURegisters Registers
        {
            get;
            private set;
        }

        public Stack Stack
        {
            get;
            private set;
        }
        public ProgramCounter ProgramCounter
        {
            get;
            private set;
        }
        public MemoryManagementUnit.MMU MMU
        {
            get
            {
                return this.device.MMU;
            }
        }

        private bool interruptMasterEnable = false;
        public bool InterruptMasterEnable
        {
            get
            {
                return interruptMasterEnable;
            }
            set
            {
                this.interruptMasterEnable = value;
            }
        }
        public Interrupts.CPUInterrupts InterruptFlags
        {
            get;
            internal set;
        }
        public Interrupts.CPUInterrupts InterruptEnabled
        {
            get;
            internal set;
        }

        MemoryStream RAM;

        internal bool disableInterruptsPending;
        internal bool enableInterruptsPending;

        public CPU(Device device)
        {
            this.device = device;
            this.Registers = new Registers.CPURegisters(this.MMU);

            this.Stack = new Stack(this);
            this.ProgramCounter = new ProgramCounter(this);

            this.InterruptFlags = new Interrupts.CPUInterrupts(this.MMU, 0xFF0F);
            this.InterruptEnabled = new Interrupts.CPUInterrupts(this.MMU, 0xFFFF);

            this.instructionManager = new InstructionManager(this);
        }

        public void Reset()
        {
            this.Registers.Reset();
            this.enableInterruptsPending = false;
            this.disableInterruptsPending = false;

            this.RAM = new IO.FilledMemoryStream(0x2000, random: true);
            // Working RAM
            this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.RAM, 0xC000, 0x2000, this.RAM, 0);
            // Shadow RAM
            this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.ShadowRAM, 0xE000, 0x1E00, this.RAM, 0);

            this.instructionManager.Initialize();
        }

        public void Step()
        {
            if (this.InterruptMasterEnable) //TODO checks missing?
            {
                //Push current PC to stack so we can return after the interrupt
                //this.Stack.PushUShort(this.Registers.PC);

                if (this.InterruptEnabled.VBlank)
                {
                    JADE.Core.Instructions.Bridge.InstructionMethods.Call(0x40);
                }
                else if (this.InterruptEnabled.LCD_STAT)
                {
                    JADE.Core.Instructions.Bridge.InstructionMethods.Call(0x48);
                }
                else if (this.InterruptEnabled.Timer)
                {
                    JADE.Core.Instructions.Bridge.InstructionMethods.Call(0x50);
                }
                else if (this.InterruptEnabled.Serial)
                {
                    JADE.Core.Instructions.Bridge.InstructionMethods.Call(0x58);
                }
                else if (this.InterruptEnabled.Joypad)
                {
                    JADE.Core.Instructions.Bridge.InstructionMethods.Call(0x60);
                }

                this.InterruptMasterEnable = false;
            }

            //TODO this is just to skip the VRAM clearing process
            if (this.Registers.PC == 0x4)
            {
                this.Registers.PC = 0xC;
            }

            byte op = this.MMU.Stream.ReadByte(this.Registers.PC);
            this.Registers.PC++;

            this.device.Status = string.Format("[CPU] FETCH: {0}", op);

            bool isExtended = false;
            if (op == 0xCB)
            {
                isExtended = true;
                op = this.MMU.Stream.ReadByte(this.Registers.PC);
                this.Registers.PC++;

                this.device.Status += string.Format(" {0}", op);
            }

            //Core.Instructions.IInstruction instruction = this.device.pluginHost.findInstruction(op, isExtended);
            //if (instruction == null)
            //{
            //    throw new NotImplementedException();
            //}
            //else
            //{
            //    byte cycles = instruction.getCycles();

            //    string mnemoric = instruction.getMnemoric();
            //    this.device.Status = string.Format("[CPU] DECODE: {0}", mnemoric);

            //    this.device.Status = "[CPU] EXECUTE";
            //    instruction.Process(this);

            //    //TODO find a cleaner way
            //    // 0xF3 = DI
            //    // 0xFB = EI
            //    if (op != 0xF3 || op != 0xFB)
            //    {
            //        if (enableInterruptsPending)
            //        {
            //            this.InterruptMasterEnable = true;
            //            enableInterruptsPending = false;
            //        }
            //        if (disableInterruptsPending)
            //        {
            //            this.InterruptMasterEnable = false;
            //            disableInterruptsPending = false;
            //        }
            //    }

            //    this.previousPC = this.Registers.PC;
            //    this.previousInstruction = instruction;

            //    return cycles;
            //}

            byte cycles = this.instructionManager.CycleInstruction(op, isExtended);
        }

    }
}
