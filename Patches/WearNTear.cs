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

            string steamName;
            string steamID;

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

            // For backwards compatibility and WardIsLove (and various other mods of mine) I can't change the name of the variables that are saved in the ZDO
            __instance.m_nview.GetZDO().Set("creatorName", Player.m_localPlayer.GetPlayerName());
            __instance.m_nview.GetZDO().Set("steamID", steamID);
            __instance.m_nview.GetZDO().Set("steamName", steamName);
        }
    }
}