# ESPAÑOL

## 📖 Descripción (ES)
**Triggers&Regions** es un plugin avanzado para servidores **TShock 6 (Terraria 1.4.5)** diseñado para automatizar eventos. Permite ejecutar cadenas de comandos al entrar en regiones o de forma manual, con un control preciso sobre los tiempos de espera y una IA interna que corrige comandos de consola.

## ⚙️ Funcionalidades Principales
- **Sistema de Tiempos Flexible**: Soporta unidades como milisegundos (`ms`), segundos (`s`) y minutos (`m`).
- **IA de Comandos**: Autocompleta automáticamente el nombre del jugador en comandos como `/heal`, `/god`, `/kill`, `/buff`, etc.
- **Traducción Automática**: Convierte comandos in-game (`/i` o `/item` a `/give` `/spawnmob` `/spawnboss`) para que funcionen correctamente desde el servidor.
- **Internacionalización**: Soporte integrado para Español, Inglés y Portugués.


## 📝 Comandos y Sintaxis
### `/trigger add <region|none> "<comandos>" <nombre> [start] [delay] [cooldown]`

- **region|none**: Nombre de la región de TShock o `none` para triggers exclusivamente manuales.
- **"comandos"**: Lista de comandos separados por `&&`. Ejemplo: `"/heal && /say Hola"`
- **nombre**: Identificador único del trigger.
- **start**: Tiempo de espera antes de iniciar el primer comando (Ej: `3s`).
- **delay**: Tiempo de espera entre cada comando de la cadena (Ej: `500ms`).
- **cooldown**: Tiempo que debe pasar antes de poder reactivar el trigger (Ej: `5m`).

### Otros Comandos:
- `/trigger list`: Muestra todos los triggers registrados.
- `/trigger delete <nombre>`: Elimina un trigger específico.
- `/trigger rename <nombre> <nuevoNombre>`: Cambia el nombre de un trigger.
- `/trigger run <nombre>`: Ejecuta un trigger manualmente (si el cooldown lo permite).
- `/trigger clear`: Borra toda la base de datos de triggers.


## 🔑 Permisos
- `trigger.permission`: Permite crear, borrar, renombrar, listar y limpiar triggers (Administración).
- `trigger.run`: Permite ejecutar triggers manualmente con `/trigger run` (Usuarios).


## 📂 Configuración y Persistencia
El plugin genera automáticamente dos archivos en la carpeta de TShock:
1. **`TR_Config.json`**: Configura el idioma (`es`, `en`, `pt`) y los tiempos por defecto.
2. **`TR_Triggers.json`**: Almacena todos tus triggers creados.

-----------

# ENGLISH

## 📖 Description (EN)
**Triggers&Regions** is an advanced plugin for **TShock 6 (Terraria 1.4.5)** servers designed to automate events. It allows the execution of command chains upon entering regions or via manual triggers, featuring precise timing control and an internal AI that corrects console-specific commands.

## ⚙️ Key Features
* **Flexible Timing System**: Supports units such as milliseconds (`ms`), seconds (`s`), and minutes (`m`).
* **Command AI**: Automatically fills in the player's name for commands like `/heal`, `/god`, `/kill`, `/buff`, etc.
* **Auto-Translation**: Converts in-game commands (`/i`, `/item`, `/spawnmob`, `/spawnboss`) so they function correctly when triggered by the server.
* **Internationalization**: Built-in support for English, Spanish, and Portuguese.


## 📝 Commands and Syntax
### `/trigger add <region|none> "<commands>" <name> [start] [delay] [cooldown]`

* **region|none**: The name of the TShock region or `none` for exclusively manual triggers.
* **"commands"**: A list of commands separated by `&&`. Example: `"/heal && /say Hello"`
* **name**: A unique identifier for the trigger.
* **start**: Delay before the first command is executed (e.g., `3s`).
* **delay**: Delay between each command in the chain (e.g., `500ms`).
* **cooldown**: Time required before the trigger can be activated again (e.g., `5m`).

### Other Commands:
* `/trigger list`: Displays all registered triggers.
* `/trigger delete <name>`: Deletes a specific trigger.
* `/trigger rename <name> <newName>`: Changes the name of an existing trigger.
* `/trigger run <name>`: Manually executes a trigger (if the cooldown allows).
* `/trigger clear`: Deletes the entire trigger database.


## 🔑 Permissions
* **`trigger.permission`**: Allows creating, deleting, renaming, listing, and clearing triggers (Administration).
* **`trigger.run`**: Allows manually executing triggers via `/trigger run` (Users).


## 📂 Configuration and Persistence
The plugin automatically generates two files in the TShock folder:
1.  **`TR_Config.json`**: Configures the language (`en`, `es`, `pt`) and default time values.
2.  **`TR_Triggers.json`**: Stores all of your created triggers.

