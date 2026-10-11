# AutoClose Doors

Version: 1.0.0

Automatically closes locally opened wood doors, wood gates, and darkwood gates.

- Auto-close is enabled by default. Change `General.EnabledByDefault` in the BepInEx configuration file to change the default.
- Press Shift+E while looking at a supported door to enable or disable it for the current character, overriding the configured default.
- The choice is saved with the character for that door in the current world.
- The default delay is 5 seconds. Change `General.CloseDelaySeconds` in the BepInEx configuration file; edits to either setting reload while the game runs.
- Install only on the client. Door state still uses Valheim's normal network interaction, so other players see the closure.
- The timer starts when the local player opens a door. Opening the door again starts a new timer; closing it manually cancels the timer.

Build with `dotnet build` after setting `VALHEIM_PATH` to the game root and `BEPINEX_PATH` to the selected profile's BepInEx directory. Copy the built DLL into that profile's `BepInEx/plugins` directory, then restart the game.
