using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace XRayVision.Patches;

[HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.ConvertCreationTime))]
public class ConvertCreationTime
{
    static void Postfix(List<ZDO> zdos)
    {
        if (!ZDOExtraData.HasTimeCreated()) return;
        foreach (ZDO zdo in zdos)
        {
            //long timeCreated = ZDOExtraData.GetTimeCreated(zdo.m_uid);
            zdo.SetOwner(ZDOMan.GetSessionID());
            zdo.Set(CreateNewZDO.Hash, DateTimeOffset.UtcNow.Ticks);
        }
    }
}

[HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.CreateNewZDO), typeof(ZDOID), typeof(Vector3), typeof(int))]
public class CreateNewZDO
{
    public static int Hash = "xray_created".GetHashCode();
    static ZDO Postfix(ZDO result)
    {
        result.Set(Hash, DateTimeOffset.UtcNow.Ticks);
        return result;
    }
}