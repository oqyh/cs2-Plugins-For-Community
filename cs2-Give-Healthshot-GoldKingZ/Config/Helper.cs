using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using Give_Healthshot_GoldKingZ.Config;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.Text.Json;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Give_Healthshot_GoldKingZ;

public class Helper
{
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }

    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid && 
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
                        p.Connected == PlayerConnectedState.PlayerConnected && 
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string output = prefix ? $"[Give HealthShot]: {message}" : message;
        Console.WriteLine(output);
        
        Console.ResetColor();
    }
    
    public static void ClearVariables()
    {
        var g_Main = GiveHealthshotGoldKingZ.Instance.g_Main;
        g_Main.Player_Data.Clear();
    }

    public static CCSGameRules? GetGameRules()
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

    public static void AddPlayerInGlobals(CCSPlayerController player)
    {
        if (!player.IsValid())return;
        var g_Main = GiveHealthshotGoldKingZ.Instance.g_Main;

        if(!g_Main.Player_Data.ContainsKey(player))
        {
            g_Main.Player_Data.Add(player, new Globals.PlayerDataClass(player, 0));
        }
    }

    public static void ResetAllIfNotPass()
    {
        foreach(var players in GetPlayersController())
        {
            if(!players.IsValid())continue;
            if(!GiveHealthshotGoldKingZ.Instance.g_Main.Player_Data.ContainsKey(players))continue;
            
            if(GiveHealthshotGoldKingZ.Instance.g_Main.Player_Data[players].Kills < Configs.GetConfigData().GiveHealthShotIfxKills)
            {
                GiveHealthshotGoldKingZ.Instance.g_Main.Player_Data[players].Kills = 0;
            }
        }
    }
}