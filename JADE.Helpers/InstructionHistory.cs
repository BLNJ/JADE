using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Helpers
{
    public class InstructionHistory<parametersT, preparedParametersT, proposedChangesT> : IList<InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>>
    {
        int capacity = 0;
        List<InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>> internalList = new List<InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>>();

        public InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set => throw new NotImplementedException();
        }

        public int Count => internalList.Count;

        public bool IsReadOnly => false;

        public InstructionHistory(int capacity)
        {
            this.capacity = capacity;
        }

        public void AddEntry(ushort opCodePC, bool isExtended, byte opCode, parametersT parameters, preparedParametersT preparedParameters, proposedChangesT proposedChanges)
        {
            if(this.internalList.Count == this.capacity)
            {
                this.internalList.RemoveAt(capacity - 1);
            }

            this.internalList.Insert(0, new InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>()
            {
                OpCodeProgramCounter = opCodePC,
                IsExtended = isExtended,
                OpCode = opCode,
                Parameters = parameters,
                PreparedParameters = preparedParameters,
                ProposedChanges = proposedChanges
            });
        }

        public void Add(InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class InstructionHistoryEntry<parametersT, preparedParametersT, proposedChangesT>
    {
        public ushort OpCodeProgramCounter;
        public bool IsExtended = false;
        public byte OpCode;

        public parametersT Parameters;
        public preparedParametersT PreparedParameters;
        public proposedChangesT ProposedChanges;
    }
}
