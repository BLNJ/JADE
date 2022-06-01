using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JADE.UI.WinWPF.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JADE.Core.Device device;
        JADE.Core.ReadOnlyMemory.ROM rom;

        public MainWindow()
        {
            this.device = new Core.Device();
            this.device.PPU.PictureDrawn += ppu_PictureDrawn;

            InitializeComponent();
            this.DataContext = this.device;
        }

        private void ppu_PictureDrawn(object sender, System.Drawing.Bitmap image)
        {
            throw new NotImplementedException();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            string romPath = "C:\\Gameboy_Dev\\totallyLegitTetris.gb";
            rom = new Core.ReadOnlyMemory.ROM(device);
            rom.OpenFile(romPath);

            device.InsertROM(rom);
            device.Start();
        }
    }
}
