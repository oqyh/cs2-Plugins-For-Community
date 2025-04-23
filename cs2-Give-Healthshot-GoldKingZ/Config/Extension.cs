using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Modules.Entities.Constants;


namespace Give_Healthshot_GoldKingZ;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool excludebot = true, bool excludeHLTV = true)
    {
        if (player == null || !player.IsValid)
            return false;

        if (excludebot && player.IsBot)
            return false;
            
        if (excludeHLTV && player.IsHLTV)
            return false;

        return true;
    }

    public static bool PlayerHasHealthShot(this CCSPlayerController player)
    {
        if(!player.IsValid())return false;

        var playerPawn = player.PlayerPawn;
        if (playerPawn == null || !playerPawn.IsValid) return false;

        var playerPawnValue = playerPawn.Value;
        if (playerPawnValue == null || !playerPawnValue.IsValid) return false;

        var playerWeaponServices = playerPawnValue.WeaponServices;
        if (playerWeaponServices == null) return false;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return false;

        foreach (var weaponInventory in playerMyWeapons)
        {
            if (weaponInventory == null || !weaponInventory.IsValid) continue;

            var weaponInventoryValue = weaponInventory.Value;
            if (weaponInventoryValue == null || !weaponInventoryValue.IsValid) continue;

            var weaponDesignerName = weaponInventoryValue.DesignerName;
            if (weaponDesignerName == null || string.IsNullOrEmpty(weaponDesignerName)) continue;

            if (weaponDesignerName.Contains("weapon_healthshot"))
            {
                return true;
            }
        }
        
        return false;
    }
}