using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace beer.umajkla.win
{
    public class AppConfig
    {
        public static AppConfig CurrentConfig;
        public static Dictionary<string, string> AppArgs;

        public string ServerAddress { get; set; }
        public Guid EventId { get; set; }
        public bool FirstStartDialogRequired { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public Dictionary<Guid,ItemChartDisplaySettings> DisplayItems { get; set; }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
            File.WriteAllText("config.json", json);
        }

        public static void LoadConfig()
        {
            AppArgs = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();
            for (int index = 1; index < args.Length; index += 2)
            {
                AppArgs.Add(args[index], args[index + 1]);
            }

            if (File.Exists("config.json"))
            {
                CurrentConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("config.json"));
                if (CurrentConfig.DisplayItems == null)
                {
                    CurrentConfig.DisplayItems = new Dictionary<Guid, ItemChartDisplaySettings>();
                    CurrentConfig.DisplayItems.Add(Guid.Empty, new ItemChartDisplaySettings());
                }
                CurrentConfig.FirstStartDialogRequired = false;
            }
            else
            {
                CurrentConfig = new AppConfig();
                CurrentConfig.DisplayItems = new Dictionary<Guid, ItemChartDisplaySettings>();
                CurrentConfig.DisplayItems.Add(Guid.Empty, new ItemChartDisplaySettings());
                CurrentConfig.FirstStartDialogRequired = true;
            }
        }

        public class ItemChartDisplaySettings
        {
            public Guid ItemId { get; set; } = new Guid();
            public string Name { get; set; } = "Defaults";
            public string SoldLabel { get; set; } = "Prodáno";
            public string RemainsLabel { get; set; } = "Zbývá";
            public Color SoldColor { get; set; } = Colors.Purple;
            public Color RemainsColor { get; set; } = Colors.Orange;
            public Color ProgressBarColor { get; set; } = new Color() { A = 255, R = 6, G = 176, B = 37 };
            public int Delay { get; set; } = 10;
            public bool Displayed { get; set; } = true;
            public bool BulkSelected { get; set; } = false;
        }
    }
}
