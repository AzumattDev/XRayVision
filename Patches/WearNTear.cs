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

    /*[HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Awake))]
    static class WearNTear_Awake_Patch
    {
        /* If the object was built before the mod was installed, or during it's removal,
         at least try to crab the creator name for the admin #1#
        static void Postfix(WearNTear __instance)
        {
            if (__instance.m_nview?.GetZDO().GetString("creatorName").Length < 2)
            {
                try
                {
                    if (!__instance.m_nview.GetZDO().m_longs
                            .TryGetValue("creator".GetStableHashCode(), out long creator) &&
                        __instance.m_nview?.GetZDO().m_longs != null)
                    {
                        string creatorNameVal;
                        if (XRayVisionPlugin.PList.TryGetValue(creator, out creatorNameVal))
                            __instance.m_nview?.GetZDO().Set("creatorName", creatorNameVal);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }*/
}