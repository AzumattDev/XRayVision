using HarmonyLib;
using XRayVision.Utilities;

namespace XRayVision.Patches
{
    /* Patches are Alphabetical. Easier to find in this hunk of mess */
    [HarmonyPatch(typeof(Bed), nameof(Bed.GetHoverText))]
    public class Bed_AddHover
    {
        public static void Postfix(Bed __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Beehive), nameof(Beehive.GetHoverText))]
    public class Beehive_AddHover
    {
        public static void Postfix(Beehive __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Chair), nameof(Chair.GetHoverText))]
    public class Chair_AddHover
    {
        public static void Postfix(Chair __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
    public class Container_AddHover
    {
        public static void Postfix(Container __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(CookingStation), nameof(CookingStation.GetHoverText))]
    public class CookingStation_AddHover
    {
        public static void Postfix(CookingStation __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.GetHoverText))]
    public class CraftingStation_AddHover
    {
        public static void Postfix(CraftingStation __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Awake))]
    public class Destructible_Awake_AddHover
    {
        public static void Postfix(Destructible __instance)
        {
            if (__instance.gameObject.GetComponent<Hoverable>() == null)
            {
                __instance.gameObject.AddComponent<HoverText>();
            }
        }
    }

    [HarmonyPatch(typeof(Door), nameof(Door.GetHoverText))]
    public class Door_AddHover
    {
        public static void Postfix(Door __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Fermenter), nameof(Fermenter.GetHoverText))]
    public class Fermenter_AddHover
    {
        public static void Postfix(Fermenter __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Fireplace), nameof(Fireplace.GetHoverText))]
    public class Fireplace_AddHover
    {
        public static void Postfix(Fireplace __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(HoverText), nameof(HoverText.GetHoverText))]
    public class HoverText_AddHover
    {
        public static void Postfix(HoverText __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.GetHoverText))]
    public class ItemDrop_AddHover
    {
        public static void Postfix(Beehive __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.GetHoverText))]
    public class ItemStand_AddHover
    {
        public static void Postfix(ItemStand __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(MineRock), nameof(MineRock.GetHoverText))]
    public class MineRock_AddHover
    {
        public static void Postfix(MineRock __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.GetHoverText))]
    public class MineRock5_AddHover
    {
        public static void Postfix(MineRock5 __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.GetHoverText))]
    public class Pickable_AddHover
    {
        public static void Postfix(Pickable __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.GetHoverText))]
    public class Plant_AddHover
    {
        public static void Postfix(Plant __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetHoverText))]
    static class Player_AddHover_Patch
    {
        static void Postfix(Player __instance, ref string __result) =>
            HoverAdditions.AddPlayerHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(PrivateArea), nameof(PrivateArea.GetHoverText))]
    public class PrivateArea_AddHover
    {
        public static void Postfix(PrivateArea __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Sign), nameof(Sign.GetHoverText))]
    public class Sign_AddHover
    {
        public static void Postfix(Sign __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.UpdateHoverTexts))]
    public partial class HoverTextUtils
    {
        private static void Switches(Smelter instance, string text)
        {
            Switch? oreSwitch = instance.m_addOreSwitch;
            Switch? woodSwitch = instance.m_addWoodSwitch;
            Switch? emptySwitch = instance.m_emptyOreSwitch;
            if (oreSwitch) oreSwitch.m_hoverText += text;
            if (woodSwitch) woodSwitch.m_hoverText += text;
            if (emptySwitch) emptySwitch.m_hoverText += text;
        }

        public static void Postfix(Smelter __instance)
        {
            string text = "";
            HoverAdditions.AddHoverText(__instance.gameObject, ref text);
            Switches(__instance, text);
        }
    }

    [HarmonyPatch(typeof(StationExtension), nameof(StationExtension.GetHoverText))]
    public class StationExtension_AddHover
    {
        public static void Postfix(StationExtension __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(Vagon), nameof(Vagon.GetHoverText))]
    public class Vagon_AddHover
    {
        public static void Postfix(Vagon __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }

    [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverText))]
    public class TeleportWorld_AddHover
    {
        public static void Postfix(TeleportWorld __instance, ref string __result) =>
            HoverAdditions.AddHoverText(__instance.gameObject, ref __result);
    }


    [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Awake))]
    public class WearNTear_Awake_AddHover
    {
        public static void Postfix(WearNTear __instance)
        {
            if (__instance.gameObject.GetComponent<Hoverable>() == null)
            {
                __instance.gameObject.AddComponent<HoverText>();
            }
        }
    }

    /*[HarmonyPatch(typeof(Hud), nameof(Hud.UpdateCrosshair))]
    static class Hud_UpdateCrosshair_Patch
    {
        static void Postfix(Hud __instance, Player player, float bowDrawPercentage)
        {
            try
            {
                GameObject hoverObject = player.GetHoverObject();
                Piece hoveringPiece = player.GetHoveringPiece();
                Hoverable? hoverable = hoverObject
                    ? hoverObject.GetComponentInParent<Hoverable>()
                    : null;
                if (XRayVisionPlugin.WardIsLoveAssembly != null && hoverable != null && !TextViewer.instance.IsVisible())
                {
                    string hoverText = hoverable.GetHoverText();
                    XRayVisionPlugin.WardIsLoveAssembly.GetType("WardIsLove.Util.WardMonoscript")
                        .GetMethod("GetHoverText", BindingFlags.Public).Invoke(null,
                            new[] { HoverAdditions.AddHoverText(hoveringPiece.gameObject, ref hoverText) });
                }
            }
            catch
            {
                // ignored
            }
        }
    }*/
}