using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Commands;
using DeathmatchWeaponMenuPlugin.Menu;

namespace DeathmatchWeaponMenuPlugin
{
    public class DeathmatchWeaponMenuPlugin : BasePlugin
    {
        public override string ModuleName => "DeathmatchWeaponMenu";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "ArtemkaNord";
        public override string ModuleDescription => "A weapon menu for deathmatch mode, type !guns or !weapons for gun selection.";

        public override void Load(bool hotReload)
        {
            // Load logic if necessary
        }

        [ConsoleCommand("weapons")]
        [ConsoleCommand("guns")]
        public void ShowRifleMenu(CCSPlayerController player, CommandInfo info)
        {
            if (!ValidatePlayer(player)) return;

            var rifleMenu = new RifleMenu();
            rifleMenu.Show(player, () => ShowPistolMenu(player, info));
        }

        [ConsoleCommand("pistols")]
        public void ShowPistolMenu(CCSPlayerController player, CommandInfo info)
        {
            if (!ValidatePlayer(player)) return;

            var pistolMenu = new PistolMenu();
            pistolMenu.Show(player);
        }

        private bool ValidatePlayer(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.IsBot || !player.PawnIsAlive)
            {
                player?.PrintToChat("You must be alive to use this command.");
                return false;
            }
            return true;
        }
    }
}
