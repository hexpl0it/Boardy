using BoardyWPF.SettingsStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BoardyWPF
{
    public static class ApplicationSettings
    {
        private static InternalSettings _sets;
        public static string CallbackDeviceID
        {
            get
            {
                return _sets.CallbackDeviceID;
            }

            set
            {
                _sets.CallbackDeviceID = value;
            }
        }
        public static string MidiInputDeviceID
        {
            get
            {
                return _sets.MidiInputDeviceID;
            }
            set
            {
                _sets.MidiInputDeviceID = value;
            }
        }
        public static string MidiOutputDeviceID
        {
            get
            {
                return _sets.MidiOutputDeviceID;
            }
            set
            {
                _sets.MidiOutputDeviceID = value;
            }
        }
        public static string MidiOutputRepeatedDeviceID
        {
            get
            {
                return _sets.MidiOutputRepeatedDeviceID;
            }
            set
            {
                _sets.MidiOutputRepeatedDeviceID = value;
            }
        }
        public static List<PadControlSettingModel> PadControls
        {
            get
            {
                return _sets.PadControls;
            }
            set
            {
                _sets.PadControls = value;
            }
        }

        static ApplicationSettings()
        {
            _sets = new InternalSettings();
            LoadConfig();
        }

        public static void SaveConfig()
        {
            _sets.PadControls = GlobalStaticContext.AllPads.Select(x => new PadControlSettingModel()
            {
                AudioFilePath = x._audioPath,
                Volume = x._volume,
                AudioDeviceID = x._audioDeviceID,
                MidiNoteMap = x._pushButtonMidiNote,
                VolumeSliderControllerMap = x._volumeSliderMidiController
            }).ToList();
            JsonSerializerSettings settings = new JsonSerializerSettings();

            string jsonOutput = JsonConvert.SerializeObject(_sets, Formatting.Indented, settings);
            System.IO.File.WriteAllText(@"config.json", jsonOutput);
        }

        public static void LoadConfig()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            _sets = JsonConvert.DeserializeObject<InternalSettings>(System.IO.File.ReadAllText(@"config.json"), settings);
        }
    }

    class InternalSettings
    {
        [JsonProperty]
        internal string CallbackDeviceID { get; set; }
        [JsonProperty]
        internal string MidiInputDeviceID { get; set; }
        [JsonProperty]
        internal string MidiOutputDeviceID { get; set; }
        [JsonProperty]
        internal string MidiOutputRepeatedDeviceID { get; set; }
        [JsonProperty]
        internal List<PadControlSettingModel> PadControls { get; set; }

        internal InternalSettings()
        {

        }
    }
}
