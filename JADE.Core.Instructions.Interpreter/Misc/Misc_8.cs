using JADE.Core.Instructions.Bridge;
using JADE.Core.Instructions.Bridge.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.Misc
{
    public static class Misc_8
    {
        [Instruction(0x3F, "CCF")]
        [Instruction(0x37, "SCF")]
        public class xCF : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                switch(opCode)
                {
                    case 0x3F:
                        parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry);
                        break;

                    case 0x37:
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                if (opCode == 0x3F)
                {
                    bool flagCarry = (bool)parametersList[0].Value;
                    changesList.AddRegisterFlag(ParameterFlag.Flag_Carry, !flagCarry);
                }
                else if(opCode == 0x37)
                {
                    changesList.AddRegisterFlag(ParameterFlag.Flag_Carry, true);
                }
                else
                {
                    throw new NotImplementedException();
                }

                changesList.AddRegisterFlag(ParameterFlag.Flag_Negation, false);
                changesList.AddRegisterFlag(ParameterFlag.Flag_HalfCarry, false);


                return 4;
            }
        }

        [Instruction(0x2F, "CPL A")]
        public class CPL_A : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;
                registerA = (byte)~registerA; // ~ = bitwise negation

                changesList.AddRegisterFlag(ParameterFlag.Flag_HalfCarry, true);
                changesList.AddRegisterFlag(ParameterFlag.Flag_Negation, true);
                changesList.AddRegister(ParameterRegister.A, registerA);

                return 4;
            }
        }

        [Instruction(0x27, "DAA A")]
        public class DAA_A : IInstruction
        {
            public bool PrepareParameters(byte opCode, ref List<InstructionParameterRequestBase> parametersList)
            {
                parametersList.AddRegister(ParameterRegister.A);
                parametersList.AddRegisterFlag(ParameterFlag.Flag_Carry);
                parametersList.AddRegisterFlag(ParameterFlag.Flag_HalfCarry);
                parametersList.AddRegisterFlag(ParameterFlag.Flag_Negation);

                return true;
            }

            public byte Process(byte opCode, ref List<InstructionParameterResponseBase> parametersList, ref List<InstructionParameterResponseBase> changesList)
            {
                byte registerA = (byte)parametersList[0].Value;
                bool flagCarry = (bool)parametersList[1].Value;
                bool flagHalfCarry = (bool)parametersList[2].Value;
                bool flagNegation = (bool)parametersList[3].Value;


                ushort correction = 0x00;

                if (flagCarry)
                {
                    correction = 0x60;
                }

                if (flagHalfCarry || (!flagNegation && ((registerA & 0x0F) > 9)))
                {
                    correction |= 0x06;
                }

                if (flagCarry || (!flagNegation && (registerA > 0x99)))
                {
                    correction |= 0x60;
                }

                if (flagNegation)
                {
                    registerA = (byte)(registerA - correction);
                }
                else
                {
                    registerA = (byte)(registerA + correction);
                }

                if (((correction << 2) & 0x100) != 0)
                {
                    changesList.AddRegisterFlag(ParameterFlag.Flag_Carry, true);
                }

                changesList.AddRegisterFlag(ParameterFlag.Flag_HalfCarry, false);
                if (registerA == 0)
                {
                    changesList.AddRegisterFlag(ParameterFlag.Flag_Zero, true);
                }
                else
                {
                    changesList.AddRegisterFlag(ParameterFlag.Flag_Zero, false);
                }

                changesList.AddRegister(ParameterRegister.A, registerA);

                return 4;
            }
        }
    }
}
