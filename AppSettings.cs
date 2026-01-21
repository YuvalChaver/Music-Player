using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YuvalChaver.Telhai.DotNet.PlayerProject
{
    public class AppSettings
    {
        public ObservableCollection<string> MusicFolders { get; set; } = new ObservableCollection<string>();
        private const string SETTINGS_FILE = "settings.json";

        /// <summary>
        /// Save List into Settings
        /// </summary>
        /// <param name="settings"></param>
        public static void Save(AppSettings settings)
        {
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(SETTINGS_FILE, json);
        }
        public static AppSettings Load()
        {
            if (File.Exists(SETTINGS_FILE))
            {
                string json = File.ReadAllText(SETTINGS_FILE);
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                // Ensure MusicFolders is properly initialized as ObservableCollection
                if (settings.MusicFolders == null)
                {
                    settings.MusicFolders = new ObservableCollection<string>();
                }
                else if (!(settings.MusicFolders is ObservableCollection<string>))
                {
                    var temp = new ObservableCollection<string>(settings.MusicFolders);
                    settings.MusicFolders = temp;
                }
                return settings;
            }
            return new AppSettings();
        }
    }
}
