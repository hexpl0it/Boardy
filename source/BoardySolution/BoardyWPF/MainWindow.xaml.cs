using BoardyWPF.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Midi;
using System.IO;
using NAudio.Wave;
using System.Net;
using System.Threading;
using BoardyClassLibrary.WebIntegration.Myinstants;

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
            this.Visibility = Visibility.Hidden;

            MainNotifyIcon.Icon = new System.Drawing.Icon("boardy_logo.ico");

            foreach(var pcs in ApplicationSettings.PadControls)
            {
                PadControl pc = new PadControl(pcs.AudioDeviceID, pcs.Volume, pcs.AudioFilePath, pcs.MidiNoteMap, pcs.VolumeSliderControllerMap);
                GlobalStaticContext.AddPad(pc);
                MainWrapPanel.Children.Add(pc);
            }

            int selectedMidiINPUTDevId = -1;

            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                if (ApplicationSettings.MidiInputDeviceID == MidiIn.DeviceInfo(i).ProductName)
                    selectedMidiINPUTDevId = i;
            }

            int selectedMidiOUTPUTDevId = -1;
            int selectedMidiREPEATEDDevId = -1;

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (ApplicationSettings.MidiOutputDeviceID == MidiOut.DeviceInfo(i).ProductName)
                    selectedMidiOUTPUTDevId = i;
                if (ApplicationSettings.MidiOutputRepeatedDeviceID == MidiOut.DeviceInfo(i).ProductName)
                    selectedMidiREPEATEDDevId = i;
            }


            if (ApplicationSettings.MidiInputDeviceID != null && ApplicationSettings.MidiInputDeviceID != "")
                GlobalStaticContext.AttachMidiInDevice(selectedMidiINPUTDevId);

            if (ApplicationSettings.MidiOutputDeviceID != null && ApplicationSettings.MidiOutputDeviceID != "")
                GlobalStaticContext.AttachMidiOutDevice(selectedMidiOUTPUTDevId);

            if (ApplicationSettings.MidiOutputRepeatedDeviceID != null && ApplicationSettings.MidiOutputRepeatedDeviceID != "")
                GlobalStaticContext.AttachMidiRepeatDevice(selectedMidiREPEATEDDevId);
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
