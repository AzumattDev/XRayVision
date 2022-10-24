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
                    /*__instance.m_hoverName.text =
                        HoverAdditions.AddHoverText(hoverObject, ref result);*/
                    XRayVisionPlugin.PropertiesText.Set(
                        (hoverObject.transform.root.gameObject.GetComponent<ZNetView>()?.GetPrefabName() is null
                            ? ""
                            : hoverObject.transform.root.gameObject.GetComponent<ZNetView>()?.GetPrefabName())!,
                        HoverAdditions.AddHoverText(hoverObject, ref result));

                    if (XRayVisionPlugin.CopyHotkey.Value.IsDown() ||
                        XRayVisionPlugin.CopyHotkey.Value.IsPressed() &&
                        XRayVisionPlugin.DisableVisuals.Value.IsPressed() ||
                        XRayVisionPlugin.CopyHotkey.Value.IsDown() && XRayVisionPlugin.DisableVisuals.Value.IsPressed())
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Copied to clipboard", 0, null);
                        GUIUtility.systemCopyBuffer = XRayVisionPlugin.CleanCopy;
                    }
                }
            }
            else if (hoverObject)
            {
                if (hoverObject.transform.root.gameObject.GetComponent<Piece>())
                    hoverObject.AddComponent<HoverText>();
            }
            else
            {
                XRayVisionPlugin.PropertiesText.HideTooltip();
                XRayVisionPlugin.PropertiesText.Set("NOT Hovering", "NOTHING");
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetHoverText))]
    static class Player_AddHover_Patch
    {
        static void Postfix(Player __instance, ref string __result) =>
            HoverAdditions.AddPlayerHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    static class HudAwakePatch
    {
        static void Postfix(Hud __instance)
        {
            XRayVisionPlugin.ToolTipGameObject = Object.Instantiate(XRayVisionPlugin.BorrowedTooltip,
                __instance.m_crosshair.transform);
            XRayVisionPlugin.ToolTipGameObject.name = "XRayVisionProperties";
            XRayVisionPlugin.PropertiesText = XRayVisionPlugin.ToolTipGameObject.GetComponent<XRayProps>();
            XRayVisionPlugin.PropertiesText.textcomp.fontSize = XRayVisionPlugin.ToolTipTextSize.Value;
            XRayVisionPlugin.PropertiesText.titlecomp.fontSize = XRayVisionPlugin.ToolTipTitleSize.Value;

            RectTransform transform = XRayVisionPlugin.ToolTipGameObject.GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.zero;
            transform.pivot = Vector2.one;
            transform.anchoredPosition = new Vector2(-75f, 0f);
        }
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.OnDestroy))]
    static class Hud_OnDestroy_Patch
    {
        static void Prefix(ref Hud __instance)
        {
            if (XRayVisionPlugin.ToolTipGameObject)
            {
                Object.DestroyImmediate(XRayVisionPlugin.ToolTipGameObject);
            }
        }
    }
}