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

            cbAudioDevices.SelectedValue = ApplicationSettings.CallbackDeviceID;


            //Popolo lista Devices Midi Input
            Dictionary<int, string> midiInDevices = new Dictionary<int, string>();
            midiInDevices.Add(-1, "");

            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
                midiInDevices.Add(i, MidiIn.DeviceInfo(i).ProductName);

            cbMidiInDevice.ItemsSource = midiInDevices.ToList();
            cbMidiInDevice.SelectedValuePath = "Key";
            cbMidiInDevice.DisplayMemberPath = "Value";

            if (ApplicationSettings.MidiInputDeviceID.HasValue)
                cbMidiInDevice.SelectedValue = ApplicationSettings.MidiInputDeviceID;
            else
                cbMidiInDevice.SelectedValue = -1;

            //Popolo lista Devices Midi Output e Repeater
            Dictionary<int, string> midiOutDevices = new Dictionary<int, string>();
            midiOutDevices.Add(-1, "");

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
                midiOutDevices.Add(i, MidiOut.DeviceInfo(i).ProductName);

            cbMidiOutDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutDevice.SelectedValuePath = "Key";
            cbMidiOutDevice.DisplayMemberPath = "Value";

            if (ApplicationSettings.MidiOutputDeviceID.HasValue)
                cbMidiOutDevice.SelectedValue = ApplicationSettings.MidiOutputDeviceID;
            else
                cbMidiOutDevice.SelectedValue = -1;

            cbMidiOutRepeaterDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutRepeaterDevice.SelectedValuePath = "Key";
            cbMidiOutRepeaterDevice.DisplayMemberPath = "Value";

            if(ApplicationSettings.MidiOutputRepeatedDeviceID.HasValue)
                cbMidiOutRepeaterDevice.SelectedValue = ApplicationSettings.MidiOutputRepeatedDeviceID;
            else
                cbMidiOutRepeaterDevice.SelectedValue = -1;
        }

        private void cbAudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.CallbackDeviceID = (int)cbAudioDevices.SelectedValue;
            

            if ((int)cbMidiInDevice.SelectedValue == -1)
                ApplicationSettings.MidiInputDeviceID = null;
            else
                ApplicationSettings.MidiInputDeviceID = (int)cbMidiInDevice.SelectedValue;


            if ((int)cbMidiOutDevice.SelectedValue == -1)
                ApplicationSettings.MidiOutputDeviceID = null;
            else
                ApplicationSettings.MidiOutputDeviceID = (int)cbMidiOutDevice.SelectedValue;


            if ((int)cbMidiOutRepeaterDevice.SelectedValue == -1)
                ApplicationSettings.MidiOutputRepeatedDeviceID = null;
            else
                ApplicationSettings.MidiOutputRepeatedDeviceID = (int)cbMidiOutRepeaterDevice.SelectedValue;


            ApplicationSettings.SaveConfig();

            //Reload Midi Devices
            GlobalStaticContext.DetachAllMidiDevices();
            if (ApplicationSettings.MidiInputDeviceID.HasValue && ApplicationSettings.MidiInputDeviceID.Value != -1)
                GlobalStaticContext.AttachMidiInDevice(ApplicationSettings.MidiInputDeviceID.Value);

            if (ApplicationSettings.MidiOutputDeviceID.HasValue && ApplicationSettings.MidiOutputDeviceID.Value != -1)
                GlobalStaticContext.AttachMidiOutDevice(ApplicationSettings.MidiOutputDeviceID.Value);

            if (ApplicationSettings.MidiOutputRepeatedDeviceID.HasValue && ApplicationSettings.MidiOutputRepeatedDeviceID.Value != -1)
                GlobalStaticContext.AttachMidiRepeatDevice(ApplicationSettings.MidiOutputRepeatedDeviceID.Value);

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
