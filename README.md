# AutoClose Doors

Automatically closes player-built doors you open when no player is nearby. Other doors are ignored.

- Auto-close can be disabled for each door (Shift+E on the door).
- The choice is saved per character.

## Configuration

Edit these settings in the BepInEx configuration file. Changes reload while the game runs.

| Setting | Default | Description |
| --- | --- | --- |
| `General.EnabledByDefault` | `true` | Enables auto-close for doors without a character-specific Shift+E choice. |
| `General.CloseDistanceMeters` | `5` | Closes a tracked door when no player is within this distance in meters. |

## Contact

Report bugs through [GitHub Issues](https://github.com/landoria-gaming/Landoria.AutoCloseDoors/issues).
