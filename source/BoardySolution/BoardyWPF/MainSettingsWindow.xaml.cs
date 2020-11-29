using BoardyWPF.Controls;
using NAudio.Midi;
using NAudio.Wave;
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

namespace BoardyWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainSettingsWindow : Window
    {
        public MainSettingsWindow()
        {
            InitializeComponent();

            Dictionary<int, string> outDevices = new Dictionary<int, string>();

            for (int i = 0; i < WaveOut.DeviceCount; i++)
                outDevices.Add(i, WaveOut.GetCapabilities(i).ProductName);

            cbAudioDevices.ItemsSource = outDevices.ToList();
            cbAudioDevices.SelectedValuePath = "Key";
            cbAudioDevices.DisplayMemberPath = "Value";

            try
            {
                cbAudioDevices.SelectedValue = ApplicationSettings.CallbackDeviceID;
            }
            catch { }


            //Popolo lista Devices Midi Input
            Dictionary<int, string> midiInDevices = new Dictionary<int, string>();
            midiInDevices.Add(-1, "");

            int selectedMidiINPUTDevId = -1;

            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                midiInDevices.Add(i, MidiIn.DeviceInfo(i).ProductName);
                if (ApplicationSettings.MidiInputDeviceID != null && ApplicationSettings.MidiInputDeviceID == MidiIn.DeviceInfo(i).ProductName)
                    selectedMidiINPUTDevId = i;
            }

            cbMidiInDevice.ItemsSource = midiInDevices.ToList();
            cbMidiInDevice.SelectedValuePath = "Key";
            cbMidiInDevice.DisplayMemberPath = "Value";

            cbMidiInDevice.SelectedValue = selectedMidiINPUTDevId;

            //Popolo lista Devices Midi Output e Repeater
            Dictionary<int, string> midiOutDevices = new Dictionary<int, string>();
            midiOutDevices.Add(-1, "");

            int selectedMidiOUTPUTDevId = -1;
            int selectedMidiREPEATEDDevId = -1;

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                midiOutDevices.Add(i, MidiOut.DeviceInfo(i).ProductName);
                if (ApplicationSettings.MidiInputDeviceID != null && ApplicationSettings.MidiInputDeviceID == MidiIn.DeviceInfo(i).ProductName)
                    selectedMidiOUTPUTDevId = i;
                if (ApplicationSettings.MidiOutputRepeatedDeviceID != null && ApplicationSettings.MidiOutputRepeatedDeviceID == MidiIn.DeviceInfo(i).ProductName)
                    selectedMidiREPEATEDDevId = i;
            }


            cbMidiOutDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutDevice.SelectedValuePath = "Key";
            cbMidiOutDevice.DisplayMemberPath = "Value";

            cbMidiOutDevice.SelectedValue = selectedMidiOUTPUTDevId;

            cbMidiOutRepeaterDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutRepeaterDevice.SelectedValuePath = "Key";
            cbMidiOutRepeaterDevice.DisplayMemberPath = "Value";

            cbMidiOutRepeaterDevice.SelectedValue = selectedMidiREPEATEDDevId;
        }

        private void cbAudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.CallbackDeviceID = ((KeyValuePair<int, string>)cbAudioDevices.SelectedItem).Value;


            if (((KeyValuePair<int, string>)cbMidiInDevice.SelectedItem).Key == -1)
                ApplicationSettings.MidiInputDeviceID = null;
            else
                ApplicationSettings.MidiInputDeviceID = ((KeyValuePair<int, string>)cbMidiInDevice.SelectedItem).Value;


            if (((KeyValuePair<int, string>)cbMidiOutDevice.SelectedItem).Key == -1)
                ApplicationSettings.MidiOutputDeviceID = null;
            else
                ApplicationSettings.MidiOutputDeviceID = ((KeyValuePair<int, string>)cbMidiOutDevice.SelectedItem).Value;


            if (((KeyValuePair<int, string>)cbMidiOutRepeaterDevice.SelectedItem).Key == -1)
                ApplicationSettings.MidiOutputRepeatedDeviceID = null;
            else
                ApplicationSettings.MidiOutputRepeatedDeviceID = ((KeyValuePair<int, string>)cbMidiOutRepeaterDevice.SelectedItem).Value;


            ApplicationSettings.SaveConfig();

            //Reload Midi Devices
            GlobalStaticContext.DetachAllMidiDevices();
            if (((KeyValuePair<int, string>)cbMidiInDevice.SelectedItem).Key != -1)
                GlobalStaticContext.AttachMidiInDevice(((KeyValuePair<int, string>)cbMidiInDevice.SelectedItem).Key);

            if (((KeyValuePair<int, string>)cbMidiOutDevice.SelectedItem).Key != -1)
                GlobalStaticContext.AttachMidiOutDevice(((KeyValuePair<int, string>)cbMidiOutDevice.SelectedItem).Key);

            if (((KeyValuePair<int, string>)cbMidiOutRepeaterDevice.SelectedItem).Key != -1)
                GlobalStaticContext.AttachMidiRepeatDevice(((KeyValuePair<int, string>)cbMidiOutRepeaterDevice.SelectedItem).Key);

            this.Close();
        }

        private void cbMidiInDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((int)cbMidiInDevice.SelectedValue != -1)
            {
                //Cerco stesso midi device per output
                for (int i = 0; i < MidiOut.NumberOfDevices; i++)
                {
                    if (MidiOut.DeviceInfo(i).ProductName == MidiIn.DeviceInfo((int)cbMidiInDevice.SelectedValue).ProductName)
                    {
                        cbMidiOutDevice.SelectedValue = i;
                    }
                }
            }
            else
                cbMidiOutDevice.SelectedValue = cbMidiOutRepeaterDevice.SelectedValue = -1;
        }
    }
}
