using JADE.Core.Instructions.Interpreter.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.Instructions.Interpreter.UnitTest
{
    public class InstructionsImplementedTest
    {
        List<JADE.Core.Instructions.Bridge.InstructionAttribute> instructionAttributes = new List<JADE.Core.Instructions.Bridge.InstructionAttribute>();

        [OneTimeSetUp]
        public void Setup()
        {
            Type instructionInterfaceType = typeof(JADE.Core.Instructions.Bridge.IInstruction);
            Type instructionAttributeType = typeof(JADE.Core.Instructions.Bridge.InstructionAttribute);

            Type bla = typeof(BIT_8_BIT); //if I dont do that we cant "see" the Assembly

            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            System.Reflection.Assembly[] assemblies = currentAppDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                System.Reflection.Assembly assembly = assemblies[i];

                Type[] assemblyTypes = assembly.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    Type assemblyType = assemblyTypes[j];

                    if (instructionInterfaceType.IsAssignableFrom(assemblyType) &&
                        !assemblyType.IsAbstract)
                    {
                        object[] attributes = assemblyType.GetCustomAttributes(false);
                        for (int k = 0; k < attributes.Length; k++)
                        {
                            object attribute = attributes[k];

                            if (attribute.GetType() == instructionAttributeType)
                            {
                                JADE.Core.Instructions.Bridge.InstructionAttribute instructionAttribute = (JADE.Core.Instructions.Bridge.InstructionAttribute)attribute;
                                instructionAttributes.Add(instructionAttribute);
                            }
                        }
                    }
                }
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this.instructionAttributes.Clear();
        }

        [Test]
        public void CheckAllInstructionsExistTest()
        {
            int[] missingOpCodes = new int[]
            {
                0xCB,
                0xD3, 0xDB, 0xDD,
                0xE3, 0xE4, 0xEB, 0xEC, 0xED,
                0xF4, 0xFC, 0xFD
            };

            List<(bool, int)> unimplementedInstruction = new List<(bool, int)>();

            bool isExtended = false;
            for(int passes = 0; passes < 2; passes++)
            {
                for(int opCode = 0; opCode <= 0xFF; opCode++)
                {
                    if(!isExtended)
                    {
                        if(missingOpCodes.Contains(opCode))
                        {
                            continue;
                        }
                    }

                    JADE.Core.Instructions.Bridge.InstructionAttribute? instructionAttribute = this.instructionAttributes.Find(ins => ins.IsExtendedInstruction == isExtended && ins.OpCode == opCode);

                    if(instructionAttribute == null)
                    {
                        unimplementedInstruction.Add((isExtended, opCode));
                    }
                }

                isExtended = true;
            }

            if(unimplementedInstruction.Count > 0)
            {
                string message = String.Format("{0} Missing: ", unimplementedInstruction.Count);
                for(int i = 0; i < unimplementedInstruction.Count; i++)
                {
                    (bool, int) instruction = unimplementedInstruction[i];
                    message += string.Format("[{0}, {1}], {2}", instruction.Item1, instruction.Item2.ToString("X2"), Environment.NewLine);
                }

                Assert.Fail(message);
            }
            else
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CheckInstructionDoubles()
        {
            Dictionary<(bool, int), List<string>> temp = new Dictionary<(bool, int), List<string>>();

            for(int i = 0; i < this.instructionAttributes.Count; i++)
            {
                Bridge.InstructionAttribute instruction = this.instructionAttributes[i];

                if(temp.ContainsKey((instruction.IsExtendedInstruction, instruction.OpCode)))
                {
                    temp[(instruction.IsExtendedInstruction, instruction.OpCode)].Add(instruction.Mnemoric);
                }
                else
                {
                    temp.Add((instruction.IsExtendedInstruction, instruction.OpCode), new List<string>());
                }
            }


            string message = "";

            int doublesCount = 0;
            foreach (KeyValuePair<(bool, int), List<string>> kvp in temp)
            {
                if (kvp.Value.Count > 0)
                {
                    message += string.Format("[{0}, {1}] {2}{3}", kvp.Key.Item1, kvp.Key.Item2.ToString("X2"), kvp.Value.Count, Environment.NewLine);
                    doublesCount++;
                }
            }
            message = string.Format("{0} Doubles: ", doublesCount) + message;

            if (doublesCount > 0)
            {
                Assert.Fail(message);
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}
