using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using Give_Healthshot_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using System;
using System.IO;
using System.Collections.Generic;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Give_Healthshot_GoldKingZ;


public class GiveHealthshotGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Healthshot Depend Players Kills";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh/cs2-Plugins-For-Community";

	public static GiveHealthshotGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;

        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

        RegisterEventHandler<EventRoundEnd>(OnEventRoundEnd);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

    }
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null || !g_Main.RoundStart) return HookResult.Continue;

        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (!victim.IsValid(false)) return HookResult.Continue;
        Helper.AddPlayerInGlobals(victim);
        if(g_Main.Player_Data.ContainsKey(victim))
        {
            g_Main.Player_Data[victim].Kills = 0;
        }


        if (!attacker.IsValid(false)) return HookResult.Continue;
        Helper.AddPlayerInGlobals(attacker);

        bool Check_teammates_are_enemies = ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false && attacker.TeamNum != victim.TeamNum || ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == true;
        if(!Check_teammates_are_enemies) return HookResult.Continue;

        if(g_Main.Player_Data.ContainsKey(attacker))
        {
            g_Main.Player_Data[attacker].Kills++;

            if(!Configs.GetConfigData().GiveHealthShotOnNewRound)
            {
                if(!Configs.GetConfigData().StackHealthShots && attacker.PlayerHasHealthShot())return HookResult.Continue;

                if( g_Main.Player_Data[attacker].Kills >= Configs.GetConfigData().GiveHealthShotIfxKills)
                {
                    g_Main.Player_Data[attacker].Player.GiveNamedItem(CsItem.Healthshot);
                    Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["PrintToChatToAll.Healtshot"], g_Main.Player_Data[attacker].Player.PlayerName, g_Main.Player_Data[attacker].Kills);
                    g_Main.Player_Data[attacker].Kills = 0;
                }
            }
        }

        
        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null || Helper.IsWarmup())return HookResult.Continue;
        g_Main.RoundStart = true;
        return HookResult.Continue;
    }

    private HookResult OnEventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid()) return HookResult.Continue;

        Helper.AddPlayerInGlobals(player);

        if(!Configs.GetConfigData().GiveHealthShotOnNewRound)return HookResult.Continue;
        if(!Configs.GetConfigData().StackHealthShots && player.PlayerHasHealthShot())return HookResult.Continue;

        Server.NextFrame( ()=> 
        {
            if (!player.IsValid() || !g_Main.Player_Data.ContainsKey(player)) return;
            if(!Configs.GetConfigData().StackHealthShots && player.PlayerHasHealthShot())return;
            

            if( g_Main.Player_Data[player].Kills >= Configs.GetConfigData().GiveHealthShotIfxKills)
            {
                g_Main.Player_Data[player].Player.GiveNamedItem(CsItem.Healthshot);
                Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["PrintToChatToAll.Healtshot"], g_Main.Player_Data[player].Player.PlayerName, g_Main.Player_Data[player].Kills);
                g_Main.Player_Data[player].Kills = 0;
            }
        });

        return HookResult.Continue;
    }

    private HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        g_Main.RoundStart = false;
        Helper.ResetAllIfNotPass();
        return HookResult.Continue;
    }

    
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid()) return HookResult.Continue;

        if (g_Main.Player_Data.ContainsKey(player))g_Main.Player_Data.Remove(player);
        

        return HookResult.Continue;
    }

    private void OnMapEnd()
    {        
        Helper.ClearVariables();
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }

    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!player.IsValid(true))return;

    } */

}