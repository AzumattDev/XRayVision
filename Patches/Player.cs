using System;
using HarmonyLib;
using Steamworks;

namespace XRayVision.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetPlayerID))]
    static class Player_XRAYSetPlayerID_Patch
    {
        static void Postfix(Player __instance)
        {
            if (!ZNet.instance || !__instance.m_nview || __instance.m_nview.GetZDO() == null) return;

            ZNetPeer netPeer;
            string steamName = string.Empty;

            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                netPeer = ZNet.instance.GetPeer(__instance.m_nview.m_zdo.m_uid.m_userID);

                if (PlatformUtils.GetPlatform(netPeer) == PrivilegeManager.Platform.Steam)
                {
                    CSteamID steamID = new(ulong.Parse(netPeer.m_rpc.GetSocket().GetHostName()));
                    steamName = SteamFriends.GetFriendPersonaName(steamID);
                }
            }
            else
            {
                netPeer = ZNet.instance.GetServerPeer();

                if (PlatformUtils.GetPlatform(netPeer) == PrivilegeManager.Platform.Steam)
                    steamName = SteamFriends.GetPersonaName();
            }

            __instance.m_nview.GetZDO().Set("steamName", steamName);
            __instance.m_nview.GetZDO().Set("steamID", netPeer.m_socket.GetHostName());
        }
    }
}
