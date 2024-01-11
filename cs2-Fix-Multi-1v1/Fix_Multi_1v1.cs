using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Fix_Multi_1v1;

public class BlockRCommands : BasePlugin
{
    public override string ModuleName => "Fix Arena";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Fix Arena";

    
    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnClientConnected>(playerSlot =>
        {
            var players = Utilities.GetPlayers().Where(x => x.Connected == PlayerConnectedState.PlayerConnected && !x.IsBot);
            var playersCount = players.Count();
            if(playersCount > 0 || playersCount < 3)
            {
                Server.ExecuteCommand("css_resetarenas; css_endround");
            }
            
        });
        RegisterListener<Listeners.OnClientDisconnectPost>(playerSlot =>
        {
            var players = Utilities.GetPlayers().Where(x => x.Connected == PlayerConnectedState.PlayerConnected && !x.IsBot);
            var playersCount = players.Count();
            if(playersCount > 0 || playersCount < 3)
            {
                Server.ExecuteCommand("css_resetarenas; css_endround");
            }
        });
    }
}
