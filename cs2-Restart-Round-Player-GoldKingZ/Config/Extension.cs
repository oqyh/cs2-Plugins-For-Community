using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


namespace Restart_Round_Player_GoldKingZ;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }

    public static bool IsAlive(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid
        || player.PlayerPawn == null || !player.PlayerPawn.IsValid
        || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid) return false;

        return player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }
}