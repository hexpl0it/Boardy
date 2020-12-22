using BoardyWPF.Controls;
using CSCore.CoreAudioAPI;
using Melanchall.DryWetMidi.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        StartupHelper.StartupManager mngr = new StartupHelper.StartupManager("BoardyWPF", StartupHelper.RegistrationScope.Local);
        public MainSettingsWindow()
        {
            InitializeComponent();

            Dictionary<string, string> outDevices = new Dictionary<string, string>();

            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        outDevices.Add(device.DevicePath, device.FriendlyName);
                    }
                }
            }


            cbAudioDevices.ItemsSource = outDevices.ToList();
            cbAudioDevices.SelectedValuePath = "Key";
            cbAudioDevices.DisplayMemberPath = "Value";

            try
            {
                cbAudioDevices.SelectedValue = ApplicationSettings.CallbackDeviceID;
            }
            catch { }


            //Popolo lista Devices Midi Input
            Dictionary<string, string> midiInDevices = new Dictionary<string, string>();
            midiInDevices.Add("", "");

            foreach (var midiInDev in Melanchall.DryWetMidi.Devices.InputDevice.GetAll())
            {
                midiInDevices.Add(midiInDev.Name, midiInDev.Name);
            }

            //cbMidiInDevice.ItemsSource = Melanchall.DryWetMidi.Devices.InputDevice.GetAll();
            cbMidiInDevice.ItemsSource = midiInDevices;
            cbMidiInDevice.SelectedValuePath = "Key";
            cbMidiInDevice.DisplayMemberPath = "Value";

            var devIn = ApplicationSettings.MidiInputDeviceID != null ? InputDevice.GetByName(ApplicationSettings.MidiInputDeviceID) : null;

            cbMidiInDevice.SelectedValue = devIn != null ? devIn.Name : "";

            //Popolo lista Devices Midi Output e Repeater
            Dictionary<string, string> midiOutDevices = new Dictionary<string, string>();
            midiOutDevices.Add("", "");

            foreach (var midiOutDev in Melanchall.DryWetMidi.Devices.OutputDevice.GetAll())
            {
                midiOutDevices.Add(midiOutDev.Name, midiOutDev.Name);
            }


            cbMidiOutDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutDevice.SelectedValuePath = "Key";
            cbMidiOutDevice.DisplayMemberPath = "Value";

            var devOut = ApplicationSettings.MidiOutputDeviceID != null ? InputDevice.GetByName(ApplicationSettings.MidiOutputDeviceID) : null;

            cbMidiOutDevice.SelectedValue = devOut != null ? devOut.Name : "";

            cbMidiOutRepeaterDevice.ItemsSource = midiOutDevices.ToList();
            cbMidiOutRepeaterDevice.SelectedValuePath = "Key";
            cbMidiOutRepeaterDevice.DisplayMemberPath = "Value";

            var devRepeat = ApplicationSettings.MidiOutputRepeatedDeviceID != null ? InputDevice.GetByName(ApplicationSettings.MidiOutputRepeatedDeviceID) : null;

            cbMidiOutRepeaterDevice.SelectedValue = devRepeat != null ? devRepeat.Name : "";

            ckbStartup.IsChecked = mngr.IsRegistered;
        }

        private void cbAudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.CallbackDeviceID = (string)cbAudioDevices.SelectedValue;


            ApplicationSettings.MidiInputDeviceID = (string)cbMidiInDevice.SelectedValue;

            ApplicationSettings.MidiOutputDeviceID = (string)cbMidiOutDevice.SelectedValue;

            ApplicationSettings.MidiOutputRepeatedDeviceID = (string)cbMidiOutRepeaterDevice.SelectedValue;


            ApplicationSettings.SaveConfig();

            //Reload Midi Devices
            GlobalStaticContext.DetachAllMidiDevices();
            ApplicationSettings.MidiInputDeviceID = (string)cbMidiInDevice.SelectedValue;

            ApplicationSettings.MidiOutputDeviceID = (string)cbMidiOutDevice.SelectedValue;

            ApplicationSettings.MidiOutputRepeatedDeviceID = (string)cbMidiOutRepeaterDevice.SelectedValue;

            GlobalStaticContext.AttachMidiInDevice(ApplicationSettings.MidiInputDeviceID);
            GlobalStaticContext.AttachMidiOutDevice(ApplicationSettings.MidiOutputDeviceID);
            GlobalStaticContext.AttachMidiRepeatDevice(ApplicationSettings.MidiOutputRepeatedDeviceID);

            this.Close();
        }

        private void cbMidiInDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)cbMidiInDevice.SelectedValue != "")
            {
                cbMidiOutDevice.SelectedValue = (string)cbMidiInDevice.SelectedValue;
            }
        }

        private void ckbStartup_Checked(object sender, RoutedEventArgs e)
        {
            mngr.Register();
        }

        private void ckbStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            mngr.Unregister();
        }
    }
}
