using CounterStrikeSharp.API.Core;
using System.Diagnostics;
using System.Numerics;
using CounterStrikeSharp.API;

namespace Give_Healthshot_GoldKingZ;

public class Globals
{
    public bool RoundStart = false;
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public int Kills { get; set; }
        public PlayerDataClass(CCSPlayerController player, int kills)
        {
            Player = player;
            Kills = kills;
        }
    }
    public Dictionary<CCSPlayerController, PlayerDataClass> Player_Data = new Dictionary<CCSPlayerController, PlayerDataClass>();
}