using System;

namespace JADE.UI.WinConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string romPath = "C:\\Gameboy_Dev\\totallyLegitTetris.gb";

            JADE.Core.Device device = new Core.Device();
            device.Reset();

            JADE.Core.ReadOnlyMemory.ROM rom = new Core.ReadOnlyMemory.ROM(device);
            rom.OpenFile(romPath);

            device.InsertROM(rom);
            device.Start();
        }
    }
}
