﻿using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Jump
{
    public static class RET_Return
    {
        [Instruction(0xC9, "RET")]
        [Instruction(0xC0, "RET NZ")]
        [Instruction(0xC8, "RET Z")]
        [Instruction(0xD0, "RET NC")]
        [Instruction(0xD8, "RET C")]
        public class RET_x : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                ParameterFlag? flag = null;

                switch (opCode)
                {
                    case 0xC0:
                    case 0xC8:
                        flag = ParameterFlag.Flag_Negation;
                        break;

                    case 0xD0:
                    case 0xD8:
                        flag = ParameterFlag.Flag_Carry;
                        break;
                }

                if (opCode != 0xC9)
                {
                    parametersList.AddRegisterFlag(flag.Value);
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                bool process = true;

                if (opCode != 0xC9)
                {
                    bool flag = (bool)parametersList[0].Value;

                    if (opCode == 0xC0 || opCode == 0xD0)
                    {
                        if (!flag)
                        {
                            process = true;
                        }
                        else
                        {
                            process = false;
                        }
                    }
                    else
                    {
                        if (flag)
                        {
                            process = true;
                        }
                        else
                        {
                            process = false;
                        }
                    }
                }

                if (process)
                {
                    //InstructionMethods.Ret();
                    changesList.AddReturn();
                }

                return 8;
            }
        }

        [Instruction(0xD9, "RETI")]
        public class RETI : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                changesList.AddReturn();
                changesList.AddMasterInterrupt(true);

                return 8;
            }
        }
    }
}
