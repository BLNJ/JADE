using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JADE.Core.Bridge.CentralProcessingUnit
{
    public abstract class ProgramCounterBase<T> : CPUBaseComponent<T> where T : struct, IComparable, IConvertible
    {
        //public T Value
        //{
        //    get;
        //    internal set;
        //}
        public override T Value
        {
            get => throw new NotImplementedException();
            internal set => throw new NotImplementedException();
        }


        public ProgramCounterBase(CPUBase cpu) : base(cpu)
        {

        }
    }
}
