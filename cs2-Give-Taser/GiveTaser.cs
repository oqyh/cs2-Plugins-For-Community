using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace Give_Taser;

public class GiveTaserConfig : BasePluginConfig
{
    [JsonPropertyName("OneTaserPerRound")] public bool OneTaserPerRound { get; set; } = true;
    [JsonPropertyName("CommandsGiveTaser")] public string CommandsGiveTaser { get; set; } = "!zeus,!taser";
    [JsonPropertyName("CommandsGiveTaserGroupsToggle")] public string CommandsGiveTaserGroupsToggle { get; set; } = "";
}

public class GiveTaser : BasePlugin, IPluginConfig<GiveTaserConfig> 
{
    public override string ModuleName => "Give Taser , VIPs";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh/cs2-Plugins-For-Community";
    internal static IStringLocalizer? Stringlocalizer;
    public GiveTaserConfig Config { get; set; } = new GiveTaserConfig();
    private Dictionary<ulong, bool> PlayerIsVIP = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> TaserGived = new Dictionary<ulong, bool>();
	
    

    public void OnConfigParsed(GiveTaserConfig config)
    {
        Config = config;
        Stringlocalizer = Localizer;
    }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        AddCommandListener("say", OnPlayerSayPublic, HookMode.Post);
        AddCommandListener("say_team", OnPlayerSayTeam, HookMode.Post);
    }
    private HookResult OnPlayerSayPublic(CCSPlayerController? player, CommandInfo info)
    {
        if (string.IsNullOrEmpty(Config.CommandsGiveTaser) || player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        var playerid = player.SteamID;
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        
        string[] Commands = Config.CommandsGiveTaser.Split(',');
        
        if (Commands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Config.CommandsGiveTaserGroupsToggle) && !PlayerIsVIP.ContainsKey(playerid))
            {
                if (!string.IsNullOrEmpty(Localizer["VIP.Only"]))
                {
                    player.PrintToChat(Localizer["VIP.Only"]);
                }
                return HookResult.Continue;
            }

            if (Config.OneTaserPerRound && TaserGived.ContainsKey(playerid))
            {
                if (!string.IsNullOrEmpty(Localizer["Item.PerRoundOnly"]))
                {
                    player.PrintToChat(Localizer["Item.PerRoundOnly"]);
                }
                return HookResult.Continue;
            }
            
            if (Config.OneTaserPerRound && !TaserGived.ContainsKey(playerid))
            {
                TaserGived.Add(playerid, true);
            }

            if (!string.IsNullOrEmpty(Localizer["Item.Gived"]))
            {
                player.PrintToChat(Localizer["Item.Gived"]);
            }
            player.GiveNamedItem("weapon_taser");
        }
        return HookResult.Continue;
    }
    private HookResult OnPlayerSayTeam(CCSPlayerController? player, CommandInfo info)
    {
        if (string.IsNullOrEmpty(Config.CommandsGiveTaser) || player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        var playerid = player.SteamID;
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        
        string[] Commands = Config.CommandsGiveTaser.Split(',');
        
        if (Commands.Any(cmd => cmd.Equals(trimmedMessage, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Config.CommandsGiveTaserGroupsToggle) && !PlayerIsVIP.ContainsKey(playerid))
            {
                if (!string.IsNullOrEmpty(Localizer["VIP.Only"]))
                {
                    player.PrintToChat(Localizer["VIP.Only"]);
                }
                return HookResult.Continue;
            }

            if (Config.OneTaserPerRound && TaserGived.ContainsKey(playerid))
            {
                if (!string.IsNullOrEmpty(Localizer["Item.PerRoundOnly"]))
                {
                    player.PrintToChat(Localizer["Item.PerRoundOnly"]);
                }
                return HookResult.Continue;
            }
            
            if (Config.OneTaserPerRound && !TaserGived.ContainsKey(playerid))
            {
                TaserGived.Add(playerid, true);
            }

            if (!string.IsNullOrEmpty(Localizer["Item.Gived"]))
            {
                player.PrintToChat(Localizer["Item.Gived"]);
            }
            player.GiveNamedItem("weapon_taser");
        }
        return HookResult.Continue;
    }
    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (string.IsNullOrEmpty(Config.CommandsGiveTaserGroupsToggle) || @event == null)return HookResult.Continue;
        var player = @event.Userid;
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;
        if(IsPlayerInGroupPermission(player))
        {
            if (!PlayerIsVIP.ContainsKey(playerid))
            {
                PlayerIsVIP.Add(playerid, true);
            }
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (!Config.OneTaserPerRound || @event == null) return HookResult.Continue;

        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
        foreach (var player in playerEntities)
        {
            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)continue;
            var playerid = player.SteamID;
            if (TaserGived.ContainsKey(playerid))
            {
                TaserGived.Remove(playerid);
            }
        }

        return HookResult.Continue;
    }
    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (!Config.OneTaserPerRound || @event == null) return HookResult.Continue;
        var player = @event.Userid;
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        var playerid = player.SteamID;

        if (PlayerIsVIP.ContainsKey(playerid))
        {
            PlayerIsVIP.Remove(playerid);
        }
        if (TaserGived.ContainsKey(playerid))
        {
            TaserGived.Remove(playerid);
        }

        return HookResult.Continue;
    }
    private bool IsPlayerInGroupPermission(CCSPlayerController player)
    {
        string[] excludedGroups = Config.CommandsGiveTaserGroupsToggle.Split(',');
        foreach (string group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                {
                    return true;
                }
            }else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnMapEnd()
    {
        PlayerIsVIP.Clear();
        TaserGived.Clear();
    }

    public override void Unload(bool hotReload)
    {
        PlayerIsVIP.Clear();
        TaserGived.Clear();
    }
    
}