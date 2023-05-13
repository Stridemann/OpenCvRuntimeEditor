namespace OpenCvRuntimeEditor.Settings
{
    using System.Collections.ObjectModel;
    using System.IO;
    using Newtonsoft.Json;

    public class GeneralSettings
    {
        private const string SETTINGS_NAME = "Settings.json";
        public double GridMoveStepSize = 16;
        public string LastFilePath;
        public ObservableCollection<LastFile> LastOpenedFiles = new ObservableCollection<LastFile>();
        public bool LoadDocumentation = true;

        public GeneralSettings()
        {
            Instance = this;
        }

        public static GeneralSettings Instance { get; private set; }

        public static GeneralSettings Load()
        {
            if (!File.Exists(SETTINGS_NAME))
            {
                return new GeneralSettings();
            }

            var json = File.ReadAllText(SETTINGS_NAME);
            return JsonConvert.DeserializeObject<GeneralSettings>(json);
        }

        public static void Save()
        {
            if (Instance == null)
                return;

            var json = JsonConvert.SerializeObject(Instance);
            File.WriteAllText(SETTINGS_NAME, json);
        }
    }

    public class LastFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
