using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JADE.Core.Bridge.Register
{
    public abstract class MemoryRegistersBase : INotifyPropertyChanged
    {
        internal MemoryManagementUnit.MMUBase mmu;

        public event PropertyChangedEventHandler PropertyChanged;

        public MemoryRegistersBase(MemoryManagementUnit.MMUBase mmu)
        {
            this.mmu = mmu;

            Reset();
        }

        public abstract void Reset();

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
