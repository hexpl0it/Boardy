using BoardyWPF.Controls;
using Melanchall.DryWetMidi.Common;
using System.Windows;

namespace BoardyWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Visibility = Visibility.Hidden;

            MainNotifyIcon.Icon = new System.Drawing.Icon("boardy_logo.ico");

            foreach (var pcs in ApplicationSettings.PadControls)
            {
                PadControl pc = new PadControl(pcs.AudioDeviceID, pcs.Volume, pcs.AudioFilePath, pcs.MidiNoteMap, (SevenBitNumber)pcs.VolumeSliderControllerMap);
                GlobalStaticContext.AddPad(pc);
                MainWrapPanel.Children.Add(pc);
            }

            if (ApplicationSettings.MidiInputDeviceID != null)
                GlobalStaticContext.AttachMidiInDevice(ApplicationSettings.MidiInputDeviceID);
            if (ApplicationSettings.MidiOutputDeviceID != null)
                GlobalStaticContext.AttachMidiOutDevice(ApplicationSettings.MidiOutputDeviceID);
            if (ApplicationSettings.MidiOutputRepeatedDeviceID != null)
                GlobalStaticContext.AttachMidiRepeatDevice(ApplicationSettings.MidiOutputRepeatedDeviceID);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsWindow seWindow = new MainSettingsWindow();
            seWindow.Show();
        }

        private void NewPadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PadControl pc = new PadControl();
            GlobalStaticContext.AddPad(pc);
            MainWrapPanel.Children.Add(pc);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void TIOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void TICloseAppMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebSearchMainWindow wsw = new WebSearchMainWindow();
            wsw.OnFileDownloaded += Wsw_OnFileDownloaded;
            wsw.ShowDialog();

            //Thread t = new Thread(() =>
            //{
            //    PlayMp3FromUrl(@"https://www.myinstants.com/media/sounds/erro.mp3");
            //});
            //t.Start();
        }

        private void Wsw_OnFileDownloaded(string pathFile)
        {
            PadControl pc = new PadControl();
            pc.ChangeAudioTrack(pathFile);
            GlobalStaticContext.AddPad(pc);
            MainWrapPanel.Children.Add(pc);
        }
    }
}
