using System.Collections.Generic;

namespace RegionTrigger
{
    public static class TRi18s
    {
        public static string CurrentLanguage = "en";

        private static readonly Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = new Dictionary<string, string>
            {
                ["HelpHeader"] = "=== /trigger Help ===",
                ["HelpAdd"] = "/trigger add <region|none> \"<command(s)>\" <name> [start] [delay] [cooldown]",
                ["TriggerAdded"] = "Trigger '{0}' added successfully.",
                ["TriggerDeleted"] = "Trigger '{0}' deleted.",
                ["TriggerNotFound"] = "Trigger '{0}' not found.",
                ["TriggerRenamed"] = "Trigger '{0}' renamed to '{1}'.",
                ["TriggersCleared"] = "All triggers cleared.",
                ["ListHeader"] = "Triggers: {0}",
                ["NoPermission"] = "You do not have permission to use this sub-command."
            },
            ["es"] = new Dictionary<string, string>
            {
                ["HelpHeader"] = "=== /trigger Ayuda ===",
                ["HelpAdd"] = "/trigger add <region|none> \"<comando(s)>\" <nombre> [inicio] [retraso] [enfriamiento]",
                ["TriggerAdded"] = "Trigger '{0}' añadido con éxito.",
                ["TriggerDeleted"] = "Trigger '{0}' eliminado.",
                ["TriggerNotFound"] = "Trigger '{0}' no encontrado.",
                ["TriggerRenamed"] = "Trigger '{0}' renombrado a '{1}'.",
                ["TriggersCleared"] = "Todos los triggers han sido borrados.",
                ["ListHeader"] = "Triggers: {0}",
                ["NoPermission"] = "No tienes permiso para usar este sub-comando."
            },
            ["pt"] = new Dictionary<string, string>
            {
                ["HelpHeader"] = "=== /trigger Ajuda === ",
                ["HelpAdd"] = "/trigger add <region|none> \"<comando(s)>\" <nome> [inicio] [atraso] [espera]",
                ["TriggerAdded"] = "Trigger '{0}' adicionado com sucesso.",
                ["TriggerDeleted"] = "Trigger '{0}' excluído.",
                ["TriggerNotFound"] = "Trigger '{0}' não encontrado.",
                ["TriggerRenamed"] = "Trigger '{0}' renomeado para '{1}'.",
                ["TriggersCleared"] = "Todos os triggers foram apagados.",
                ["ListHeader"] = "Triggers: {0}",
                ["NoPermission"] = "Você não tem permissão para usar este sub-comando."
            }
        };

        public static string GetString(string key, params object[] args)
        {
            if (_translations.ContainsKey(CurrentLanguage) && _translations[CurrentLanguage].ContainsKey(key))
            {
                return string.Format(_translations[CurrentLanguage][key], args);
            }
            if (_translations.ContainsKey("en") && _translations["en"].ContainsKey(key))
            {
                return string.Format(_translations["en"][key], args);
            }
            return key;
        }
    }
}