using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.UnitTest
{
    public class StackTest
    {
        ushort stackPosition = 0xFFF;
        JADE.Core.Device device;

        [SetUp]
        public void Setup()
        {
            this.device = new Device();
            device.Reset();

            device.MMU.RemoveAllMappedStreams();
            device.MMU.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.RAM, 0, (ushort)(stackPosition + 1), random: false);
            device.CPU.Registers.SP = stackPosition;
        }

        [Test]
        public void StackByteTest()
        {
            Random random = new Random();
            byte[] randomValues = new byte[stackPosition];
            random.NextBytes(randomValues);

            for(int i = 0; i < randomValues.Length; i++)
            {
                byte value = randomValues[i];
                device.CPU.Stack.PushByte(value);
                
                if(device.CPU.Registers.SP != (stackPosition - (i + 1)))
                {
                    Assert.Fail("[{0}] Write - SP doesnt match", i);
                }
            }

            Array.Reverse(randomValues);
            ushort loadedStack = device.CPU.Registers.SP;

            for(int i = 0; i < randomValues.Length; i++)
            {
                byte value = device.CPU.Stack.PopByte();
                byte expectedValue = randomValues[i];

                if (device.CPU.Registers.SP != (loadedStack + i + 1))
                {
                    Assert.Fail("[{0}] Read - SP doesnt match", i);
                }

                if(value != expectedValue)
                {
                    Assert.Fail("[{0}] Read - Value doesnt match: {1} {2}", i, value, expectedValue);
                }
            }
        }

        [Test]
        public void StackUShortTest()
        {
            Random random = new Random();
            ushort[] randomValues = new ushort[stackPosition / 2];
            for (int i = 0; i < randomValues.Length; i++)
            {
                ushort value = (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
                randomValues[i] = value;
            }

            for (int i = 0; i < randomValues.Length; i++)
            {
                ushort value = randomValues[i];
                device.CPU.Stack.PushUShort(value);

                if (device.CPU.Registers.SP != (stackPosition - ((i * 2) + 2)))
                {
                    Assert.Fail("[{0}] Write - SP doesnt match", i);
                }
            }

            Array.Reverse(randomValues);
            ushort loadedStack = device.CPU.Registers.SP;

            for (int i = 0; i < randomValues.Length; i++)
            {
                ushort value = device.CPU.Stack.PopUShort();
                ushort expectedValue = randomValues[i];

                if (device.CPU.Registers.SP != (loadedStack + (i * 2) + 2))
                {
                    Assert.Fail("[{0}] Read - SP doesnt match", i);
                }

                if (value != expectedValue)
                {
                    Assert.Fail("[{0}] Read - Value doesnt match: {1} {2}", i, value, expectedValue);
                }
            }
        }
    }
}
