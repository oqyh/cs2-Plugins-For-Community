using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using Restart_Round_Player_GoldKingZ.Config;

namespace Restart_Round_Player_GoldKingZ;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Restart Round When 1 Player Alive In CT/T";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh/cs2-Plugins-For-Community";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);

        RegisterEventHandler<EventPlayerDeath>(OnEventPlayerDeath);
    }

    public HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        if(Helper.IsWarmup())return HookResult.Continue;

        var players = Helper.GetPlayersController(
        IncludeBots: Configs.GetConfigData().IncludeBotsAsCouting,
        IncludeHLTV: false,
        IncludeNone: false,
        IncludeSPEC: false,
        IncludeCT: true,
        IncludeT: true);

        var lastCT = players.FirstOrDefault(p => p.IsAlive() && p.Team == CsTeam.CounterTerrorist);
        var lastT = players.FirstOrDefault(p => p.IsAlive() && p.Team == CsTeam.Terrorist);

        int ctAlive = players.Count(p => p.IsAlive() && p.Team == CsTeam.CounterTerrorist);
        int tAlive = players.Count(p => p.IsAlive() && p.Team == CsTeam.Terrorist);

        if (ctAlive == 1 && lastCT.IsValid(Configs.GetConfigData().IncludeBotsAsCouting))
        {
            ExcuteRestart(lastT);
        }
        else if (tAlive == 1 && lastT.IsValid(Configs.GetConfigData().IncludeBotsAsCouting))
        {
            ExcuteRestart(lastT);
        }

        return HookResult.Continue;
    }

    public void ExcuteRestart(CCSPlayerController? player)
    {
        if (!player.IsValid(Configs.GetConfigData().IncludeBotsAsCouting)) return;

        string teamName = player.Team == CsTeam.CounterTerrorist
        ? Localizer["PrintChatToAll.CT.Team"]
        : Localizer["PrintChatToAll.T.Team"];

        if (!string.IsNullOrEmpty(Configs.GetConfigData().Execute_Cfg))
        {
            Server.ExecuteCommand("execifexists " + Configs.GetConfigData().Execute_Cfg);
        }
        else
        {
            Server.ExecuteCommand("mp_restartgame 1");
        }

        Server.PrintToChatAll(Localizer["PrintChatToAll.Restart", player.PlayerName, teamName]);
    }
    public void OnMapEnd()
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
        if (!player.IsValid())return;
    } */
}