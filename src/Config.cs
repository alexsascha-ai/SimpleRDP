using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace SimpleRDP {
    public class Config {

        private static Config? _instance;
        public static string filename = "config.json";
        public static string rdpDir = "rdp-files";
        
        public static Config GetConfig() {
            if (!File.Exists(filename)) {
                File.AppendAllText(filename, "{}");
            }

            if (!Directory.Exists(rdpDir)) {
                Directory.CreateDirectory(rdpDir);
            }

            var json = File.ReadAllText(filename);
            
            _instance = JsonConvert.DeserializeObject<Config>(json);

            if (_instance == null) {
                throw new Exception("Cannot read config.json");
            }
            
            if (_instance.Profiles == null) {
                _instance.Profiles = new List<Profile>();
                _instance.Save();
            }

            foreach (var item in _instance.Profiles) {
                if (item.IsGroup && item.Children == null) {
                    item.Children = new List<Profile>();
                }
            }

            return _instance;
        }

        public List<Profile> Profiles { get; set; } = new List<Profile>();

        public bool KeepOpening { get; set; } = true;

        public int FormHeight { get; set; } = 0;

        public void Save() {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}
