using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Entities;
using System.Linq;

namespace DeathmatchWeaponMenuPlugin.Menu
{
    public abstract class BaseWeaponMenu
    {
        protected string Title;
        protected WeaponType? FilterType;
        protected Dictionary<string, Weapon> Weapons;
        protected Dictionary<string, Weapon> WeaponCheckers;

        public BaseWeaponMenu(string title, WeaponType? filterType = null)
        {
            Title = title;
            FilterType = filterType;
            var weaponData = WeaponHelper.LoadWeapons();
            Weapons = weaponData.Weapons
                .Where(w => filterType == null || w.Value.Type == filterType)
                .ToDictionary(w => w.Key, w => w.Value);
            WeaponCheckers = weaponData.WeaponCheckers;
        }

        public void Show(CCSPlayerController player, Action nextMenuAction = null)
        {
            var chatMenu = new ChatMenu($"| ===== {Title} ===== |");
            foreach (var weapon in Weapons)
            {
                string weaponKey = weapon.Key;
                string formattedOption = $"* {weaponKey} *";

                chatMenu.AddMenuOption(formattedOption, (playerController, menuOption) =>
                {
                    GiveSelectedItem(playerController, weaponKey, nextMenuAction);
                });
            }
            ChatMenus.OpenMenu(player, chatMenu);
        }

        private void GiveSelectedItem(CCSPlayerController player, string weaponKey, Action nextMenuAction)
        {
            if (player == null || !player.IsValid || player.IsBot || player.PlayerPawn?.Value?.WeaponServices?.MyWeapons == null)
            {
                return;
            }

            if (Weapons.TryGetValue(weaponKey, out var selectedWeapon))
            {
                RemoveCurrentWeapon(player, selectedWeapon);
                player.GiveNamedItem(selectedWeapon.GiveName);

                nextMenuAction?.Invoke();
            }
        }

        private void RemoveCurrentWeapon(CCSPlayerController player, Weapon selectedWeapon)
        {
            foreach (var weapon in player.PlayerPawn!.Value!.WeaponServices!.MyWeapons)
            {
                if (weapon.Value != null &&
                    !string.IsNullOrWhiteSpace(weapon.Value.DesignerName) &&
                    weapon.Value.DesignerName != "[null]" &&
                    WeaponCheckers.TryGetValue(weapon.Value.DesignerName, out var currentWeapon))
                {
                    if (currentWeapon.Type == selectedWeapon.Type)
                    {
                        weapon.Value.Remove();
                    }
                }
            }
        }
    }

    public class Weapon
    {
        public Weapon(string giveName, WeaponType type = WeaponType.Primary)
        {
            GiveName = giveName;
            Type = type;
        }

        public string GiveName { get; set; }
        public WeaponType Type { get; set; }
    }

    public enum WeaponType
    {
        Primary,
        Secondary
    }

    public static class WeaponHelper
    {
        public static (Dictionary<string, Weapon> Weapons, Dictionary<string, Weapon> WeaponCheckers) LoadWeapons()
        {
            var weapons = new Dictionary<string, Weapon>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"AK-47", new Weapon("weapon_ak47")},
                {"M4A4", new Weapon("weapon_m4a1")},
                {"M4A1-S", new Weapon("weapon_m4a1_silencer")},
                {"AWP", new Weapon("weapon_awp")},
                {"SSG 08", new Weapon("weapon_ssg08")},
                {"Galil AR", new Weapon("weapon_galilar")},
                {"FAMAS", new Weapon("weapon_famas")},
                {"SG 553", new Weapon("weapon_sg553")},
                {"AUG", new Weapon("weapon_aug")},
                {"G3SG1", new Weapon("weapon_g3sg1")},
                {"SCAR-20", new Weapon("weapon_scar20")},
                {"Glock-18", new Weapon("weapon_glock", WeaponType.Secondary)},
                {"P2000", new Weapon("weapon_hkp2000", WeaponType.Secondary)},
                {"USP-S", new Weapon("weapon_usp_silencer", WeaponType.Secondary)},
                {"Tec-9", new Weapon("weapon_tec9", WeaponType.Secondary)},
                {"P250", new Weapon("weapon_p250", WeaponType.Secondary)},
                {"CZ75-Auto", new Weapon("weapon_cz75a", WeaponType.Secondary)},
                {"Dual Berettas", new Weapon("weapon_elite", WeaponType.Secondary)},
                {"Five-SeveN", new Weapon("weapon_fiveseven", WeaponType.Secondary)},
                {"R8 Revolver", new Weapon("weapon_revolver", WeaponType.Secondary)},
            };

            var weaponCheckers = weapons.ToDictionary(kvp => kvp.Value.GiveName, kvp => kvp.Value);
            return (weapons, weaponCheckers);
        }
    }
}
