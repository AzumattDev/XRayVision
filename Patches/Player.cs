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
            if (!ZNet.instance) return;
            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                ZNetPeer? netPeer = ZNet.instance.GetPeer(__instance.m_nview.m_zdo.m_uid.m_userID);

                CSteamID SteamID = new(ulong.Parse(netPeer.m_rpc.GetSocket().GetHostName()));
                __instance.m_nview?.GetZDO()
                    .Set("steamName", SteamFriends.GetFriendPersonaName(SteamID));
                __instance.m_nview?.GetZDO()
                    .Set("steamID", netPeer.m_rpc.GetSocket().GetHostName());
            }
            else
            {
                string? steamID = readLocalSteamID();
                if (__instance.m_nview.GetZDO() == null)
                    return;
                __instance.m_nview?.GetZDO()
                    .Set("steamName", SteamFriends.GetPersonaName());
                __instance.m_nview?.GetZDO()
                    .Set("steamID", steamID);
            }
        }

        internal static string? readLocalSteamID() =>
            Type.GetType("Steamworks.SteamUser, assembly_steamworks")?.GetMethod("GetSteamID")!
                .Invoke(null, Array.Empty<object>()).ToString();
    }
}