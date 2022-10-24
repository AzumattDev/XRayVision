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
            if (!__instance.m_nview.IsValid() || __instance.m_nview == null) return;
            if (__instance.gameObject.GetComponent<Hoverable>() == null)
            {
                __instance.gameObject.AddComponent<HoverText>();
            }

            string? steamID = readLocalSteamID();
            if (__instance == null) return;
            __instance.m_nview?.GetZDO().Set("creatorName", Player.m_localPlayer.GetPlayerName());
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