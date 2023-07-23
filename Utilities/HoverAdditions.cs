using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XRayVision.Patches;

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
            if (!needItCleanJack)
            {
                stringBuilder.Append(GetVisualButtonText());
                stringBuilder.Append(GetCopyButtonText());
            }

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
            if (ZDOExtraData.s_ints.TryGetValue(tuple.Item1.m_zdo.m_uid, out var ints) && ints.Count > 0)
            {
                stringBuilder.Append(GetCreatorString(tuple.Item1, needItCleanJack));
            }

            if (tuple.Item1.m_zdo.GetString(ZDOVars.s_creatorName).Length > 1)
            {
                stringBuilder.Append(GetCreatorNameString(tuple.Item1, needItCleanJack));
            }

            if (XRayVisionPlugin.ModeratorConfigs.ContainsKey(XRayVisionPlugin.ID))
            {
                bool? showSteamInformation =
                    XRayVisionPlugin.ModeratorConfigs[XRayVisionPlugin.ID].ShowSteamInformation;
                if (showSteamInformation!.Value)
                {
                    if (tuple.Item1.m_zdo.GetString("steamName").Length > 1 ||
                        tuple.Item1.m_zdo.GetString("steamID").Length > 1)
                    {
                        stringBuilder.Append(GetSteamInfoString(tuple.Item1, needItCleanJack));
                    }
                }
            }
            else if (XRayVisionPlugin.IsAdmin || XRayVisionPlugin.configSync.IsAdmin)
            {
                if (tuple.Item1.m_zdo.GetString("steamName").Length > 1 ||
                    tuple.Item1.m_zdo.GetString("steamID").Length > 1)
                {
                    stringBuilder.Append(GetSteamInfoString(tuple.Item1, needItCleanJack));
                }
            }

            stringBuilder.Append(GetZdoOwnerText(tuple.Item1, needItCleanJack));
            stringBuilder.Append(GetGameObjectsComponents(tuple.Item2, needItCleanJack));

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
                        XRayVisionPlugin.XRayLogger.LogDebug(
                            "XRayVision: HoverTextDisplay is true, attempting to turn off visuals");
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
                        XRayVisionPlugin.XRayLogger.LogDebug(
                            "XRayVision: HoverTextDisplay is false, attempting to turn on visuals");
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

        private static string GetCopyButtonText()
        {
            return XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.On
                ? Localization.instance.Localize(
                    $"\n[<color=yellow><b>{XRayVisionPlugin.CopyHotkey.Value}</b></color>] Copy tooltip contents to clipboard")
                : Localization.instance.Localize(
                    $"\n Hide tooltip. While hovering object, press [<color=yellow><b>{XRayVisionPlugin.CopyHotkey.Value}</b></color>] to copy tooltip contents to clipboard");
        }

        private static string GetPrefabString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.PrefabNameColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}</color>";
        }

        private static string GetPieceString(GameObject obj, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.PieceNameColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}</color>";
        }

        private static string GetItemString(GameObject obj, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}"
                : $"\n<color={XRayVisionPlugin.PieceNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}</color>";
        }

        private static string GetCreationString(ZNetView view, bool clean = false)
        {
            long storedTicks = view!.m_zdo.GetLong("xray_created");

            if (storedTicks < DateTimeOffset.MinValue.Ticks || storedTicks > DateTimeOffset.MaxValue.Ticks)
            {
                return $"Invalid timestamp: {storedTicks}";
            }

            DateTimeOffset storedDateTimeOffset = new DateTimeOffset(storedTicks, TimeSpan.Zero);
            DateTimeOffset localDateTimeOffset = storedDateTimeOffset.ToLocalTime();
            string formattedDateTime = localDateTimeOffset.ToString("MM/dd/yyyy h:mm tt");

            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  {formattedDateTime}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatedColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  {formattedDateTime}</color>";
        }

        private static string GetCreatorString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorIDColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}</color>"; // Mimic piece component's grabbing of creator
        }

        private static string GetCreatorNameString(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorNameColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}</color>";
        }

        private static string GetZdoOwnerText(ZNetView view, bool clean = false)
        {
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.OwnerColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}</color>";
        }

        private static string GetOwnerText(ZNetView view, bool clean = false)
        {
            ZNet.PlayerInfo? owner = ZNet.instance.m_players.Where(i => i.m_characterID.UserID == view.m_zdo.GetOwner())
                .Cast<ZNet.PlayerInfo?>().FirstOrDefault();

            if (owner == null)
            {
                return "-";
            }

            string ownerIsMe = view.IsOwner() ? "(Me)" : "";
            return $"{owner?.m_name}, {view.m_zdo.GetOwner()} {ownerIsMe}";
        }

        private static string GetSteamInfoString(ZNetView view, bool clean = false)
        {
            if (view.m_zdo.GetString("steamName").Length > 1)
                return clean
                    ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}"
                    : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Creator Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}</color>";
            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamID")}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Creator Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamID")}</color>";
        }

        private static string GetGameObjectsComponents(GameObject obj, bool clean = false)
        {
            Component[] components = obj.GetComponents<Component>();
            StringBuilder stringBuilder = new();
            foreach (Component component in components)
            {
                stringBuilder.Append($"\t\t{component.GetType().Name}{Environment.NewLine}");
            }

            return clean
                ? $"\n{XRayVisionPlugin.LeftSeperator.Value}Components{XRayVisionPlugin.RightSeperator.Value}  {Environment.NewLine} {stringBuilder}"
                : $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Components{XRayVisionPlugin.RightSeperator.Value}  {Environment.NewLine}{stringBuilder}</color>";
        }

        public static string AddPlayerHoverText(GameObject gobj, ref string __result)
        {
            switch (XRayVisionPlugin.IsAdmin || XRayVisionPlugin.configSync.IsAdmin)
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
                                $"<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorNameColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Name{XRayVisionPlugin.RightSeperator.Value}  {(view ? obj.GetComponent<Player>().GetPlayerName() : obj.name)}</color>");
                        stringBuilder.Append(
                            $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorNameColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Player ID{XRayVisionPlugin.RightSeperator.Value}  {obj.GetComponent<Player>().GetPlayerID()}</color>");
                        stringBuilder
                            .Append(
                                $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Last Spawned{XRayVisionPlugin.RightSeperator.Value}  ")
                            .Append(
                                DateTimeOffset.Now
                                    .AddTicks(ZDOExtraData.GetTimeCreated(view.m_zdo.m_uid) - ZNet.instance.GetTime().Ticks)
                                    .ToString("g"))
                            .Append("</color>");
                        if (view.m_zdo.GetString("steamName").Length >= 1)
                            stringBuilder.Append(
                                $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Player Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}</color>");
                        else if (view.m_zdo.GetString("steamID").Length >= 1)
                            stringBuilder.Append(
                                $"\n<color=#{ColorUtility.ToHtmlStringRGBA(XRayVisionPlugin.CreatorSteamInfoColor.Value)}>{XRayVisionPlugin.LeftSeperator.Value}Player Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamID")}</color>");
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