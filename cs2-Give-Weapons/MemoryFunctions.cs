using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace Give_Weapons;

public class CustomGameData
{
    private static Dictionary<string, Dictionary<OSPlatform, string>> _customGameData = new()
    {
        {
            "GiveNamedItem2",
            new()
            {
                {
                    OSPlatform.Linux,
                    @"\x55\x48\x89\xE5\x41\x57\x41\x56\x41\x55\x41\x54\x53\x48\x83\xEC\x18\x48\x89\x7D\xC8\x48\x85\xF6\x74"
                },
                {
                    OSPlatform.Windows,
                    @"\x48\x83\xEC\x38\x48\xC7\x44\x24\x28\x00\x00\x00\x00\x45\x33\xC9\x45\x33\xC0\xC6\x44\x24\x20\x00\xE8\x2A\x2A\x2A\x2A\x48\x85"
                }
            }
        }
    };

    private readonly MemoryFunctionVoid<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>? GiveNamedItem2;

    public CustomGameData()
    {
        var giveNamedItemData = GetCustomGameDataKey("GiveNamedItem2");
        if (giveNamedItemData is not null)
        {
            GiveNamedItem2 = new(giveNamedItemData);
        }
    }

    private string? GetCustomGameDataKey(string key)
    {
        if (!_customGameData.TryGetValue(key, out var customGameData))
        {
            return null;
        }

        OSPlatform platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platform = OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platform = OSPlatform.Windows;
        }
        else
        {
            throw new Exception("Unsupported platform");
        }

        return customGameData.TryGetValue(platform, out var customData) ? customData : null;
    }

    public void PlayerGiveNamedItem(CCSPlayerController player, string item)
    {
        if (!player.PlayerPawn.IsValid) return;
        if (player.PlayerPawn.Value == null) return;
        if (!player.PlayerPawn.Value.IsValid) return;
        if (player.PlayerPawn.Value.ItemServices == null) return;

        if (GiveNamedItem2 is not null)
        {
            GiveNamedItem2.Invoke(player.PlayerPawn.Value.ItemServices.Handle, item, 0, 0, 0, 0, 0, 0);
        }
        else
        {
            player.GiveNamedItem(item);
        }
    }
}