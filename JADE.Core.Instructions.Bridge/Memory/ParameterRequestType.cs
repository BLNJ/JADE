using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge.Memory
{
    public enum ParameterRequestType
    {
        /// <summary>
        /// sbyte
        /// </summary>
        SignedByte,
        /// <summary>
        /// byte
        /// </summary>
        UnsignedByte,

        /// <summary>
        /// short
        /// </summary>
        SignedShort,
        /// <summary>
        /// ushort
        /// </summary>
        UnsignedShort,

        /// <summary>
        /// int
        /// </summary>
        SignedInteger,
        /// <summary>
        /// uint
        /// </summary>
        UnsignedInteger
    }
}
