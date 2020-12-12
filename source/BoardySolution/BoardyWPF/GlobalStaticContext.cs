using BoardyWPF.Controls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardyWPF
{
    public static class GlobalStaticContext
    {
        private static List<Tuple<int, PadControl>> _midiPushButtonRegisteredPads = new List<Tuple<int, PadControl>>();
        private static List<Tuple<SevenBitNumber, PadControl>> _midiVolumeSliderRegisteredPads = new List<Tuple<SevenBitNumber, PadControl>>();
        private static List<PadControl> _allPads = new List<PadControl>();
        private static InputDevice _midiIn;
        private static OutputDevice _midiOut;
        private static DevicesConnector _devConnector;

        private static bool LearnMode = false;

        public static List<PadControl> AllPads { get => _allPads; }

        private static event EventHandler<MidiEventReceivedEventArgs> LearnCodeReaded;
        public static void RegisterPadWithMidiNote(PadControl pad, int nota)
        {
            _midiPushButtonRegisteredPads.Add(new Tuple<int, PadControl>(nota, pad));
        }
        public static void AddPad(PadControl pad)
        {
            _allPads.Add(pad);
        }
        public static void AttachMidiInDevice(string midiDeviceId)
        {
            _midiIn = InputDevice.GetByName(midiDeviceId);
            if (_midiIn != null)
            {
                _midiIn.EventReceived += _midiIn_EventReceived;
                _midiIn.StartEventsListening();
            }
        }

        private static void _midiIn_EventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            if (LearnMode)
            {
                LearnCodeReaded?.Invoke(sender, e);
            }
            else
            {
                if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NoteOn)
                {
                    var evento = (Melanchall.DryWetMidi.Core.NoteOnEvent)e.Event;

                    if(ApplicationSettings.MidiChannel != evento.Channel)
                    {
                        ApplicationSettings.MidiChannel = evento.Channel;
                        ApplicationSettings.SaveConfig();
                    }    

                    _midiPushButtonRegisteredPads.Where(x => x.Item1 == evento.NoteNumber).ToList().ForEach(x =>
                    {
                        x.Item2.PlaySound();
                    });
                }

                if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.ControlChange)
                {
                    var evento = (Melanchall.DryWetMidi.Core.ControlChangeEvent)e.Event;
                    _midiVolumeSliderRegisteredPads.Where(x => x.Item1 == evento.ControlNumber).ToList().ForEach(x =>
                    {
                        x.Item2.ChangeVolume(evento.ControlValue, true);
                    });
                }
            }
        }

        public static void AttachMidiOutDevice(string midiDeviceId)
        {
            if (_midiOut != null)
                _midiOut.Dispose();
            _midiOut = OutputDevice.GetByName(midiDeviceId);
            if (_midiIn != null)
            {
                _midiOut.PrepareForEventsSending();
            }
        }
        public static void AttachMidiRepeatDevice(string midiDeviceId)
        {
            var listoutDev = _devConnector != null ? _devConnector.OutputDevices.ToList() : new List<IOutputDevice>();
            var repeatDev = OutputDevice.GetByName(midiDeviceId);
            if (repeatDev != null)
            {
                listoutDev.Add(repeatDev);

                if (_devConnector != null)
                    _devConnector.Disconnect();

                _devConnector = _midiIn.Connect(listoutDev.ToArray());
            }
        }
        internal static void DetachAllMidiDevices()
        {
            if (_devConnector != null)
                _devConnector.Disconnect();

            if (_midiIn != null)
                _midiIn.Dispose();
        }

        public static void EnterLearnMode(EventHandler<MidiEventReceivedEventArgs> eventHandler)
        {
            LearnMode = true;
            LearnCodeReaded = eventHandler;
        }

        public static void ExitLearnMode()
        {
            LearnMode = false;
            LearnCodeReaded = null;
        }

        internal static void RegisterPadWithMidiController(PadControl padControl, SevenBitNumber volumeSliderMidiController)
        {
            _midiVolumeSliderRegisteredPads.Add(new Tuple<SevenBitNumber, PadControl>(volumeSliderMidiController, padControl));
        }

        public static void SendOutputFeedbackMessage(MidiEvent midiEvent)
        {
            switch (midiEvent.EventType)
            {
                case MidiEventType.NoteOn:
                case MidiEventType.NoteOff: ((NoteEvent)midiEvent).Channel = (FourBitNumber)ApplicationSettings.MidiChannel; break;
            }

            _midiOut.SendEvent(midiEvent);
        }

        internal static void RemovePad(PadControl padControl)
        {
            _allPads.Remove(padControl);

            if (padControl._pushButtonMidiNote != -1)
                _midiPushButtonRegisteredPads.RemoveAll(x => x.Item1 == padControl._pushButtonMidiNote);

            if (padControl._volumeSliderMidiController > 0)
                _midiVolumeSliderRegisteredPads.RemoveAll(x => x.Item1 == padControl._volumeSliderMidiController);
        }
    }
}
