using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RegionTrigger
{
    public class Trigger
    {
        public string RegionName { get; set; }
        public string Command { get; set; }
        public string TriggerName { get; set; }

        public string Start { get; set; } = "";
        public string Delay { get; set; } = "";
        public string Cooldown { get; set; } = "";

        public DateTime LastActivated { get; set; } = DateTime.MinValue;
    }

    public static class TRjson
    {
        public static List<Trigger> Triggers = new List<Trigger>();

        public static void LoadTriggers(string path)
        {
            if (File.Exists(path))
            {
                Triggers = JsonConvert.DeserializeObject<List<Trigger>>(File.ReadAllText(path)) ?? new List<Trigger>();
            }
        }

        public static void SaveTriggers(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Triggers, Formatting.Indented));
        }
    }
}