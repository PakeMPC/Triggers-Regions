using Newtonsoft.Json;
using System;
using System.IO;
using TShockAPI;

namespace RegionTrigger
{
    public class TRconfig
    {
        public string Language { get; set; } = "es";
        public string DefaultStart { get; set; } = "10ms";
        public string DefaultDelay { get; set; } = "50ms";
        public string DefaultCooldown { get; set; } = "5m";

        public static TRconfig Read(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    var defaultConfig = new TRconfig();
                    File.WriteAllText(path, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
                    TShock.Log.ConsoleInfo("[Triggers&Regions] Archivo TR_Config.json creado por defecto.");
                    return defaultConfig;
                }
                return JsonConvert.DeserializeObject<TRconfig>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[Triggers&Regions] Error leyendo TR_Config.json: {ex.Message}");
                return new TRconfig();
            }
        }
    }
}