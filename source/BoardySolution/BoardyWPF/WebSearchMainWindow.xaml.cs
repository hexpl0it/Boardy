using BoardyClassLibrary.WebIntegration.Myinstants;
using BoardyWPF.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BoardyWPF
{
    /// <summary>
    /// Logica di interazione per WebSearchMainWindow.xaml
    /// </summary>
    public partial class WebSearchMainWindow : Window
    {
        public delegate void SampleDownlaodHandler(string pathFile);
        public event SampleDownlaodHandler OnFileDownloaded;
        public WebSearchMainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyInstantsWebSite ws = new MyInstantsWebSite();
            var t = ws.Home();
            t.Wait();
            foreach (var f in t.Result)
            {
                var control = new WebSampleButtonControl(f);
                control.OnFileDownloaded += Control_OnFileDownloaded;
                MainWrapPanel.Children.Add(control);
            }
        }

        private void Control_OnFileDownloaded(string pathFile)
        {
            OnFileDownloaded?.Invoke(pathFile);
        }

        private void BtnCerca_Click(object sender, RoutedEventArgs e)
        {
            MyInstantsWebSite ws = new MyInstantsWebSite();
            var t = ws.SearchSample(TbxCerca.Text);
            t.Wait();
            MainWrapPanel.Children.Clear();
            foreach (var f in t.Result)
            {
                var control = new WebSampleButtonControl(f);
                control.OnFileDownloaded += Control_OnFileDownloaded;
                MainWrapPanel.Children.Add(control);
            }
        }
    }
}
