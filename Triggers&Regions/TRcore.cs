using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RegionTrigger
{
    [ApiVersion(2, 1)]
    public class TRcore : TerrariaPlugin
    {
        public override string Name => "Triggers&Regions";
        public override string Author => "PakeMPC";
        public override string Description => "Execute commands when entering regions or manually with triggers";
        public override Version Version => typeof(TRcore).Assembly.GetName().Version;

        private Dictionary<int, string> lastRegion = new Dictionary<int, string>();
        private string configPath = Path.Combine(TShock.SavePath, "TR_Config.json");
        private string triggerPath = Path.Combine(TShock.SavePath, "TR_Triggers.json");

        public static TRconfig Config;

        public TRcore(Main game) : base(game) { }

        public override void Initialize()
        {
            Config = TRconfig.Read(configPath);
            TRi18s.CurrentLanguage = Config.Language;
            TRjson.LoadTriggers(triggerPath);
            TRcommands.TriggerFilePath = triggerPath;

            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            Commands.ChatCommands.Add(new Command(TRcommands.TriggerCommand, "trigger"));

            TShock.Log.ConsoleInfo("[Triggers&Regions] Mod cargado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void OnUpdate(EventArgs args)
        {
            foreach (TSPlayer player in TShock.Players.Where(p => p != null && p.Active))
            {
                var currentRegion = TShock.Regions.InAreaRegion(player.TileX, player.TileY).FirstOrDefault();
                string currentRegionName = currentRegion?.Name ?? "none";

                if (!lastRegion.ContainsKey(player.Index))
                {
                    lastRegion[player.Index] = "none";
                }

                if (lastRegion[player.Index] != currentRegionName)
                {
                    lastRegion[player.Index] = currentRegionName;

                    if (currentRegionName != "none")
                    {
                        var matchingTriggers = TRjson.Triggers.Where(t => t.RegionName == currentRegionName).ToList();
                        foreach (var trigger in matchingTriggers)
                        {
                            if (TRmisc.CanExecute(trigger))
                            {
                                TRmisc.ExecuteTrigger(trigger, player);
                            }
                        }
                    }
                }
            }
        }
    }
}