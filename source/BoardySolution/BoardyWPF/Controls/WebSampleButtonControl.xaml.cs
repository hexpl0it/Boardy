using BoardyClassLibrary.WebIntegration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoardyWPF.Controls
{
    /// <summary>
    /// Logica di interazione per WebSampleButtonControl.xaml
    /// </summary>
    public partial class WebSampleButtonControl : UserControl
    {
        public delegate void SampleDownlaodHandler(string pathFile);
        public event SampleDownlaodHandler OnFileDownloaded;

        //WaveOut waveOut = new WaveOut();
        IBoardyWebSample _sample;
        Stream _cache = null;
        Thread t;
        public WebSampleButtonControl(IBoardyWebSample sample)
        {
            InitializeComponent();
            _sample = sample;
            BtnPlaySound.Content = _sample.Description;
        }

        private void BtnPlaySound_Click(object sender, RoutedEventArgs e)
        {
            if (_cache == null)
            {
                _cache = new MemoryStream();
                _sample.Download().CopyTo(_cache);
            }

            t = new Thread(() =>
            {
                //PlayMp3FromStream(_cache);
            });
            t.Start();
        }

        private void BtnDownloadSound_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\tmp\" + _sample.Name;
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            _sample.Download().CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Dispose();

            OnFileDownloaded?.Invoke(filePath);
        }

        //public void PlayMp3FromStream(Stream orig)
        //{
        //    using (Stream ms = new MemoryStream())
        //    {
        //        orig.Position = 0;
        //        orig.CopyTo(ms);


        //        ms.Position = 0;
        //        using (WaveStream blockAlignedStream =
        //            new BlockAlignReductionStream(
        //                WaveFormatConversionStream.CreatePcmStream(
        //                    new Mp3FileReader(ms))))
        //        {
        //            waveOut.Stop();
        //            waveOut = new WaveOut();

        //            int callbackdeviceID = -1;

        //            for (int i = 0; i < WaveOut.DeviceCount; i++)
        //            {
        //                if (WaveOut.GetCapabilities(i).ProductName == ApplicationSettings.CallbackDeviceID)
        //                    callbackdeviceID = i;
        //            }


        //            waveOut.DeviceNumber = callbackdeviceID;
        //            waveOut.Init(blockAlignedStream);
        //            waveOut.Play();
        //            while (waveOut.PlaybackState == PlaybackState.Playing)
        //            {
        //                System.Threading.Thread.Sleep(10);
        //            }

        //        }
        //    }
        //}
    }
}
