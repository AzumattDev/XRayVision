using HarmonyLib;
using UnityEngine;
using XRayVision.Utilities;

namespace XRayVision.Patches
{
    /* Didn't know I could do this shit this easy. Removed 90% of the code thanks to KG's and Margmas's suggestion to patch UpdateCrosshair instead */
    [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateCrosshair))]
    static class Hud_UpdateCrosshair_Patch
    {
        static void Postfix(Hud __instance, Player player, float bowDrawPercentage)
        {
            GameObject hoverObject = player.GetHoverObject();
            Hoverable? hoverable = (bool)(Object)hoverObject
                ? hoverObject.GetComponentInParent<Hoverable>()
                : null;
            if (hoverable != null && !TextViewer.instance.IsVisible())
            {
                GameObject
                    root = hoverObject.transform.root
                        .gameObject; // Found that you can always get the top-most parent by asking for root.
                string result = __instance.m_hoverName.text;
                if (root.GetComponent<Player>()) // Not sure this works lol
                {
                    HoverAdditions.AddPlayerHoverText(hoverObject, ref result);
                }
                else
                {
                    __instance.m_hoverName.text =
                        HoverAdditions.AddHoverText(hoverObject, ref result);
                }
            }
            else if (hoverObject)
            {
                if (hoverObject.transform.root.gameObject.GetComponent<Piece>())
                    hoverObject.AddComponent<HoverText>();
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetHoverText))]
    static class Player_AddHover_Patch
    {
        static void Postfix(Player __instance, ref string __result) =>
            HoverAdditions.AddPlayerHoverText(__instance.gameObject, ref __result);
    }
}