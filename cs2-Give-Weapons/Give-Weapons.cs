using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Utils;

namespace Give_Weapons;

public class GiveWeapons : BasePlugin
{
    public override string ModuleName => "Give Weapons Teams";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	public static string SMapName => NativeAPI.GetMapName();
    private CustomGameData? CustomFunctions { get; set; }
    public override void Load(bool hotReload)
    {
        CustomFunctions = new CustomGameData();
        string jsonFilePath = $"{ModuleDirectory}/../../plugins/Give-Weapons/Weapons/Weapons.json";
        string weaponfolder = $"{ModuleDirectory}/../../plugins/Give-Weapons/Weapons/";
        if(!Directory.Exists(weaponfolder))
        {
            Directory.CreateDirectory(weaponfolder);
        }
    
        CreateDefaultWeaponsJson(jsonFilePath);
        
        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Pre);
    }
    
    private HookResult OnEventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;
        
        string mapname = SMapName;
        string jsonFilePath = $"{ModuleDirectory}/../../plugins/Give-Weapons/Weapons/Weapons.json";

        if (!File.Exists(jsonFilePath))
        {
            return HookResult.Continue;
        }

        string jsonString = File.ReadAllText(jsonFilePath);
        dynamic jsonData = JsonConvert.DeserializeObject(jsonString)!;
        if (jsonData!.ContainsKey(mapname))
        {
            dynamic mapData = jsonData[mapname];

            if (player.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                string[] weaponsCT;
                weaponsCT = mapData["CT"].ToString().Split(',');
                foreach (string weapon in weaponsCT)
                {
                    if (string.IsNullOrWhiteSpace(weapon))
                    {
                        continue;
                    }
                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);
                }
                
            }else if (player.TeamNum == (byte)CsTeam.Terrorist)
            {
                string[] weaponsT;
                weaponsT = mapData["T"].ToString().Split(',');
                foreach (string weapon in weaponsT)
                {
                    if (string.IsNullOrWhiteSpace(weapon))
                    {
                        continue;
                    }
                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);
                }
            }
            
        }

        return HookResult.Continue;
    }

    private void CreateDefaultWeaponsJson(string jsonFilePath)
    {
        string defaultJson = @"
        {
            ""de_dust2"": {
                ""CT"": ""weapon_m4a1,weapon_hkp2000"",
                ""T"": ""weapon_ak47,weapon_deagle""
            },
            ""1v1_hospital"": {
                ""CT"": ""weapon_m4a1,weapon_hkp2000"",
                ""T"": ""weapon_ak47,weapon_deagle""
            }
        }";

        if (!File.Exists(jsonFilePath))
        {
            using (StreamWriter sw = File.CreateText(jsonFilePath))
            {
                sw.Write(defaultJson);
            }
        }
    }
}