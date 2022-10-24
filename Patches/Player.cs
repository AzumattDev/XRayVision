using System;
using HarmonyLib;
using Steamworks;
using XRayVision.Utilities;

namespace XRayVision.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetPlayerID))]
    static class PlayerXraySetPlayerIDPatch
    {
        static void Postfix(Player __instance)
        {
            if (!ZNet.instance) return;
            ZNetPeer netPeer;
            string steamName = string.Empty;
            string steamID = string.Empty;
            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                netPeer = ZNet.instance.GetPeer(__instance.m_nview.m_zdo.m_uid.m_userID);

                if (PlatformUtils.GetPlatform(netPeer) == PrivilegeManager.Platform.Steam)
                {
                    CSteamID SteamID = new(ulong.Parse(netPeer.m_rpc.GetSocket().GetHostName()));
                    steamName = SteamFriends.GetFriendPersonaName(SteamID);
                    steamID = SteamID.ToString();
                }
            }
            else
            {
                if (PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam)
                {
                    steamName = SteamFriends.GetPersonaName();
                    steamID = SteamUser.GetSteamID().ToString();
                }
                else
                {
                    steamName = "";
                    steamID = PrivilegeManager.GetNetworkUserId();
                }
            }

            // For backwards compatibility and WardIsLove (and various other mods of mine) I can't change the name of the variables that are saved in the ZDO
            __instance.m_nview.GetZDO().Set("steamName", steamName);
            __instance.m_nview.GetZDO().Set("steamID", steamID);
        }
    }
}