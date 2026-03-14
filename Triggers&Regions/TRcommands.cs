using System.Linq;
using TShockAPI;

namespace RegionTrigger
{
    public static class TRcommands
    {
        public static string TriggerFilePath { get; set; }

        public static void TriggerCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                SendHelp(args.Player);
                return;
            }

            string subCmd = args.Parameters[0].ToLower();

            switch (subCmd)
            {
                case "add":
                case "list":
                case "delete":
                case "rename":
                case "clear":
                    if (!args.Player.HasPermission("trigger.permission"))
                    {
                        args.Player.SendErrorMessage(TRi18s.GetString("NoPermission"));
                        return;
                    }

                    if (subCmd == "add") HandleAdd(args);
                    else if (subCmd == "list") HandleList(args);
                    else if (subCmd == "delete") HandleDelete(args);
                    else if (subCmd == "rename") HandleRename(args);
                    else if (subCmd == "clear") HandleClear(args);
                    break;

                case "run":
                    if (!args.Player.HasPermission("trigger.run"))
                    {
                        args.Player.SendErrorMessage(TRi18s.GetString("NoPermission"));
                        return;
                    }
                    HandleRun(args);
                    break;

                default:
                    SendHelp(args.Player);
                    break;
            }
        }

        private static void HandleAdd(CommandArgs args)
        {
            if (args.Parameters.Count >= 4)
            {
                var newTrigger = new Trigger
                {
                    RegionName = args.Parameters[1],
                    Command = args.Parameters[2],
                    TriggerName = args.Parameters[3]
                };

                if (args.Parameters.Count >= 5) newTrigger.Start = args.Parameters[4];
                if (args.Parameters.Count >= 6) newTrigger.Delay = args.Parameters[5];
                if (args.Parameters.Count >= 7) newTrigger.Cooldown = args.Parameters[6];

                TRjson.Triggers.Add(newTrigger);
                TRjson.SaveTriggers(TriggerFilePath);
                args.Player.SendSuccessMessage(TRi18s.GetString("TriggerAdded", newTrigger.TriggerName));
            }
            else SendHelp(args.Player);
        }

        private static void HandleList(CommandArgs args)
        {
            string triggerNames = string.Join(", ", TRjson.Triggers.Select(t => t.TriggerName));
            args.Player.SendInfoMessage(TRi18s.GetString("ListHeader", triggerNames));
        }

        private static void HandleDelete(CommandArgs args)
        {
            if (args.Parameters.Count >= 2)
            {
                string id = args.Parameters[1];
                var t = TRjson.Triggers.FirstOrDefault(tr => tr.TriggerName == id);
                if (t != null)
                {
                    TRjson.Triggers.Remove(t);
                    TRjson.SaveTriggers(TriggerFilePath);
                    args.Player.SendSuccessMessage(TRi18s.GetString("TriggerDeleted", id));
                }
                else args.Player.SendErrorMessage(TRi18s.GetString("TriggerNotFound", id));
            }
        }

        private static void HandleRename(CommandArgs args)
        {
            if (args.Parameters.Count >= 3)
            {
                string id = args.Parameters[1];
                string newName = args.Parameters[2];
                var t = TRjson.Triggers.FirstOrDefault(tr => tr.TriggerName == id);
                if (t != null)
                {
                    t.TriggerName = newName;
                    TRjson.SaveTriggers(TriggerFilePath);
                    args.Player.SendSuccessMessage(TRi18s.GetString("TriggerRenamed", id, newName));
                }
                else args.Player.SendErrorMessage(TRi18s.GetString("TriggerNotFound", id));
            }
        }

        private static void HandleRun(CommandArgs args)
        {
            if (args.Parameters.Count >= 2)
            {
                string id = args.Parameters[1];
                var t = TRjson.Triggers.FirstOrDefault(tr => tr.TriggerName == id);
                if (t != null && TRmisc.CanExecute(t))
                {
                    TRmisc.ExecuteTrigger(t, args.Player);
                }
            }
        }

        private static void HandleClear(CommandArgs args)
        {
            TRjson.Triggers.Clear();
            TRjson.SaveTriggers(TriggerFilePath);
            args.Player.SendSuccessMessage(TRi18s.GetString("TriggersCleared"));
        }

        private static void SendHelp(TSPlayer player)
        {
            player.SendInfoMessage(TRi18s.GetString("HelpHeader"));
            player.SendInfoMessage(TRi18s.GetString("HelpAdd"));
            player.SendInfoMessage("/trigger list");
            player.SendInfoMessage("/trigger delete <name>");
            player.SendInfoMessage("/trigger rename <name> <newName>");
            player.SendInfoMessage("/trigger run <name>");
            player.SendInfoMessage("/trigger clear");
        }
    }
}