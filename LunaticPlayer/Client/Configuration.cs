using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LunaticPlayer.Client
{
    class ConfigurationData
    {
        public double Volume { get; set; }
    }

    class Configuration
    {
        private const string Filename = "config.json";

        private static Configuration _instance;

        public ConfigurationData Data { get; set; }

        /// <summary>
        /// The source of configuration data (Filesystem, Default Object, ...).
        /// </summary>
        public string Source { get; set; }

        public static Configuration GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Configuration();
                _instance.Initialize();
            }

            return _instance;
        }

        /// <summary>
        /// Loads the configuration and stores it to the filesystem if necessary.
        /// </summary>
        private void Initialize()
        {

            if (File.Exists(Filename))
            {
                Data = JsonConvert.DeserializeObject<ConfigurationData>(File.ReadAllText(Filename));
                Source = "Filesystem";
            }
            else
            {
                Source = "DefaultDataObject";

                SetupData();

                // Erstellt eine Konfigurationsdatei für das nächste Mal.
                Save();
            }
        }

        /// <summary>
        /// Creates a <seealso cref="ConfigurationData"/> object with default values.
        /// </summary>
        private void SetupData()
        {
            var data = new ConfigurationData()
            {
                Volume = 0.4
            };

            Data = data;
        }

        /// <summary>
        /// Saves the current configuration to the file specified at <see cref="Filename"/>.
        /// </summary>
        public void Save()
        {
            Console.WriteLine("Saving configuration to filesystem.");
            File.WriteAllText(Filename, JsonConvert.SerializeObject(Data));
        }
    }
}
