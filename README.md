# ESPAÑOL

## 📖 Descripción
**Triggers&regions** es un plugin para servidores **TShock** que permite ejecutar comandos automáticamente al entrar en regiones o de forma manual mediante triggers.  
Incluye soporte para cooldowns, delays y reescritura de comandos que normalmente no funcionan desde la consola.

---

## ⚙️ Funcionalidades principales
- **Triggers basados en regiones**: ejecuta comandos al entrar en una región definida.
- **Triggers manuales**: crea triggers sin región (`none` o `global`) y ejecútalos con `/trigger run`.
- **Cooldown y delay**: configurable por trigger.
- **Persistencia**: todos los triggers se guardan en `triggers.json`.

---

## 📝 Comandos disponibles
- `/trigger add <region|none> <command(s)> <name> [cooldown] [delay]`  
  Crea un trigger en una región o manual (si se usa `none`).
- `/trigger list`  
  Lista todos los triggers.
- `/trigger delete <number|name>`  
  Elimina un trigger por índice o nombre.
- `/trigger rename <number|name> <newName>`  
  Renombra un trigger.
- `/trigger run <number|name>`  
  Ejecuta un trigger manualmente.
- `/trigger clear`  
  Elimina todos los triggers de una vez.
- `/trigger help`  
  Muestra la ayuda.
  
---

## 🔑 Permisos
- Para **agregar, editar, eliminar, renombrar, limpiar o ejecutar triggers**, un jugador debe tener el permiso `trigger.permission`
- Sin este permiso, los jugadores no pueden gestionar triggers.

---

## 📂 Persistencia
- Los triggers se guardan en `triggers.json` dentro de la carpeta de TShock.
- Se cargan automáticamente al iniciar el servidor.
- Se actualizan cada vez que se agregan, eliminan, renombran o se limpian.

---

## ✅ Ejemplos de uso

- Crear un trigger automático (activado por region):  
`/trigger add MiRegion "/heal && /buff 1" CurarBuff 30`
Al entrar en `MiRegion`, el jugador se cura y recibe el buff 1, con un cooldown de 30 segundos.

- Crear un trigger manual:
`/trigger add none "/spawnboss 50" BossManual 60 0`
Crea un trigger llamado `BossManual` que invoca al boss con ID 50.  
Solo se ejecuta manualmente con `/trigger run BossManual`

---

## 👤 Autor
- **PakeMPC**

---

## 📌 Notas
- Todos los comandos se ejecutan como **Server/SuperAdmin** para evitar problemas de permisos, aunque esto puede provocar que no funcionen algunos comandos de plugins de terceros.
- Los triggers sin región (`none`) nunca se activan automáticamente, solo mediante `/trigger run`.
- El cooldown afecta en cuánto tiempo puede volver a activarse/ejecutarse el trigger.
- El delay afecta cuanto tardará en ejecutarse cada comando, si tienes varios comandos en un trigger y pusiste delay de 5, el primer comando se ejecutará después de 5 segundos, el segundo se ejecutará 5 segundos después que el primero y así sucesivamente.






# ENGLISH

## 📖 Description
**Triggers&regions** is a plugin for **TShock** servers that allows executing commands automatically when entering regions or manually through triggers.  
It includes support for cooldowns, delays, and command rewriting for commands that normally don’t work from the console.

---

## ⚙️ Main Features
- **Region-based triggers**: run commands when entering a defined region.
- **Manual triggers**: create triggers without a region (`none` or `global`) and run them with `/trigger run`.
- **Cooldown and delay**: configurable per trigger.
- **Persistence**: all triggers are saved in `triggers.json`.

---

## 📝 Available Commands
- `/trigger add <region|none> <command(s)> <name> [cooldown] [delay]`  
  Creates a trigger in a region or manual-only (if `none` is used).
- `/trigger list`  
  Lists all triggers.
- `/trigger delete <number|name>`  
  Deletes a trigger by index or name.
- `/trigger rename <number|name> <newName>`  
  Renames a trigger.
- `/trigger run <number|name>`  
  Executes a trigger manually.
- `/trigger clear`  
  Deletes all triggers at once.
- `/trigger help`  
  Shows help information.

---

## 🔑 Permissions
- To **add, edit, delete, rename, clear, or run triggers**, a player must have the `trigger.permission`
- Without this permission, players cannot manage triggers.

---

## 📂 Persistence
- Triggers are saved in `triggers.json` inside the TShock folder.
- They are automatically loaded when the server starts.
- They are updated whenever triggers are added, deleted, renamed, or cleared.

---

## ✅ Usage Examples

- Create an automatic trigger:
`/trigger add MyRegion "/heal && /buff 1" HealBuff 30 0`
When entering `MyRegion`, the player is healed and receives buff 1, with a 30-second cooldown.

- Create a manual trigger:
`/trigger add none "/spawnboss 50" BossManual 60 0`
Creates a trigger named `BossManual` that spawns the boss with ID 50.  
It only runs manually with:
`/trigger run BossManual`

---

## 👤 Author
- **PakeMPC**

---

## 📌 Notes
- All commands are executed as **Server/SuperAdmin** to avoid permission issues.
- Triggers without a region (`none`) never activate automatically, only via `/trigger run`.
