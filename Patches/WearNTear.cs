using System;
using HarmonyLib;
using Steamworks;
namespace XRayVision.Patches
{
    [HarmonyPatch]
    public class WearNTearPatches
    {
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.OnPlaced))]
        [HarmonyPrefix]
        private static void Prefix(WearNTear __instance)
        {
            if (!__instance || !__instance.m_nview || !__instance.m_nview.IsValid()) return;
            if (__instance.gameObject.GetComponent<Hoverable>() == null)
            {
                __instance.gameObject.AddComponent<HoverText>();
            }

            ZNetPeer netPeer = ZNet.instance.GetServerPeer();

            __instance.m_nview.GetZDO().Set("creatorName", Player.m_localPlayer.GetPlayerName());
            __instance.m_nview.GetZDO().Set("steamID", netPeer.m_socket.GetHostName());

            if (PlatformUtils.GetPlatform(netPeer) == PrivilegeManager.Platform.Steam)
                __instance.m_nview.GetZDO().Set("steamName", SteamFriends.GetPersonaName());
        }
    }
}
