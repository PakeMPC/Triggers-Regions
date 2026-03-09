using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RegionTrigger
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "Triggers&Regions";
        public override string Author => "PakeMPC";
        public override string Description => "Execute commands when entering regions or manually with triggers";
        public override Version Version => typeof(Plugin).Assembly.GetName().Version;

        private Dictionary<int, string> lastRegion = new Dictionary<int, string>();
        private List<Trigger> triggers = new List<Trigger>();
        private string triggerFile = Path.Combine(TShock.SavePath, "triggers.json");

        public Plugin(Main game) : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            Commands.ChatCommands.Add(new Command("trigger.permission", TriggerCommand, "trigger"));
            LoadTriggers();
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
                int tileX = (int)(player.X / 16);
                int tileY = (int)(player.Y / 16);

                var regions = TShock.Regions.InAreaRegionName(tileX, tileY);
                string region = regions.FirstOrDefault() ?? string.Empty;

                if (!lastRegion.ContainsKey(player.Index))
                    lastRegion[player.Index] = region;

                if (region != lastRegion[player.Index])
                {
                    foreach (var trigger in triggers.Where(t => !string.IsNullOrEmpty(t.RegionName) && t.RegionName == region))
                    {
                        if ((DateTime.Now - trigger.LastActivated).TotalSeconds < trigger.Cooldown)
                            continue;

                        trigger.LastActivated = DateTime.Now;
                        ExecuteTrigger(trigger, player);
                    }

                    lastRegion[player.Index] = region;
                }
            }
        }

        private void ExecuteTrigger(Trigger trigger, TSPlayer player)
        {
            var commands = trigger.Command.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            int delayAccum = 0;

            foreach (var cmd in commands)
            {
                Action executeCommand = () =>
                {
                    try
                    {
                        string rewritten = RewriteCommandForServer(cmd, player);
                        if (!string.IsNullOrEmpty(rewritten))
                        {
                            Commands.HandleCommand(TSPlayer.Server, rewritten);
                        }
                    }
                    catch (Exception ex)
                    {
                        player.SendErrorMessage($"Error executing command: {cmd.Trim()} ({ex.Message})");
                    }
                };

                if (trigger.Delay == 0)
                {
                    executeCommand();
                }
                else
                {
                    int delayMs = (trigger.Delay + delayAccum) * 1000;
                    delayAccum += trigger.Delay;

                    var timer = new Timer(delayMs) { AutoReset = false };
                    timer.Elapsed += (s, e) =>
                    {
                        executeCommand();
                        timer.Dispose();
                    };
                    timer.Start();
                }
            }
        }

        private string RewriteCommandForServer(string cmd, TSPlayer player)
        {
            string[] parts = cmd.Trim().Split(' ');
            if (parts.Length == 0) return cmd;

            string baseCmd = parts[0].ToLower();

            if (baseCmd == "/i" || baseCmd == "/item")
            {
                if (parts.Length >= 3)
                {
                    string itemId = parts[1];
                    string amount = parts[2];
                    string prefix = parts.Length >= 4 ? parts[3] : "0";
                    return $"/give {itemId} {player.Name} {amount} {prefix}";
                }
            }

            if (baseCmd == "/heal")
            {
                player.Heal(player.TPlayer.statLifeMax2);
                player.SendSuccessMessage("You have been healed.");
                return "";
            }

            if (baseCmd == "/god")
            {
                player.GodMode = !player.GodMode;
                player.SendSuccessMessage($"God mode {(player.GodMode ? "enabled" : "disabled")}.");
                return "";
            }

            if (baseCmd == "/buff" && parts.Length >= 2)
            {
                if (int.TryParse(parts[1], out int buffId))
                {
                    int durationSeconds = parts.Length >= 3 ? int.Parse(parts[2]) : 60;
                    int durationFrames = durationSeconds * 60;
                    player.SetBuff(buffId, durationFrames);
                    player.SendSuccessMessage($"Buff {buffId} applied for {durationSeconds} seconds.");
                    return "";
                }
            }

            if (baseCmd == "/kill")
            {
                player.DamagePlayer(999999);
                player.SendSuccessMessage("You have been killed.");
                return "";
            }

            if (baseCmd == "/tp")
            {
                if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                {
                    player.Teleport(x * 16, y * 16);
                    player.SendSuccessMessage($"Teleported to {x},{y}.");
                    return "";
                }
                else if (parts.Length == 2)
                {
                    string dest = parts[1].ToLower();
                    if (dest == "spawn")
                    {
                        player.Teleport(Main.spawnTileX * 16, Main.spawnTileY * 16);
                        player.SendSuccessMessage("Teleported to spawn.");
                        return "";
                    }
                    else if (dest == "home" && player.TPlayer.SpawnX != -1 && player.TPlayer.SpawnY != -1)
                    {
                        player.Teleport(player.TPlayer.SpawnX * 16, player.TPlayer.SpawnY * 16);
                        player.SendSuccessMessage("Teleported to home.");
                        return "";
                    }
                    else if (dest == "dungeon")
                    {
                        player.Teleport(Main.dungeonX * 16, Main.dungeonY * 16);
                        player.SendSuccessMessage("Teleported to dungeon entrance.");
                        return "";
                    }
                }
            }

            if (baseCmd == "/time" && parts.Length >= 2)
            {
                string mode = parts[1].ToLower();
                if (mode == "day")
                {
                    Main.dayTime = true;
                    Main.time = 0;
                    Main.bloodMoon = false;
                    Main.eclipse = false;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo, "");
                    TSPlayer.All.SendInfoMessage("Time set to day.");
                    return "";
                }
                else if (mode == "night")
                {
                    Main.dayTime = false;
                    Main.time = 0;
                    Main.bloodMoon = false;
                    Main.eclipse = false;
                    TSPlayer.All.SendData(PacketTypes.WorldInfo, "");
                    TSPlayer.All.SendInfoMessage("Time set to night.");
                    return "";
                }
            }

            if (baseCmd == "/spawnboss" && parts.Length >= 2)
            {
                if (int.TryParse(parts[1], out int npcId))
                {
                    SpawnNpcNearPlayer(player, npcId, 1);
                    return "";
                }
            }

            if (baseCmd == "/spawnmob" && parts.Length >= 3)
            {
                if (int.TryParse(parts[1], out int npcId) && int.TryParse(parts[2], out int amount))
                {
                    SpawnNpcNearPlayer(player, npcId, amount);
                    return "";
                }
            }

            return cmd;
        }

        private void SpawnNpcNearPlayer(TSPlayer player, int npcId, int amount)
        {
            int tileX = (int)(player.X / 16);
            int tileY = (int)(player.Y / 16);

            for (int i = 0; i < amount; i++)
            {
                int npcIndex = Terraria.NPC.NewNPC(null, tileX * 16, tileY * 16, npcId);
                if (npcIndex >= 0 && npcIndex < Main.maxNPCs)
                {
                    Main.npc[npcIndex].TargetClosest();
                }
            }

            player.SendSuccessMessage($"Spawned {amount} NPC(s) with ID {npcId} near you.");
        }

        private void TriggerCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("Usage: /trigger <add|list|delete|rename|run|clear|help>");
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "add":
                    if (args.Parameters.Count < 4)
                    {
                        args.Player.SendErrorMessage("Usage: /trigger add <region|none> <command(s)> <name> [cooldown] [delay]");
                        return;
                    }

                    int cooldown = 0;
                    int delay = 0;

                    if (args.Parameters.Count >= 5)
                        int.TryParse(args.Parameters[4], out cooldown);
                    if (args.Parameters.Count >= 6)
                        int.TryParse(args.Parameters[5], out delay);

                    string regionParam = args.Parameters[1].ToLower();
                    string regionName = (regionParam == "none" || regionParam == "global") ? "" : args.Parameters[1];

                    triggers.Add(new Trigger
                    {
                        RegionName = regionName,
                        Command = args.Parameters[2],
                        TriggerName = args.Parameters[3],
                        Cooldown = cooldown,
                        Delay = delay
                    });
                    SaveTriggers();

                    args.Player.SendSuccessMessage($"Trigger '{args.Parameters[3]}' added successfully (region='{(string.IsNullOrEmpty(regionName) ? "none" : regionName)}').");
                    break;

                case "list":
                    if (triggers.Count == 0)
                    {
                        args.Player.SendInfoMessage("No triggers defined currently.");
                    }
                    else
                    {
                        int i = 1;
                        foreach (var t in triggers)
                        {
                            string regionDisplay = string.IsNullOrEmpty(t.RegionName) ? "none" : t.RegionName;
                            args.Player.SendInfoMessage($"{i}. {t.TriggerName} -> Region: {regionDisplay}, Command(s): {t.Command}, Cooldown: {t.Cooldown}s, Delay: {t.Delay}s");
                            i++;
                        }
                    }
                    break;

                case "delete":
                    if (args.Parameters.Count < 2)
                    {
                        args.Player.SendErrorMessage("Usage: /trigger delete <number|name>");
                        return;
                    }
                    string target = args.Parameters[1];
                    Trigger toRemove = null;

                    if (int.TryParse(target, out int index) && index > 0 && index <= triggers.Count)
                        toRemove = triggers[index - 1];
                    else
                        toRemove = triggers.FirstOrDefault(t => t.TriggerName.Equals(target, StringComparison.OrdinalIgnoreCase));

                    if (toRemove != null)
                    {
                        triggers.Remove(toRemove);
                        SaveTriggers();
                        args.Player.SendSuccessMessage("Trigger deleted.");
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Trigger not found.");
                    }
                    break;

                case "rename":
                    if (args.Parameters.Count < 3)
                    {
                        args.Player.SendErrorMessage("Usage: /trigger rename <number|name> <newName>");
                        return;
                    }
                    string targetRename = args.Parameters[1];
                    string newName = args.Parameters[2];
                    Trigger toRename = null;

                    if (int.TryParse(targetRename, out int idx) && idx > 0 && idx <= triggers.Count)
                        toRename = triggers[idx - 1];
                    else
                        toRename = triggers.FirstOrDefault(t => t.TriggerName.Equals(targetRename, StringComparison.OrdinalIgnoreCase));

                    if (toRename != null)
                    {
                        toRename.TriggerName = newName;
                        SaveTriggers();
                        args.Player.SendSuccessMessage("Trigger renamed.");
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Trigger not found.");
                    }
                    break;

                case "run":
                    if (args.Parameters.Count < 2)
                    {
                        args.Player.SendErrorMessage("Usage: /trigger run <number|name>");
                        return;
                    }

                    string targetRun = args.Parameters[1];
                    Trigger toRun = null;

                    if (int.TryParse(targetRun, out int runIndex) && runIndex > 0 && runIndex <= triggers.Count)
                        toRun = triggers[runIndex - 1];
                    else
                        toRun = triggers.FirstOrDefault(t => t.TriggerName.Equals(targetRun, StringComparison.OrdinalIgnoreCase));

                    if (toRun != null)
                    {
                        if ((DateTime.Now - toRun.LastActivated).TotalSeconds < toRun.Cooldown)
                        {
                            args.Player.SendErrorMessage($"Trigger '{toRun.TriggerName}' is on cooldown.");
                            return;
                        }

                        toRun.LastActivated = DateTime.Now;
                        ExecuteTrigger(toRun, args.Player);
                        args.Player.SendSuccessMessage($"Trigger '{toRun.TriggerName}' executed.");
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Trigger not found.");
                    }
                    break;

                case "clear":
                    triggers.Clear();
                    SaveTriggers();
                    args.Player.SendSuccessMessage("All triggers have been deleted.");
                    break;

                case "help":
                    args.Player.SendInfoMessage("=== /trigger Help ===");
                    args.Player.SendInfoMessage("/trigger add <region|none> <command(s)> <name> [cooldown] [delay]");
                    args.Player.SendInfoMessage("/trigger list");
                    args.Player.SendInfoMessage("/trigger delete <number|name>");
                    args.Player.SendInfoMessage("/trigger rename <number|name> <newName>");
                    args.Player.SendInfoMessage("/trigger run <number|name>");
                    args.Player.SendInfoMessage("/trigger clear");
                    args.Player.SendInfoMessage("/trigger help");
                    break;
            }
        }

        private void LoadTriggers()
        {
            if (File.Exists(triggerFile))
            {
                triggers = JsonConvert.DeserializeObject<List<Trigger>>(File.ReadAllText(triggerFile)) ?? new List<Trigger>();
            }
        }

        private void SaveTriggers()
        {
            File.WriteAllText(triggerFile, JsonConvert.SerializeObject(triggers, Formatting.Indented));
        }
    }

    public class Trigger
    {
        public string RegionName { get; set; }
        public string Command { get; set; } // commands separated by &&
        public string TriggerName { get; set; }
        public int Cooldown { get; set; } = 0; // seconds, default 0
        public int Delay { get; set; } = 0;    // seconds, default 0
        public DateTime LastActivated { get; set; } = DateTime.MinValue;
    }
}