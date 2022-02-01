using System;
using System.Text;
using UnityEngine;

namespace XRayVision.Utilities
{
    static class HoverAdditions
    {
        public static bool HoverTextDisplay = true;

        public static string AddHoverText(GameObject obj, ref string __result)
        {
            StringBuilder stringBuilder = new();

            switch (XRayVisionPlugin.isAdmin)
            {
                /* Colored text examples https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html */
                case true when HoverTextDisplay:
                    stringBuilder.Append(
                        Localization.instance.Localize(
                            $"[<color=yellow><b>{XRayVisionPlugin._disableVisuals.Value}</b></color>] $piece_guardstone_deactivate XRayVision"));
                    if (XRayVisionPlugin._disableVisuals.Value.IsDown())
                    {
                        HoverTextDisplay = false;
                    }

                    try
                    {
                        ZNetView view = obj.GetComponent<ZNetView>();
                        stringBuilder.Append(
                            $"\n<color={XRayVisionPlugin._prefabNameColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Prefab Name{XRayVisionPlugin._rightSeperator.Value}  {(view ? view?.GetPrefabName() : obj.name)}</color>");
                        if (obj.GetComponent<Piece>())
                        {
                            stringBuilder.Append(
                                $"\n<color={XRayVisionPlugin._pieceNameColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Piece Name{XRayVisionPlugin._rightSeperator.Value}   {obj.GetComponent<Piece>().m_name}</color>");
                        }

                        stringBuilder
                            .Append(
                                $"\n<color={XRayVisionPlugin._createdColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Created{XRayVisionPlugin._rightSeperator.Value}  ")
                            .Append(
                                DateTimeOffset.Now
                                    .AddTicks(view.m_zdo.m_timeCreated -
                                              (long)(ZNet.instance.m_netTime * 10000000))
                                    .ToString("g"))
                            .Append("</color>");


                        if (view.m_zdo.m_longs
                                .TryGetValue("creator".GetStableHashCode(), out long creator) &&
                            view?.m_zdo.m_longs != null)
                        {
                            stringBuilder.Append(
                                $"\n<color={XRayVisionPlugin._creatorIDColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Creator ID{XRayVisionPlugin._rightSeperator.Value}  {creator}</color>");
                        }

                        if (view?.m_zdo.GetString("creatorName").Length > 1)
                            stringBuilder.Append(
                                $"\n<color={XRayVisionPlugin._creatorNameColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Creator Name{XRayVisionPlugin._rightSeperator.Value}  {view?.m_zdo.GetString("creatorName")}</color>");
                        if (view?.m_zdo.GetString("steamName").Length > 1)
                            stringBuilder.Append(
                                $"\n<color={XRayVisionPlugin._creatorSteamInfoColor.Value}>{XRayVisionPlugin._leftSeperator.Value}Creator Steam Info{XRayVisionPlugin._rightSeperator.Value}  {view?.m_zdo.GetString("steamName")} × {view?.m_zdo.GetString("steamID")}</color>");
                    }
                    catch
                    {
                        // Don't really care if this fails.
                    }

                    return __result += "\n\n" + stringBuilder;
                case true when !HoverTextDisplay:
                    stringBuilder.Append(
                        Localization.instance.Localize(
                            $"\n[<color=yellow><b>{XRayVisionPlugin._disableVisuals.Value}</b></color>] $piece_guardstone_activate XRayVision"));
                    if (XRayVisionPlugin._disableVisuals.Value.IsDown())
                    {
                        HoverTextDisplay = true;
                    }

                    return __result += "\n\n" + stringBuilder;
                default:
                    return __result;
            }
        }

        public static string AddPlayerHoverText(GameObject obj, ref string __result)
        {
            switch (XRayVisionPlugin.isAdmin)
            {
                /* Colored text examples https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html */
                case true when HoverTextDisplay:
                {
                    if (XRayVisionPlugin._disableVisuals.Value.IsDown())
                    {
                        HoverTextDisplay = false;
                    }

                    StringBuilder stringBuilder = new();
                    try
                    {
                        ZNetView view = obj.GetComponent<ZNetView>();
                        if (obj.GetComponent<Player>().IsCrouching())
                            stringBuilder.Append(
                                $"<color=#00afd4>{XRayVisionPlugin._leftSeperator.Value}Name{XRayVisionPlugin._rightSeperator.Value}  {(view ? obj.GetComponent<Player>().GetPlayerName() : obj.name)}</color>");
                        stringBuilder.Append(
                            $"\n<color=#00afd4>{XRayVisionPlugin._leftSeperator.Value}Player ID{XRayVisionPlugin._rightSeperator.Value}  {obj.GetComponent<Player>().GetPlayerID()}</color>");
                        stringBuilder
                            .Append(
                                $"\n<color=#95DBE5FF>{XRayVisionPlugin._leftSeperator.Value}Last Spawned{XRayVisionPlugin._rightSeperator.Value}  ")
                            .Append(
                                DateTimeOffset.Now
                                    .AddTicks(view.m_zdo.m_timeCreated -
                                              (long)(ZNet.instance.m_netTime * 10000000))
                                    .ToString("g"))
                            .Append("</color>");
                        if (view?.m_zdo.GetString("steamName").Length > 1)
                            stringBuilder.Append(
                                $"\n<color=#95DBE5FF>{XRayVisionPlugin._leftSeperator.Value}Steam Info{XRayVisionPlugin._rightSeperator.Value}  {view?.m_zdo.GetString("steamName")} × {view?.m_zdo.GetString("steamID")}</color>");
                    }
                    catch
                    {
                        // Don't really care if this fails.
                    }

                    return __result += "\n\n" + stringBuilder;
                }
                case true when !HoverTextDisplay:
                {
                    if (XRayVisionPlugin._disableVisuals.Value.IsDown())
                    {
                        HoverTextDisplay = true;
                    }

                    return __result;
                }
                default:
                    return __result;
            }
        }
    }
}