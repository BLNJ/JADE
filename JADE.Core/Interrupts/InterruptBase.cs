using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JADE.Core.Interrupts
{
    public class InterruptBase : INotifyPropertyChanged
    {
        MemoryManagementUnit.MMU mmu;

        public ushort BaseAddress
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public InterruptBase(MemoryManagementUnit.MMU mmu, ushort baseAddress)
        {
            this.mmu = mmu;
            this.BaseAddress = baseAddress;
        }

        public byte ReadByte(ushort address)
        {
            byte flags = mmu.Stream.ReadByte(BaseAddress + address, jumpBack: true);
            return flags;
        }

        public void WriteByte(ushort address, byte value)
        {
            mmu.Stream.WriteByte(BaseAddress + address, value, jumpBack: true);
        }
    }
}
