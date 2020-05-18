using BoardyWPF.Controls;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardyWPF
{
    public static class GlobalStaticContext
    {
        private static List<Tuple<int, PadControl>> _midiPushButtonRegisteredPads = new List<Tuple<int, PadControl>>();
        private static List<Tuple<MidiController, PadControl>> _midiVolumeSliderRegisteredPads = new List<Tuple<MidiController, PadControl>>();
        private static List<PadControl> _allPads = new List<PadControl>();
        private static MidiIn _midiIn;
        private static MidiOut _midiOut;
        private static MidiOut _repeatDevice;

        private static bool LearnMode = false;

        public static List<PadControl> AllPads { get => _allPads; }

        private static event EventHandler<MidiInMessageEventArgs> LearnCodeReaded;
        public static void RegisterPadWithMidiNote(PadControl pad, int nota)
        {
            _midiPushButtonRegisteredPads.Add(new Tuple<int, PadControl>(nota, pad));
        }
        public static void AddPad(PadControl pad)
        {
            _allPads.Add(pad);
        }
        public static void AttachMidiInDevice(int midiDeviceId)
        {
            _midiIn = new MidiIn(midiDeviceId);
            _midiIn.MessageReceived += _midiIn_MessageReceived;
            _midiIn.Start();
        }
        public static void AttachMidiOutDevice(int midiDeviceId)
        {
            _midiOut = new MidiOut(midiDeviceId);
        }
        public static void AttachMidiRepeatDevice(int midiDeviceId)
        {
            _repeatDevice = new MidiOut(midiDeviceId);
        }
        internal static void DetachAllMidiDevices()
        {
            if (_midiIn != null)
                _midiIn.Close();

            if (_midiOut != null)
                _midiOut.Close();

            if (_repeatDevice != null)
                _repeatDevice.Close();
        }

        public static void SendMidiMessage(int rawMessage)
        {
            _midiOut.Send(rawMessage);
        }

        private static void _midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (LearnMode)
            {
                LearnCodeReaded?.Invoke(sender, e);
            }
            else
            {
                if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    var evento = (NoteOnEvent)e.MidiEvent;
                    _midiPushButtonRegisteredPads.Where(x => x.Item1 == evento.NoteNumber).ToList().ForEach(x =>
                    {
                        x.Item2.PlaySound();
                    });
                }

                if (e.MidiEvent.CommandCode == MidiCommandCode.ControlChange)
                {
                    var evento = (ControlChangeEvent)e.MidiEvent;
                    _midiVolumeSliderRegisteredPads.Where(x => x.Item1 == evento.Controller).ToList().ForEach(x =>
                    {
                        x.Item2.ChangeVolume(evento.ControllerValue, true);
                    });
                }
            }

            _repeatDevice.Send(e.RawMessage);

            switch (e.MidiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOff:
                    break;
                case MidiCommandCode.NoteOn:
                    break;
                case MidiCommandCode.KeyAfterTouch:
                    break;
                case MidiCommandCode.ControlChange:
                    break;
                case MidiCommandCode.PatchChange:
                    break;
                case MidiCommandCode.ChannelAfterTouch:
                    break;
                case MidiCommandCode.PitchWheelChange:
                    break;
                case MidiCommandCode.Sysex:
                    break;
                case MidiCommandCode.Eox:
                    break;
                case MidiCommandCode.TimingClock:
                    break;
                case MidiCommandCode.StartSequence:
                    break;
                case MidiCommandCode.ContinueSequence:
                    break;
                case MidiCommandCode.StopSequence:
                    break;
                case MidiCommandCode.AutoSensing:
                    break;
                case MidiCommandCode.MetaEvent:
                    break;
                default:
                    break;
            }
        }

        public static void EnterLearnMode(EventHandler<MidiInMessageEventArgs> eventHandler)
        {
            LearnMode = true;
            LearnCodeReaded = eventHandler;
        }

        public static void ExitLearnMode()
        {
            LearnMode = false;
            LearnCodeReaded = null;
        }

        internal static void RegisterPadWithMidiController(PadControl padControl, MidiController volumeSliderMidiController)
        {
            _midiVolumeSliderRegisteredPads.Add(new Tuple<MidiController, PadControl>(volumeSliderMidiController, padControl));
        }

        internal static void RemovePad(PadControl padControl)
        {
            _allPads.Remove(padControl);

            if (padControl._pushButtonMidiNote != -1)
                _midiPushButtonRegisteredPads.RemoveAll(x => x.Item1 == padControl._pushButtonMidiNote);

            if (padControl._volumeSliderMidiController.HasValue)
                _midiVolumeSliderRegisteredPads.RemoveAll(x => x.Item1 == padControl._volumeSliderMidiController);
        }
    }
}
