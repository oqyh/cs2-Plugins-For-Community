using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using Restart_Round_Player_GoldKingZ.Config;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Entities;

namespace Restart_Round_Player_GoldKingZ;

public class Helper
{
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        return Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => 
                p != null && 
                p.IsValid &&
                p.Connected == PlayerConnectedState.PlayerConnected &&
                (IncludeBots || !p.IsBot) &&
                (IncludeHLTV || !p.IsHLTV) &&
                ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                (IncludeNone && p.TeamNum == (byte)CsTeam.None) || 
                (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();
    }
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch (Exception ex)
        {
            DebugMessage(ex.Message);
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void ClearVariables()
    {
        var g_Main = MainPlugin.Instance.g_Main;
        g_Main.StartRestart = false;
    }
    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string output = prefix ? $"[Restart Round Player]: {message}" : message;
        Console.WriteLine(output);
        
        Console.ResetColor();
    }
}