using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardyClassLibrary
{
    public class AudioPlayer
    {
        public int _idAudioDevice { get; private set; }
        private float _volume;
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                if(afr != null)
                    afr.Volume = value;
            }
        }
        public string _fileAudioPath { get; private set; }
        private WaveOut player = new WaveOut();
        private AudioFileReader afr;

        public enum SoundState
        {
            Started, Stopped
        }
        public delegate void SoundStateChangeHandler(SoundState state);
        public event SoundStateChangeHandler OnSoundStateChange;
        public delegate void AudioTrackChangeHandler(string filePath);
        public event AudioTrackChangeHandler OnAudioTrackChange;
        public delegate void LoopbackDeviceChangeHandler(int deviceId);
        public event LoopbackDeviceChangeHandler OnLoopbackDeviceChange;
        public AudioPlayer(string fileAudioPath, int audioDeviceID = 0)
        {
            _idAudioDevice = audioDeviceID;
            _fileAudioPath = fileAudioPath;
        }

        public void PlaySound()
        {
            player.PlaybackStopped -= Player_PlaybackStopped;
            player.Stop();
            player = new WaveOut();
            player.PlaybackStopped += Player_PlaybackStopped;
            player.DeviceNumber = _idAudioDevice;
            afr = new AudioFileReader(_fileAudioPath);
            afr.Volume = this._volume;
            player.Init(afr);
            player.Play();

            OnSoundStateChange?.Invoke(SoundState.Started);
        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            OnSoundStateChange?.Invoke(SoundState.Stopped);
        }

        public void ChangeAudioDevice(int idAudioDevice)
        {
            _idAudioDevice = idAudioDevice;
            OnLoopbackDeviceChange?.Invoke(idAudioDevice);
        }
        public void ChangeAudioTrack(string audioFilePath)
        {
            _fileAudioPath = audioFilePath;
            OnAudioTrackChange?.Invoke(_fileAudioPath);
        }
    }
}
