using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JADE.Core.Registers
{
    public abstract class MemoryRegistersBase : INotifyPropertyChanged
    {
        internal MemoryManagementUnit.MMU mmu;

        public event PropertyChangedEventHandler PropertyChanged;

        public MemoryRegistersBase(MemoryManagementUnit.MMU mmu)
        {
            this.mmu = mmu;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
