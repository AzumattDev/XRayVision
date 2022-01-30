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
            string? steamID = readLocalSteamID();
            if (__instance.m_nview.GetZDO() == null || __instance.GetPlayerID() != 0L)
                return;
            __instance.m_nview?.GetZDO()
                .Set("steamName", SteamFriends.GetPersonaName());
            __instance.m_nview?.GetZDO()
                .Set("steamID", steamID);
        }

        internal static string? readLocalSteamID() =>
            Type.GetType("Steamworks.SteamUser, assembly_steamworks")?.GetMethod("GetSteamID")!
                .Invoke(null, Array.Empty<object>()).ToString();
    }
}