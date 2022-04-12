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

namespace JADE.UX.WPF.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JADE.Core.Device device;
        GameBoy_Emulator.Core.ROM rom;

        public MainWindow()
        {
            device = new Core.Device();
            device.PPU.PictureDrawn += this.GPU_PictureDrawn;

            InitializeComponent();
            this.DataContext = this.device;
        }

        private void GPU_PictureDrawn(object sender, System.Drawing.Bitmap image)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();


                    imgRender.Source = bitmapImage;

                }
            }));
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            this.rom = new Core.ROM();
            rom.FromFile(@"D:\Development\GameBoy_Emulator\Files\Tetris (World).gb");
            rom.Read();

            device.InsertROM(rom);
            device.Start();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false
            };

            bool? result = ofd.ShowDialog();
            if (result != null && result == true)
            {
                var fileStream = ofd.OpenFile();

                GameBoy_Emulator.Core.ROM rom = new Core.ROM();
                rom.FromStream(fileStream);
                rom.Read();
                device.InsertROM(rom);
            }
        }
    }
}
