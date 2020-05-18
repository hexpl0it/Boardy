using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoardyWPF.SettingsStructure
{
    public class PadControlSettingModel
    {
        public string AudioFilePath { get; set; }
        public float Volume { get; set; }
        public int AudioDeviceID { get; set; }
        public int MidiNoteMap { get; set; }
        public MidiController? VolumeSliderControllerMap { get; set; }
    }
}
