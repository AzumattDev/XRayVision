using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XRayVision.Utilities
{
    static class HoverAdditions
    {
        public static bool HoverTextDisplay = false;

        public static string AddHoverText(GameObject gobj, ref string __result)
        {
            /* TODO: Clean up this mess */
            StringBuilder
                stringBuilder =
                    new(); // Mimic the way they add text in PrivateArea script. I'm the ward Guy after all :)
            StringBuilder cleanCopyBuilder = new(); // For the clean copy of the text
            GameObject obj = gobj.transform.root.gameObject; // Always get the top-most parent 
            ZNetView view = obj.GetComponent<ZNetView>();
            if (!XRayVisionPlugin.IsModerator && !XRayVisionPlugin.IsAdmin) return __result;
            switch (HoverTextDisplay)
            {
                case true:
                    XRayVisionPlugin.PropertiesText.ShowTooltip();
                    stringBuilder.Append(
                        Localization.instance.Localize(
                            $"[<color=yellow><b>{XRayVisionPlugin.DisableVisuals.Value}</b></color>] $piece_guardstone_deactivate XRayVision"));
                    if (XRayVisionPlugin.DisableVisuals.Value.IsUp() &&
                        XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.Off)
                    {
                        HoverTextDisplay = false;
                    }

                    if (XRayVisionPlugin.DisableVisuals.Value.IsDown() &&
                        XRayVisionPlugin.ToggleTooltip.Value == XRayVisionPlugin.Toggle.On)
                    {
                        HoverTextDisplay = false;
                    }

                    if (!view || !view.IsValid()) return __result += "\n\n" + stringBuilder;
                    stringBuilder.Append(
                        $"\n<color={XRayVisionPlugin.PrefabNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}</color>");
                    cleanCopyBuilder.Append(
                        $"{XRayVisionPlugin.LeftSeperator.Value}Prefab Name{XRayVisionPlugin.RightSeperator.Value}  {(view?.GetPrefabName())}");
                    if (obj.GetComponent<Piece>())
                    {
                        stringBuilder.Append(
                            $"\n<color={XRayVisionPlugin.PieceNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}</color>");
                        cleanCopyBuilder.Append(
                            $"\n{XRayVisionPlugin.LeftSeperator.Value}Piece Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<Piece>().m_name}");
                    }

                    if (obj.GetComponent<ItemDrop>())
                    {
                        stringBuilder.Append(
                            $"\n<color={XRayVisionPlugin.PieceNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}</color>");
                        cleanCopyBuilder.Append(
                            $"\n{XRayVisionPlugin.LeftSeperator.Value}ItemData Shared Name{XRayVisionPlugin.RightSeperator.Value}   {obj.GetComponent<ItemDrop>().m_itemData.m_shared.m_name}");
                    }

                    stringBuilder
                        .Append(
                            $"\n<color={XRayVisionPlugin.CreatedColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  ")
                        .Append(DateTimeOffset.Now
                            .AddTicks(view!.m_zdo.m_timeCreated - ZNet.instance.GetTime().Ticks)
                            .ToString("g"))
                        .Append("</color>");
                    cleanCopyBuilder.Append(
                        $"\n{XRayVisionPlugin.LeftSeperator.Value}Created{XRayVisionPlugin.RightSeperator.Value}  {DateTimeOffset.Now.AddTicks(view!.m_zdo.m_timeCreated - ZNet.instance.GetTime().Ticks).ToString("g")}");


                    if (view.m_zdo.m_longs != null)
                    {
                        stringBuilder.Append(
                            $"\n<color={XRayVisionPlugin.CreatorIDColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}</color>"); // Mimic piece component's grabbing of creator
                        cleanCopyBuilder.Append(
                            $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator ID{XRayVisionPlugin.RightSeperator.Value}  {view.GetZDO().GetLong("creator".GetStableHashCode())}");
                    }


                    if (view.m_zdo.GetString("creatorName").Length > 1)
                    {
                        stringBuilder.Append(
                            $"\n<color={XRayVisionPlugin.CreatorNameColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}</color>");
                        cleanCopyBuilder.Append(
                            $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Name{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("creatorName")}");
                    }

                    if (XRayVisionPlugin.ModeratorConfigs.ContainsKey(XRayVisionPlugin.ID))
                    {
                        bool? showSteamInformation =
                            XRayVisionPlugin.ModeratorConfigs[XRayVisionPlugin.ID].ShowSteamInformation;
                        if (showSteamInformation!.Value)
                        {
                            if (view.m_zdo.GetString("steamName").Length > 1)
                            {
                                stringBuilder.Append(
                                    $"\n<color={XRayVisionPlugin.CreatorSteamInfoColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}</color>");
                                cleanCopyBuilder.Append(
                                    $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}");
                            }
                        }
                    }
                    else if (XRayVisionPlugin.IsAdmin)
                    {
                        if (view.m_zdo.GetString("steamName").Length > 1)
                        {
                            stringBuilder.Append(
                                $"\n<color={XRayVisionPlugin.CreatorSteamInfoColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}</color>");
                            cleanCopyBuilder.Append(
                                $"\n{XRayVisionPlugin.LeftSeperator.Value}Creator Steam Info{XRayVisionPlugin.RightSeperator.Value}  {view.m_zdo.GetString("steamName")} × {view.m_zdo.GetString("steamID")}");
                        }
                    }

                    stringBuilder.Append(
                        $"\n<color={XRayVisionPlugin.OwnerColor.Value}>{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}</color>");
                    cleanCopyBuilder.Append(
                        $"\n{XRayVisionPlugin.LeftSeperator.Value}Owner{XRayVisionPlugin.RightSeperator.Value}  {GetOwnerText(view)}");
                    XRayVisionPlugin.CleanCopy = cleanCopyBuilder.ToString();
                    return __result = stringBuilder.ToString();

                case false:
                    switch (XRayVisionPlugin.ToggleTooltip.Value)
                    {
                        case XRayVisionPlugin.Toggle.On when
                            XRayVisionPlugin.DisableVisuals.Value.IsDown():
                        case XRayVisionPlugin.Toggle.Off when
                            XRayVisionPlugin.DisableVisuals.Value.IsPressed():
                            HoverTextDisplay = true;
                            break;
                    }

                    XRayVisionPlugin.PropertiesText.HideTooltip();
                    XRayVisionPlugin.PropertiesText.Set("NOT Hovering", "NOTHING");
                    XRayVisionPlugin.CleanCopy = cleanCopyBuilder.ToString();
                    return stringBuilder.ToString();
            }
        }

        private static string GetOwnerText(ZNetView view)
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