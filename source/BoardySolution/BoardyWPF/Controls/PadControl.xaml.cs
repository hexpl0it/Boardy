using BoardyClassLibrary;
using CSCore.CoreAudioAPI;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Devices;
using Microsoft.Win32;
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

            _player = new AudioPlayer();
            _player.OnSoundStateChange += _player_OnSoundStateChange;
            _player.OnAudioTrackChange += _player_OnAudioTrackChange;
            ChangeVolume(100, false);
        }

        public PadControl(string audiodevice, float volume, string audioFilePath, int midiNote, SevenBitNumber volumeSilderContr)
        {
            InitializeComponent();

            MMDevice mmAudioDev = new MMDeviceEnumerator().GetDeviceFromPath(audiodevice);


            _player = new AudioPlayer();
            _player.OnSoundStateChange += _player_OnSoundStateChange;
            _player.OnAudioTrackChange += _player_OnAudioTrackChange;

            _player.ChangeAudioDevice(mmAudioDev);
            _player.ChangeAudioTrack(audioFilePath);
            ChangeVolume(Convert.ToInt32(volume), false);

            this._pushButtonMidiNote = midiNote;
            if (this._pushButtonMidiNote != -1)
                GlobalStaticContext.RegisterPadWithMidiNote(this, midiNote);

            if (volumeSilderContr > 0)
            {
                this._volumeSliderMidiController = volumeSilderContr;
                GlobalStaticContext.RegisterPadWithMidiController(this, volumeSilderContr);
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
                return _player.AudioFilepath;
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
                return _player.AudioDevice.DevicePath;
            }
        }
        internal int _pushButtonMidiNote = -1;
        internal SevenBitNumber _volumeSliderMidiController;
        internal int _volume
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

            if (state == AudioPlayer.SoundState.Started)
            {
                if (_pushButtonMidiNote > 0)
                {
                    GlobalStaticContext.SendOutputFeedbackMessage(new Melanchall.DryWetMidi.Core.NoteOnEvent((SevenBitNumber)_pushButtonMidiNote, SevenBitNumber.MaxValue));
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { btnPlaySound.Background = new SolidColorBrush(Color.FromRgb(123, 237, 154)); }));
            }
            else
            {
                if (_pushButtonMidiNote > 0)
                {
                    GlobalStaticContext.SendOutputFeedbackMessage(new Melanchall.DryWetMidi.Core.NoteOffEvent((SevenBitNumber)_pushButtonMidiNote, SevenBitNumber.MaxValue));
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { btnPlaySound.Background = new SolidColorBrush(Color.FromRgb(201, 201, 201)); }));
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

            if (fromMidi)
                newVal = Convert.ToInt32(((controllerValue + 1F) * 100F / 128F) - 1F);

            _player.Volume = newVal;


            Application.Current.Dispatcher.Invoke(new Action(() => { VolumeMeter.Value = newVal; }));
        }

        public void PlaySound()
        {
            _player.Play();
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

            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (
                    var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        MenuItem mi = new MenuItem();
                        mi.Header = device.FriendlyName;
                        mi.Uid = device.DevicePath;
                        mi.Click += AudioDevicesSubMenuItem_Click;

                        if (_audioDeviceID == device.DevicePath)
                            mi.IsChecked = true;

                        AudioDevicesSubMenu.Items.Add(mi);
                    }
                }
            }
        }

        private void AudioDevicesSubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem x = (MenuItem)sender;
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                _player.ChangeAudioDevice(mmdeviceEnumerator.GetDeviceFromPath(x.Uid));
            }
        }

        private void LearnMidiButton_Click(object sender, RoutedEventArgs e)
        {
            lblLearning.Visibility = Visibility.Visible;
            GlobalStaticContext.EnterLearnMode(MidiPushButtonLearnCallback);
        }

        private void MidiPushButtonLearnCallback(object sender, MidiEventReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lblLearning.Visibility = Visibility.Collapsed;
            });
            Melanchall.DryWetMidi.Core.NoteOnEvent evento = (Melanchall.DryWetMidi.Core.NoteOnEvent)e.Event;
            _pushButtonMidiNote = evento.NoteNumber;
            GlobalStaticContext.RegisterPadWithMidiNote(this, _pushButtonMidiNote);
            GlobalStaticContext.ExitLearnMode();
        }

        private void MidiVolumeSliderLearnCallback(object sender, MidiEventReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lblLearning.Visibility = Visibility.Collapsed;
            });
            Melanchall.DryWetMidi.Core.ControlChangeEvent evento = (Melanchall.DryWetMidi.Core.ControlChangeEvent)e.Event;
            _volumeSliderMidiController = evento.ControlNumber;
            GlobalStaticContext.RegisterPadWithMidiController(this, _volumeSliderMidiController);
            GlobalStaticContext.ExitLearnMode();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _player.Volume = Convert.ToInt32(e.NewValue);
        }

        private void LearnMidiSlider_Click(object sender, RoutedEventArgs e)
        {
            lblLearning.Visibility = Visibility.Visible;
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
