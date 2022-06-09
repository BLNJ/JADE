using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JADE.Core.UnitTest
{
    public class MMUTest
    {
        ushort testStreamsCount = 10;
        ushort testStreamsSize = 10;

        JADE.Core.Device device;
        JADE.Core.MemoryManagementUnit.MMU mmu
        {
            get
            {
                return device.MMU;
            }
        }

        MemoryManagementUnit.MappedMemoryRegion topMostRegion = null;
        List<MemoryManagementUnit.MappedMemoryRegion> regions = new List<MemoryManagementUnit.MappedMemoryRegion>();

        [SetUp]
        public void Setup()
        {
            this.device = new Device();
            device.Reset();
            mmu.RemoveAllMappedStreams();

            this.topMostRegion = mmu.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_BankX, 0, (ushort)(testStreamsSize), topMost: true);
            fillStream(topMostRegion.ExternalMemory, 0, 0x69);

            for (int i = 1; i < testStreamsCount; i++)
            {
                MemoryManagementUnit.MappedMemoryRegion region = mmu.AddMappedStream(MemoryManagementUnit.MappedMemoryRegion.Name.CartridgeROM_BankX, 0, (ushort)(testStreamsSize * (i + 1)), topMost: false);
                fillStream(region.ExternalMemory, i, (byte)i);

                this.regions.Add(region);
            }
        }

        private void fillStream(IO.ExternalMemory stream, int index, byte data)
        {
            stream.Position = 0;
            for(int i = 0; i < (testStreamsSize * (index + 1)); i++)
            {
                stream.WriteByte((byte)data);
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.regions.Clear();

            mmu.RemoveAllMappedStreams();
            this.device.Reset();
        }

        [Test]
        public void OverlayingMemoryFindMemoryTest()
        {
            int totalSize = (testStreamsCount * testStreamsSize) - 1;
            MemoryManagementUnit.MappedMemoryRegion latestRegion = this.regions.Last();

            for (int i = 0; i < totalSize; i++)
            {
                MemoryManagementUnit.MappedMemoryRegion region = this.mmu.FindMappedMemory((ushort)i);

                if(region == null)
                {
                    Assert.Fail("Stream at position {0} was null", i);
                }
                else
                {
                    if(i < testStreamsSize)
                    {
                        if(region != this.topMostRegion)
                        {
                            Assert.Fail("Expected TopMost Stream at {0} but received Name {1}:{2}", i, region.RegionName, region.RegionIteration);
                        }
                    }
                    else
                    {
                        
                        if (region != latestRegion)
                        {
                            Assert.Fail("Expected {1}:{2}:{3} at {0} but received {4}:{5}:{6}", i, latestRegion.RegionName, latestRegion.RegionIteration, latestRegion.TopMost, region.RegionName, region.RegionIteration, region.TopMost);
                        }
                    }
                }
            }
        }

        [Test]
        public void OverlayingMemoryIterationTest()
        {
            MemoryManagementUnit.MappedMemoryRegion expectedRegion;

            for (int i = 0; i < testStreamsCount; i++)
            {
                if(i == 0)
                {
                    expectedRegion = this.topMostRegion;
                }
                else
                {
                    expectedRegion = this.regions.Last();
                }

                MemoryManagementUnit.MappedMemoryRegion region = this.mmu.FindMappedMemory((ushort)(this.testStreamsSize / 2));

                if (region == null)
                {
                    Assert.Fail("[{0}] Expected Region {1}:{2}:{3} not found", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost);
                }
                else
                {
                    if (region != expectedRegion)
                    {
                        Assert.Fail("[{0}] Expected Region {1}:{2}:{3} not received {4}:{5}:{6}", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost, region.RegionName, region.RegionIteration, region.TopMost);
                    }
                    else
                    {
                        this.mmu.RemoveMappedStream(region);

                        if(i != 0)
                        {
                            this.regions.Remove(region);
                        }
                    }
                }
            }
        }

        [Test]
        public void OverlayingMemoryReadTest()
        {
            MemoryManagementUnit.MappedMemoryRegion expectedRegion;
            byte expectedValue;

            for (int i = 0; i < testStreamsCount; i++)
            {
                if (i == 0)
                {
                    expectedRegion = this.topMostRegion;
                    expectedValue = 0x69;
                }
                else
                {
                    expectedRegion = this.regions.Last();
                    expectedValue = (byte)(testStreamsCount - i);
                }

                MemoryManagementUnit.MappedMemoryRegion region = this.mmu.FindMappedMemory((ushort)(this.testStreamsSize / 2));
                region.ExternalMemory.Position = (this.testStreamsSize / 2);

                if (region == null)
                {
                    Assert.Fail("[{0}] Expected Region {1}:{2}:{3} not found", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost);
                }
                else
                {
                    if (region != expectedRegion)
                    {
                        Assert.Fail("[{0}] Expected Region {1}:{2}:{3} not received {4}:{5}:{6}", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost, region.RegionName, region.RegionIteration, region.TopMost);
                    }
                    else
                    {
                        byte value = (byte)region.ExternalMemory.ReadByte();

                        if (value != expectedValue)
                        {
                            Assert.Fail("[{0}] Expected Region {1}:{2}:{3} did not read the right byte {4}:{5}", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost, expectedValue, value);
                        }
                        else
                        {
                            region.ExternalMemory.Position--;
                            region.ExternalMemory.WriteByte((byte)(0xFF - i));
                            region.ExternalMemory.Position--;
                            byte newValue = (byte)region.ExternalMemory.ReadByte();

                            if (newValue != (0xFF - i))
                            {
                                Assert.Fail("[{0}] Expected Region {1}:{2}:{3} did not read the right byte {4}:{5}", i, expectedRegion.RegionName, expectedRegion.RegionIteration, expectedRegion.TopMost, (0xFF - i), newValue);
                            }
                            else
                            {
                                this.mmu.RemoveMappedStream(region);

                                if (i != 0)
                                {
                                    this.regions.Remove(region);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
