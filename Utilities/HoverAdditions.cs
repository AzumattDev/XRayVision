using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XRayVision.Utilities
{
    static class HoverAdditions
    {
        public static bool HoverTextDisplay;

        public static string AddHoverText(GameObject gobj, ref string __result, bool needItCleanJack = false)
        {
            StringBuilder
                stringBuilder =
                    new(); // Mimic the way they add text in PrivateArea script. I'm the ward Guy after all :)
            Tuple<ZNetView, GameObject> tuple = GetZNetView(gobj);
            if (!XRayVisionPlugin.IsModerator && !XRayVisionPlugin.IsAdmin) return __result;
            if(!needItCleanJack){stringBuilder.Append(GetVisualButtonText());}

            if (!tuple.Item1 || !tuple.Item1.IsValid()) return __result += "\n\n" + stringBuilder;
            stringBuilder.Append(GetPrefabString(tuple.Item1, needItCleanJack));
            if (tuple.Item2.GetComponent<Piece>())
            {
                stringBuilder.Append(GetPieceString(tuple.Item2, needItCleanJack));
            }

            if (tuple.Item2.GetComponent<ItemDrop>())
            {
                stringBuilder.Append(GetItemString(tuple.Item2, needItCleanJack));
            }

            stringBuilder.Append(GetCreationString(tuple.Item1, needItCleanJack));
            if (tuple.Item1.m_zdo.m_longs != null)
            {
                stringBuilder.Append(GetCreatorString(tuple.Item1, needItCleanJack));
            }

            if (tuple.Item1.m_zdo.GetString("creatorName").Length > 1)
            {
                stringBuilder.Append(GetCreatorNameString(tuple.Item1, needItCleanJack));
            }

            if (XRayVisionPlugin.ModeratorConfigs.ContainsKey(XRayVisionPlugin.ID))
            {
                bool? showSteamInformation =
                    XRayVisionPlugin.ModeratorConfigs[XRayVisionPlugin.ID].ShowSteamInformation;
                if (showSteamInformation!.Value)
                {
                    if (tuple.Item1.m_zdo.GetString("steamName").Length > 1)
                    {
                        stringBuilder.Append(GetSteamInfoString(tuple.Item1, needItCleanJack));
                    }
                }
            }
            else if (XRayVisionPlugin.IsAdmin)
            {
                if (tuple.Item1.m_zdo.GetString("steamName").Length > 1)
                {
                    stringBuilder.Append(GetSteamInfoString(tuple.Item1, needItCleanJack));
                }
            }

            stringBuilder.Append(GetZdoOwnerText(tuple.Item1, needItCleanJack));

            switch (HoverTextDisplay)
            {
                case true:
                    XRayVisionPlugin.PropertiesText.ShowTooltip();
                    if (XRayVisionPlugin.DisableVisuals.Value.IsUp() &&
                        XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.Off)
                    {
                        HoverTextDisplay = false;
                    }

                    if (XRayVisionPlugin.DisableVisuals.Value.IsDown() &&
                        XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.On)
                    {
                        XRayVisionPlugin.XRayLogger.LogError("XRayVision: HoverTextDisplay is true, attempting to turn off visuals");
                        HoverTextDisplay = false;
                    }

                    if (needItCleanJack)
                    {
                        XRayVisionPlugin.CleanCopy = stringBuilder.ToString();
                    }

                    return __result = stringBuilder.ToString();

                case false:
                    if ((XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.On &&
                         XRayVisionPlugin.DisableVisuals.Value.IsDown()) ||
                        (XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.Off &&
                         XRayVisionPlugin.DisableVisuals.Value.IsPressed()))
                    {
                        XRayVisionPlugin.XRayLogger.LogError("XRayVision: HoverTextDisplay is false, attempting to turn on visuals");
                        HoverTextDisplay = true;
                    }
                    else
                    {
                        XRayVisionPlugin.PropertiesText.HideTooltip();
                        XRayVisionPlugin.PropertiesText.Set("NOT Hovering", "NOTHING");
                        if (needItCleanJack)
                        {
                            XRayVisionPlugin.CleanCopy = stringBuilder.ToString();
                        }
                    }

                    return stringBuilder.ToString();
            }
        }

        private static Tuple<ZNetView, GameObject> GetZNetView(GameObject gobj)
        {
            GameObject obj = gobj.transform.root.gameObject;
            ZNetView view = obj.GetComponent<ZNetView>();
            return Tuple.Create(view, obj);
        }

        private static string GetVisualButtonText()
        {
            return XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.On
                ? Localization.instance.Localize(
                    $"[<color=yellow><b>{XRayVisionPlugin.DisableVisuals.Value}</b></color>] $piece_guardstone_deactivate XRayVision")
                : Localization.instance.Localize(
                    $"Release [<color=yellow><b>{XRayVisionPlugin.DisableVisuals.Value}</b></color>] to hide tooltip");
        }

        private static string GetPrefabString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}"
                : $"\n<color={XRayVisionPlugin.PrefabNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}</color>";
        }

        private static string GetPieceString(GameObject obj, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}"
                : $"\n<color={XRayVisionPlugin.PieceNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}</color>";
        }

        private static string GetItemString(GameObject obj, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}"
                : $"\n<color={XRayVisionPlugin.PieceNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}</color>";
        }

        private static string GetCreationString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  {DateTimeOffset.Now.AddTicks(view!.m_zdo.m_timeCreated - ZNet.instance.GetTime().Ticks):g}"
                : $"\n<color={XRayVisionPlugin.CreatedColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  {DateTimeOffset.Now.AddTicks(view!.m_zdo.m_timeCreated - ZNet.instance.GetTime().Ticks):g}</color>";
        }

        private static string GetCreatorString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}"
                : $"\n<color={XRayVisionPlugin.CreatorIDColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}</color>"; // Mimic piece component's grabbing of creator
        }

        private static string GetCreatorNameString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}"
                : $"\n<color={XRayVisionPlugin.CreatorNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}</color>";
        }

        private static string GetZdoOwnerText(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}"
                : $"\n<color={XRayVisionPlugin.OwnerColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}</color>";
        }

        private static string GetOwnerText(ZNetView view, bool clean = false)
        {
            ZNet.PlayerInfo? owner = ZNet.instance.m_players.Where(i => i.m_characterID.userID == view.m_zdo.m_owner)
                .Cast<ZNet.PlayerInfo?>().FirstOrDefault();

            if (owner == null)
            {
                return "-";
            }

            string ownerIsMe = view.IsOwner() ? "(Me)" : "";
            return $"{owner?.m_name}, {view.m_zdo.m_owner} {ownerIsMe}";
        }

        private static string GetSteamInfoString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}"
                : $"\n<color={XRayVisionPlugin.CreatorSteamInfoColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}</color>";
        }

        public static string AddPlayerHoverText(GameObject gobj, ref string __result)
        {
            switch (XRayVisionPlugin.IsAdmin)
            {
                /* Colored text examples https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html */
                case true when HoverTextDisplay:
                {
                    if (XRayVisionPlugin.DisableVisuals.Value.IsUp())
                    {
                        HoverTextDisplay = false;
                    }

                    StringBuilder stringBuilder = new();
                    GameObject obj = gobj.transform.root.gameObject;
                    try
                    {
                        ZNetView view = obj.GetComponent<ZNetView>();
                        if (obj.GetComponent<Player>()
                            .IsCrouching()) // If they are crouching, I still want to see the name.
                            stringBuilder.Append(
                                $"<color=#00afd4>{XRayVisionPlugin.LeftSeperator.Value}Name{XRayVisionPlugin.RightSeperator.Value}  {(view ? obj.GetComponent<Player>().GetPlayerName() : obj.name)}</color>");
                        stringBuilder.Append(
                            $"\n<color=#00afd4>{XRayVisionPlugin.LeftSeperator.Value}Player ID{XRayVisionPlugin.RightSeperator.Value}  {obj.GetComponent<Player>().GetPlayerID()}</color>");
                        stringBuilder
                            .Append(
                                $"\n<color=#95DBE5FF>{XRayVisionPlugin.LeftSeperator.Value}Last Spawned{XRayVisionPlugin.RightSeperator.Value}  ")
                            .Append(
                                DateTimeOffset.Now
                                    .AddTicks(view.m_zdo.m_timeCreated - ZNet.instance.GetTime().Ticks)
                                    .ToString("g"))
                            .Append("</color>");
                        if (view?.m_zdo.GetString("steamName").Length > 1)
                            stringBuilder.Append(
                                $"\n<color=#95DBE5FF>{XRayVisionPlugin.LeftSeperator.Value}Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view?.m_zdo.GetString("steamName")} × {view?.m_zdo.GetString("steamID")}</color>");
                    }
                    catch
                    {
                        // Don't really care if this fails.
                    }

                    return __result += "\n\n" + stringBuilder;
                }
                case true when !HoverTextDisplay:
                {
                    if (XRayVisionPlugin.DisableVisuals.Value.IsPressed())
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