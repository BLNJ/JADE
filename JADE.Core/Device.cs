using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using JADE.IO;

namespace JADE.Core
{
    public class Device : INotifyPropertyChanged
    {
        public CentralProcessingUnit.CPU CPU
        {
            get;
            private set;
        }

        public PictureProcessingUnit.PPU PPU
        {
            get;
            private set;
        }

        public MemoryManagementUnit.MMU MMU
        {
            get;
            private set;
        }

        public ReadOnlyMemory.ROM ROM
        {
            get;
            private set;
        }

        internal byte[] ioMapped;
        public IO.TriggerStream MappedIO
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        string status;
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                OnPropertyChanged();
            }
        }

        public delegate void Tick(object sender);
        public event Tick DebugTick;

        public Device()
        {
            this.MMU = new MemoryManagementUnit.MMU(this);
            this.CPU = new CentralProcessingUnit.CPU(this);
            this.PPU = new PictureProcessingUnit.PPU(this);

            Reset();

            this.Status = "Nothing";
        }

        public void Start()
        {
            this.Status = "Running";

            Thread testThread = new Thread(testLoop);
            testThread.Name = "testLoop";
            testThread.Start();

            //testLoop();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Reset(bool loadBootloader = false)
        {
            this.MMU.Reset(loadBootloader: loadBootloader);

            this.ioMapped = new byte[0x80];
            this.MappedIO = new TriggerStream(new FilledMemoryStream(this.ioMapped, random: false));
            this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.IO, 0xFF00, 0x80, this.MappedIO, 0);

            //TODO 'Subscribe' doesnt know anthing about its range
            //Maybe Add "Subsribe" to MMU and wrap a TriggerStream around the Stream if necessary?
            this.MappedIO.Subscribe((0xFF50 - 0xFF00), onWrite: IODisableBootrom);

            this.CPU.Reset();
            this.PPU.Reset();
        }

        private void testLoop()
        {
            byte cpuCycles = 0;

            while (true)
            {
                cpuCycles = 0;
                for(int i = 0; i < 2; i++)
                {
                    cpuCycles += this.CPU.Cycle();
                }

                //for (int i = 0; i < cpuCycles; i++)
                //{
                this.PPU.Cycle(1);
                //}


                OnDebugTick();
            }
        }

        private void IODisableBootrom(TriggerStream triggerStream, long offset, byte[] buffer)
        {
            Console.WriteLine("Bootrom disabled");
            //Lets unmap the Bootrom, since we need the space for the ROM´s Full Header
            this.MMU.RemoveMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.Bootstrap);

            //We needed the Header before, but only used a small (0x100 - 0x150) part, so lets unmap the Header...
            //this.MMU.RemoveMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeHeader);
            //...and map it fully
            //this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeHeader, 0x0, 0x150, this.ROM.Stream, 0x0);
        }

        public void InsertROM(ReadOnlyMemory.ROM rom)
        {
            if (this.ROM == null)// && rom.device == null)
            {
                this.ROM = rom;

                //0100-014F
                //this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeHeader, 0x100, 0x50, this.ROM.Stream, 0x100);
                //0150-3FFF
                //this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_Bank0, 0x0150, 0x3EB0, this.ROM.Stream, 0x0150);

                //this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_Bank0, 0x0, 0x4000, this.ROM.Stream, 0x0);
                this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_Bank0, 0x0, 0x4000, new ReadOnlyMemory.Streams.Bank0Stream(this.ROM.Stream), 0x0);

                //TODO Debug
                //4000-7FFF
                this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_BankX, 0x4000, 0x4000, this.ROM.Stream, 0x4000);

                //TODO Debug
                //A000-BFFF
                this.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeRAM, 0xA000, 0x2000, random: false);
            }
            else
            {
                throw new Exception("ROM already inserted");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnDebugTick()
        {
            Tick handler = this.DebugTick;
            if(handler != null)
            {
                handler(this);
            }
        }
    }
}
