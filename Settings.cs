using System;
using System.IO;
using System.Xml.Serialization;

namespace Screenshoter {
    [Serializable]
    public class Settings {
        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Screenshoter", "config.xml");

        public string SavePath { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshoter");

        public int CaptureTime { get; set; } = 60;
        public int DeleteDays { get; set; } = 90;

        private Settings() { }

        public static Settings Load() {
            if (!File.Exists(SettingsFile)) {
                return new Settings();
            }

            var formatter = new XmlSerializer(typeof(Settings));
            using var fs = File.OpenRead(SettingsFile);
            var settings = formatter.Deserialize(fs) as Settings;
            return settings ?? new Settings();
        }

        public void Save() {
            var formatter = new XmlSerializer(typeof(Settings));
            var directory = Path.GetDirectoryName(SettingsFile);
            if (directory != null) {
                Directory.CreateDirectory(directory);
            } else {
                throw new Exception($"Filed to get directory for file {SettingsFile}");
            }

            using var fs = File.Create(SettingsFile);
            formatter.Serialize(fs, this);
        }
    }
}