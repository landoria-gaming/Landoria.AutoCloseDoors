# AutoClose Doors

Automatically closes player-built doors you open when no player is nearby. Other doors are ignored.

- Auto-close can be disabled for each door (Alt+E by default).
- The choice is saved per character.

## Configuration

Edit these settings in the BepInEx configuration file. Changes reload while the game runs.

| Setting | Default | Description |
| --- | --- | --- |
| `General.EnabledByDefault` | `true` | Enables auto-close for doors without a character-specific modifier+E choice. |
| `General.CloseDistanceMeters` | `5` | Closes a tracked door when no player is within this distance in meters. |
| `General.ToggleModifier` | Alt | Modifier for toggling a door's auto-close choice: Alt, Shift, or Ctrl. |

## Demo

![Auto-close demonstration](assets/autoclose.gif)

## Contact

Report bugs through [GitHub Issues](https://github.com/landoria-gaming/Landoria.AutoCloseDoors/issues).
