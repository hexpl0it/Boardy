using Melanchall.DryWetMidi.Common;

namespace BoardyWPF.SettingsStructure
{
    public class PadControlSettingModel
    {
        public string AudioFilePath { get; set; }
        public int Volume { get; set; }
        public string AudioDeviceID { get; set; }
        public int MidiNoteMap { get; set; }
        public int VolumeSliderControllerMap { get; set; }
    }
}
