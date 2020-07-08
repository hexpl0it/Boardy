using BoardyClassLibrary;
using Microsoft.Win32;
using NAudio.Midi;
using NAudio.Wave;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoardyWPF.Controls
{
    /// <summary>
    /// Logica di interazione per PadControl.xaml
    /// </summary>
    public partial class PadControl : UserControl
    {
        public PadControl()
        {
            InitializeComponent();

            int callbackdeviceID = -1;

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                if (WaveOut.GetCapabilities(i).ProductName == ApplicationSettings.CallbackDeviceID)
                    callbackdeviceID = i;
            }

            _player = new AudioPlayer("", callbackdeviceID);
            _player.OnSoundStateChange += _player_OnSoundStateChange;
            _player.OnAudioTrackChange += _player_OnAudioTrackChange;
            ChangeVolume(100, false);
        }

        public PadControl(string audiodevice, float volume, string audioFilePath, int midiNote, MidiController? volumeSilderContr)
        {
            InitializeComponent();

            int audiodeviceID = -1;
            int callbackdeviceID = -1;

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                if (WaveOut.GetCapabilities(i).ProductName == audiodevice)
                    audiodeviceID = i;
                if (WaveOut.GetCapabilities(i).ProductName == ApplicationSettings.CallbackDeviceID)
                    callbackdeviceID = i;
            }

            _player = new AudioPlayer("", callbackdeviceID);
            _player.OnSoundStateChange += _player_OnSoundStateChange;
            _player.OnAudioTrackChange += _player_OnAudioTrackChange;

            _player.ChangeAudioDevice(audiodeviceID);
            _player.ChangeAudioTrack(audioFilePath);
            ChangeVolume(Convert.ToInt32(volume * 100), false);

            this._pushButtonMidiNote = midiNote;
            if(this._pushButtonMidiNote != -1)
                GlobalStaticContext.RegisterPadWithMidiNote(this, midiNote);

            if (volumeSilderContr.HasValue)
            {
                this._volumeSliderMidiController = volumeSilderContr.Value;
                GlobalStaticContext.RegisterPadWithMidiController(this, volumeSilderContr.Value);
            }
        }

        private void _player_OnAudioTrackChange(string filePath)
        {
            lblFileName.Content = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }

        private AudioPlayer _player;
        internal string _audioPath
        {
            get
            {
                return _player._fileAudioPath;
            }

            set
            {
                _player.ChangeAudioTrack(value);
            }
        }
        internal string _audioDeviceID
        {
            get
            {
                return WaveOut.GetCapabilities(_player._idAudioDevice).ProductName;
            }

            set
            {
                int audioDevID = -1;

                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    if (WaveOut.GetCapabilities(i).ProductName == value)
                        audioDevID = i;
                }
                _player.ChangeAudioDevice(audioDevID);
            }
        }
        internal int _pushButtonMidiNote = -1;
        internal MidiController? _volumeSliderMidiController = null;
        internal float _volume
        {
            get
            {
                return _player.Volume;
            }

            set
            {
                _player.Volume = value;
            }
        }
        private void _player_OnSoundStateChange(AudioPlayer.SoundState state)
        {
            if (_pushButtonMidiNote > 0)
            {
                if (state == AudioPlayer.SoundState.Started)
                {
                    GlobalStaticContext.SendMidiMessage(MidiMessage.StartNote(_pushButtonMidiNote, 24, 1).RawData);
                    Application.Current.Dispatcher.Invoke(new Action(() => { btnPlaySound.Background = new SolidColorBrush(Color.FromRgb(123, 237, 154)); }));
                }
                else
                {
                    GlobalStaticContext.SendMidiMessage(MidiMessage.StopNote(_pushButtonMidiNote, 0, 1).RawData);
                    Application.Current.Dispatcher.Invoke(new Action(() => { btnPlaySound.Background = new SolidColorBrush(Color.FromRgb(201, 201, 201)); }));
                }
            }
        }

        public string OpenAudioFileSelector()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".wav";
            ofd.Filter = "WAV File (*.wav)|*.wav";
            ofd.Multiselect = false;

            bool? diagResult = ofd.ShowDialog();

            if (diagResult.HasValue && diagResult.Value)
            {
                return ofd.FileName;
            }
            else
                return "";
        }

        internal void ChangeVolume(int controllerValue, bool fromMidi)
        {
            int newVal = controllerValue;
            
            if(fromMidi)
                newVal =  Convert.ToInt32(((controllerValue + 1F) * 100F / 128F) - 1F);

            _player.Volume = Convert.ToSingle(newVal) / 100;


            Application.Current.Dispatcher.Invoke(new Action(() => { VolumeMeter.Value = newVal; }));
        }

        public void PlaySound()
        {
            _player.PlaySound();
        }

        public void ChangeAudioDevice(int audioDev)
        {
            _player.ChangeAudioDevice(audioDev);
        }
        public void ChangeAudioTrack(string audioPathFile)
        {
            _player.ChangeAudioTrack(audioPathFile);
        }

        private void btnPlaySound_Click(object sender, RoutedEventArgs e)
        {
            PlaySound();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            //(new MacroSoundSettingsForm(this)).ShowDialog();
        }
        private void LoadSampleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filePath = OpenAudioFileSelector();
            _player.ChangeAudioTrack(filePath);
        }

        private void btnPlaySound_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            AudioDevicesSubMenu.Items.Clear();

            Dictionary<int, string> outDevices = new Dictionary<int, string>();

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                outDevices.Add(i, WaveOut.GetCapabilities(i).ProductName);
                MenuItem mi = new MenuItem();
                mi.Header = i.ToString() + " - " + WaveOut.GetCapabilities(i).ProductName;
                mi.Uid = "admi-" + i.ToString();
                mi.Click += AudioDevicesSubMenuItem_Click;

                if (_audioDeviceID == WaveOut.GetCapabilities(i).ProductName)
                    mi.IsChecked = true;

                AudioDevicesSubMenu.Items.Add(mi);
            }
        }

        private void AudioDevicesSubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem x = (MenuItem)sender;
            _player.ChangeAudioDevice(Convert.ToInt32(x.Uid.Replace("admi-", "")));
        }

        private void LearnMidiButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalStaticContext.EnterLearnMode(MidiPushButtonLearnCallback);
        }

        private void MidiPushButtonLearnCallback(object sender, MidiInMessageEventArgs e)
        {
            NoteOnEvent evento = (NoteOnEvent)e.MidiEvent;
            _pushButtonMidiNote = evento.NoteNumber;
            GlobalStaticContext.RegisterPadWithMidiNote(this, _pushButtonMidiNote);
            GlobalStaticContext.ExitLearnMode();
        }

        private void MidiVolumeSliderLearnCallback(object sender, MidiInMessageEventArgs e)
        {
            ControlChangeEvent evento = (ControlChangeEvent)e.MidiEvent;
            _volumeSliderMidiController = evento.Controller;
            GlobalStaticContext.RegisterPadWithMidiController(this, _volumeSliderMidiController.Value);
            GlobalStaticContext.ExitLearnMode();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _player.Volume = Convert.ToSingle(e.NewValue / 100);
        }

        private void LearnMidiSlider_Click(object sender, RoutedEventArgs e)
        {
            GlobalStaticContext.EnterLearnMode(MidiVolumeSliderLearnCallback);
        }

        private void RemovePad_Click(object sender, RoutedEventArgs e)
        {
            var parent = (WrapPanel)this.Parent;
            parent.Children.Remove(this);
            GlobalStaticContext.RemovePad(this);
            //MainWrapPanel.Children.Add(pc);
        }
    }
}
