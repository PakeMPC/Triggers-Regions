using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace RegionTrigger
{
    public static class TRmisc
    {
        private static int ResolveNPCID(string nameOrId)
        {
            if (int.TryParse(nameOrId, out int id)) return id;

            switch (nameOrId.ToLower())
            {
                case "eol": case "empress": case "emperatriz": case "mami": return NPCID.HallowBoss;
                case "wof": case "wall": case "muro": case "carne": return NPCID.WallofFlesh;
                case "ml": case "moonlord": case "moon": case "cthulhu": case "muslo": return NPCID.MoonLordCore;
                case "skeleprime": case "prime": case "skeleporn": return NPCID.SkeletronPrime;
                case "twins": case "gemelos": return NPCID.Retinazer;
                case "fishron": case "duke": case "duque": return NPCID.DukeFishron;
                case "plantera": return NPCID.Plantera;
                case "golem": return NPCID.Golem;
                case "king": case "rey": return NPCID.KingSlime;
                case "queen": case "reina": return NPCID.QueenSlimeBoss;
                case "eye": case "ojo": return NPCID.EyeofCthulhu;
                case "sans": case "skeletron": return NPCID.SkeletronHead;
                case "zombie": return NPCID.Zombie;
                default:
                    var found = TShock.Utils.GetNPCByIdOrName(nameOrId);
                    return found.Count > 0 ? found[0].type : -1;
            }
        }

        private static List<string> Tokenize(string input)
        {
            var args = new List<string>();
            string current = "";
            bool inQuotes = false;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '\"') inQuotes = !inQuotes;
                else if (c == ' ' && !inQuotes)
                {
                    if (!string.IsNullOrWhiteSpace(current)) args.Add(current);
                    current = "";
                }
                else current += c;
            }
            if (!string.IsNullOrWhiteSpace(current)) args.Add(current);
            return args;
        }

        public static int ParseTime(string timeStr, string defaultTime)
        {
            string t = string.IsNullOrWhiteSpace(timeStr) ? defaultTime : timeStr;
            t = t.ToLower().Trim();
            try
            {
                if (t.EndsWith("ms")) return int.Parse(t.Replace("ms", ""));
                if (t.EndsWith("m")) return int.Parse(t.Replace("m", "")) * 60000;
                if (t.EndsWith("s")) return int.Parse(t.Replace("s", "")) * 1000;
                return int.Parse(t);
            }
            catch { return 0; }
        }

        public static bool CanExecute(Trigger trigger)
        {
            int cooldownMs = ParseTime(trigger.Cooldown, TRcore.Config.DefaultCooldown);
            return (DateTime.Now - trigger.LastActivated).TotalMilliseconds >= cooldownMs;
        }

        public static async void ExecuteTrigger(Trigger trigger, TSPlayer player)
        {
            trigger.LastActivated = DateTime.Now;
            int startMs = ParseTime(trigger.Start, TRcore.Config.DefaultStart);
            int delayMs = ParseTime(trigger.Delay, TRcore.Config.DefaultDelay);

            if (startMs > 0) await Task.Delay(startMs);
            if (player == null || !player.Active) return;

            string[] cmds = trigger.Command.Split(new string[] { "&&" }, StringSplitOptions.None);

            for (int i = 0; i < cmds.Length; i++)
            {
                if (player == null || !player.Active) return;

                string cmd = cmds[i].Replace("{player}", $"\"{player.Name}\"").Trim();
                if (!cmd.StartsWith("/")) cmd = "/" + cmd;

                var commandArgs = Tokenize(cmd);
                if (commandArgs.Count > 0)
                {
                    string firstWord = commandArgs[0].ToLower();

                    // LÓGICA DE SPAWNING MANUAL
                    if (firstWord == "/spawnboss" || firstWord == "/spawnmob")
                    {
                        if (commandArgs.Count >= 2)
                        {
                            int npcId = ResolveNPCID(commandArgs[1]);
                            int amount = 1;
                            if (commandArgs.Count >= 3) int.TryParse(commandArgs[2], out amount);

                            if (npcId > 0)
                            {
                                int direction = player.TPlayer.direction;
                                float spawnX = player.X + (direction * 160);
                                float spawnY = player.Y;

                                for (int j = 0; j < amount; j++)
                                {
                                    int index = NPC.NewNPC(NPC.GetBossSpawnSource(player.Index), (int)spawnX, (int)spawnY, npcId);
                                    NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, index);
                                }
                                continue; 
                            }
                        }
                    }
                    // IA de /i a /give
                    else if (firstWord == "/i" || firstWord == "/item")
                    {
                        if (commandArgs.Count >= 2)
                        {
                            string itemName = commandArgs[1];
                            string amount = commandArgs.Count >= 3 ? commandArgs[2] : "1";
                            cmd = $"/give \"{itemName}\" \"{player.Name}\" {amount}";
                        }
                    }
                    // Autocompletado de comandos (agrega el nombre del jugador)
                    else if (commandArgs.Count == 1 && (firstWord == "/heal" || firstWord == "/god" || firstWord == "/kill" || firstWord == "/clear" || firstWord == "/buff"))
                    {
                        cmd += $" \"{player.Name}\"";
                    }
                }

                Commands.HandleCommand(TSPlayer.Server, cmd);

                if (delayMs > 0 && i < cmds.Length - 1)
                {
                    await Task.Delay(delayMs);
                }
            }
        }
    }
}