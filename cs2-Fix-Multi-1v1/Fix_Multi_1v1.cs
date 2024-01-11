using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Fix_Multi_1v1;

public class BlockRCommands : BasePlugin
{
    public override string ModuleName => "Fix Arena";
    public override string ModuleVersion => "1.0.1";
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
                Server.NextFrame(() =>
                {
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand("resetarenas");
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand("css_rq");
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand("css_endround");
                    });
                });
            }
        });

        RegisterListener<Listeners.OnClientDisconnectPost>(playerSlot =>
        {
            var players = Utilities.GetPlayers().Where(x => x.Connected == PlayerConnectedState.PlayerConnected && !x.IsBot);
            var playersCount = players.Count();
            if(playersCount > 0 || playersCount < 3)
            {
                Server.NextFrame(() =>
                {
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand("resetarenas");
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand("css_rq");
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand("css_endround");
                    });
                });
            }
        });
    }
}
