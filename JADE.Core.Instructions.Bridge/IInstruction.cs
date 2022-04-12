using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Bridge
{
    public interface IInstruction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="parametersList"></param>
        /// <returns>Signals if all preperations are done or another value is required after receiving values</returns>
        bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode">The OP-Code thats processed right now (only necessery when one class handles multiple instructions)</param>
        /// <param name="parametersList">All Parameters that have been requested before</param>
        /// <param name="changesList">All changes that need to be done in the main kontext</param>
        /// <returns>The Machine Cycles that the process took</returns>
        byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList);
    }
}
