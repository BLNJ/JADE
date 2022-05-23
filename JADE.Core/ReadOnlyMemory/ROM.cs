using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JADE.Core.ReadOnlyMemory
{
    public class ROM
    {
        Device device;
        public ROMHeader Header;

        public Stream Stream
        {
            get;
            private set;
        }

        public ROM(Device device)
        {
            this.device = device;
        }

        public void Read()
        {
            this.Header = new ROMHeader(this);
            Header.Read();
        }

        public void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }
            else
            {
                FileStream fs = File.OpenRead(filePath);
                this.Open(fs);
            }
        }

        public void Open(Stream stream)
        {
            this.Stream = stream;
        }

        public class ROMHeader
        {
            public byte[] EntryPoint;
            public byte[] NintendoLogo;
            public byte[] TitleRaw;
            public byte[] LicenseeNew;
            public SGBFlag SGB_Flag;
            public CartridgeType Cartridge_Type;

            private byte romsize;
            private byte ramsize;

            public byte Destination;
            public byte LicenseeOld;
            public byte ROM_Version;
            public byte Checksum;
            public byte[] Checksum_Global;

            public byte ROM_Banks
            {
                get
                {
                    switch (this.romsize)
                    {
                        case 0x00:
                        case 0x01:
                        case 0x02:
                        case 0x03:
                        case 0x04:
                        case 0x05:
                        case 0x06:
                        case 0x07:
                            return (byte)Math.Pow(2, this.romsize);
                        case 0x52:
                            return 72;
                        case 0x53:
                            return 80;
                        case 0x54:
                            return 96;
                        default:
                            throw new Exception("Unknown ROM_Size: " + this.romsize.ToString());
                    }
                }
            }

            ROM rom;
            BinaryReader br;

            public ROMHeader(ROM rom)
            {
                this.rom = rom;
            }

            public void Read()
            {
                //ugly
                this.br = new BinaryReader(this.rom.Stream);
                br.BaseStream.Position = 0x100;

                this.EntryPoint = br.ReadBytes(0x4);
                this.NintendoLogo = br.ReadBytes(0x30);
                this.TitleRaw = br.ReadBytes(0x10);
                this.LicenseeNew = br.ReadBytes(0x2);
                this.SGB_Flag = (SGBFlag)br.ReadByte();
                this.Cartridge_Type = (CartridgeType)br.ReadByte();
                this.romsize = br.ReadByte();
                this.ramsize = br.ReadByte();
                this.Destination = br.ReadByte();
                this.LicenseeOld = br.ReadByte();
                this.ROM_Version = br.ReadByte();
                this.Checksum = br.ReadByte();
                this.Checksum_Global = br.ReadBytes(0x2);
            }

            public enum SGBFlag:byte
            {
                None = 0x0,
                Supported = 0x03
            }
            public enum CartridgeType:byte
            {
                ROM_Only = 0x00,
                MBC1 = 0x01,
                MBC1_RAM = 0x02,
                MBC1_RAM_Battery = 0x03,
                //
                MBC2 = 0x05,
                MBC2_Battery = 0x06,
                //
                ROM_RAM = 0x08,
                ROM_RAM_Battery = 0x09,
                //
                MMM01 = 0x0B,
                MMM01_RAM = 0x0C,
                MMM01_RAM_Battery = 0x0D,
                //
                MBC3_Timer_Battery = 0x0F,
                MBC3_Timer_RAM_Battery = 0x10,
                MBC3 = 0x11,
                MBC3_RAM = 0x12,
                MBC3_RAM_Battery = 0x13,
                //
                MBC4 = 0x15,
                MBC4_RAM = 0x16,
                MBC4_RAM_Battery = 0x17,
                //
                MBC5 = 0x19,
                MBC5_RAM = 0x1A,
                MBC5_RAM_Battery = 0x1B,
                MBC5_Rumble = 0x1C,
                MBC5_Rumble_RAM = 0x1D,
                MBC5_Rumble_RAM_Battery = 0x1E,
                //
                //
                PocketCamera = 0xFC,
                BandaiTAMA5 = 0xFD,
                HuC3 = 0xFE,
                HuC1_RAM_Battery = 0xFF
            }
        }

    }
}
