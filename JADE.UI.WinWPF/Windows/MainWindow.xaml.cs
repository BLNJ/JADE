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
using System.Windows.Threading;

namespace JADE.UI.WinWPF.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JADE.Core.Device device;
        JADE.Core.ReadOnlyMemory.ROM rom;

        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send);

        long ticks = 0;
        DateTime dtLastUpdate;

        public MainWindow()
        {
            this.device = new Core.Device();
            this.device.DebugTick += Device_DebugTick;
            this.device.PPU.PictureDrawn += ppu_PictureDrawn;

            this.timer.Interval = new TimeSpan(16666660);
            this.timer.Tick += Timer_Tick;

            InitializeComponent();
            this.DataContext = this.device;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            drawEverything();
        }

        private void ppu_PictureDrawn(object sender, System.Drawing.Bitmap image)
        {
            ////TODO this is pulled directly from StackOverlow and just a temporary solution to tickle my inner need to actually see something happening
            //this.imgRender.Source = null;
            //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //{
            //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //    ms.Seek(0, System.IO.SeekOrigin.Begin);

            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.StreamSource = ms;
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.EndInit();

            //    this.imgRender.Source = bitmapImage;
            //}

            //DispatcherFrame frame = new DispatcherFrame();
            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            //{
            //    frame.Continue = false;
            //    return null;
            //}), null);

            //try
            //{
            //    Dispatcher.PushFrame(frame);
            //    //EDIT:
            //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            //                                  new Action(delegate { }));
            //}
            //catch
            //{ }

            drawEverything();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            //this.device.Reset(false);
            this.device.Reset(true);
            string romPath = "C:\\Gameboy_Dev\\totallyLegitTetris2.gb";
            rom = new Core.ReadOnlyMemory.ROM(device);
            rom.OpenFile(romPath);

            device.InsertROM(rom);

            this.dtLastUpdate = DateTime.Now;
            device.Start();
            this.ppuRegisters.DataContext = this.device.PPU.LCDPosition;
        }

        private void drawEverything()
        {
            if (this.device.PPU.LCDControlRegisters.LCDEnabled)
            {
                processFPS();

                //DateTime dtBefore = DateTime.Now;
                System.Drawing.Bitmap background = this.device.PPU.DrawBackground();
                //System.Drawing.Bitmap window = this.device.PPU.DrawWindow();
                System.Drawing.Bitmap tileData = this.device.PPU.EverythingTileTable.DrawTileTable(); //this.device.PPU.DrawTileData();
                //DateTime dtEnd = DateTime.Now;

                //var time = (dtEnd - dtBefore);
                //Console.WriteLine(time);
                //System.Diagnostics.Debug.WriteLine(time);

                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        bitmapToFrontendImage(imgBackground, background);
                        //bitmapToFrontendImage(imgWindow, window);
                        bitmapToFrontendImage(imgTileData, tileData);
                    });
                }
                catch { }
            }
        }

        private void processFPS()
        {
            TimeSpan span = (DateTime.Now - this.dtLastUpdate);

            if(span.TotalSeconds >= 1)
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Title = string.Format("JADE FPS:{0}", this.ticks.ToString());
                        ticks = 0;
                        dtLastUpdate = DateTime.Now;
                    });
                }
                catch { }
            }
            else
            {
                ticks++;
            }
        }

        private void Device_DebugTick(object sender)
        {
            //if(!this.timer.IsEnabled)
            //{
            //    this.timer.Start();
            //}

            //drawEverything();
        }

        private void bitmapToFrontendImage(Image imageControl, System.Drawing.Bitmap bitmap)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                //if(imageControl.Source == null)
                //{
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    imageControl.Source = bitmapImage;
                //}
                //else
                //{
                //    ((BitmapImage)imageControl.Source).StreamSource = ms;
                //}
            }
        }
    }
}
