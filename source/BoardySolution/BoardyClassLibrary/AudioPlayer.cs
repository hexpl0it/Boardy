using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams;
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
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;
        public string AudioFilepath { get; set; }
        public MMDevice AudioDevice { get; set; }

        public enum SoundState
        {
            Started, Stopped, Paused
        }
        public delegate void SoundStateChangeHandler(SoundState state);
        public event SoundStateChangeHandler OnSoundStateChange;

        public delegate void AudioTrackChangeHandler(string filePath);
        public event AudioTrackChangeHandler OnAudioTrackChange;

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                    return _soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
            }
            set
            {
                if (_waveSource != null)
                    _waveSource.SetPosition(value);
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                    return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                }
            }
        }

        public AudioPlayer()
        {
            _waveSource = new SineGenerator().ToWaveSource();
            MMDevice device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            AudioDevice = device;
            _soundOut = new WasapiOut() { Latency = 100, Device = AudioDevice };
            _soundOut.Initialize(_waveSource);
            _soundOut.Stopped += _soundOut_Stopped;
        }

        public AudioPlayer(string fileAudioPath, MMDevice device = null)
        {
            AudioFilepath = fileAudioPath;

            if (device == null)
            {
                device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }

            CleanupPlayback();

            _waveSource =
                CodecFactory.Instance.GetCodec(fileAudioPath)
                    .ToSampleSource()
                    .ToMono()
                    .ToWaveSource();
            AudioDevice = device;
            _soundOut = new WasapiOut() { Latency = 100, Device = AudioDevice };
            _soundOut.Initialize(_waveSource);
            _soundOut.Stopped += _soundOut_Stopped;
        }


        private void _soundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            OnSoundStateChange?.Invoke(SoundState.Stopped);
        }

        public void Play()
        {
            if (_soundOut != null)
            {
                _soundOut.WaveSource.SetPosition(TimeSpan.FromMilliseconds(0));
                _soundOut.Play();
                OnSoundStateChange?.Invoke(SoundState.Started);
            }
        }

        public void Pause()
        {
            if (_soundOut != null)
            {
                _soundOut.Pause();
                OnSoundStateChange?.Invoke(SoundState.Paused);
            }
        }

        public void Stop()
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
                //OnSoundStateChange?.Invoke(SoundState.Stopped);
            }
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        public void ChangeAudioDevice(MMDevice device)
        {
            bool isPlaying = false;
            TimeSpan position = TimeSpan.FromMilliseconds(0);
            if(_soundOut.PlaybackState == PlaybackState.Playing)
            {
                isPlaying = true;
                position = _soundOut.WaveSource.GetPosition();
            }

            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }

            AudioDevice = device;
            _soundOut = new WasapiOut() { Latency = 100, Device = AudioDevice };
            _soundOut.Initialize(_waveSource); 
            _soundOut.Stopped += _soundOut_Stopped;
            if (isPlaying)
            {
                _soundOut.WaveSource.SetPosition(position);
                _soundOut.Play();
            }
        }

        public void ChangeAudioTrack(string filePath)
        {
            AudioFilepath = filePath;
            _soundOut.Stop();
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }

            _waveSource =
                CodecFactory.Instance.GetCodec(filePath)
                    .ToSampleSource()
                    .ToMono()
                    .ToWaveSource();

            _soundOut.Initialize(_waveSource);
            OnAudioTrackChange?.Invoke(filePath);
        }
    }
}
