using System;
using HarmonyLib;

namespace XRayVision.Utilities.RPCShit
{
    [HarmonyPatch]
    public class XRayClient
    {
        /// <summary>
        ///     All requests, put in order to the corresponding events. These are "sent" to the server
        /// </summary>
        public static void RPC_XRayRequestAdminSync(long sender, ZPackage pkg)
        {
        }

        public static void RPC_XRayBadRequestMsg(long sender, ZPackage pkg)
        {
            if (sender != ZRoutedRpc.instance.GetServerPeerID() || pkg == null || pkg.Size() <= 0)
                return;
            string str = pkg.ReadString();
            if (str == "")
                return;
            Chat.m_instance.AddString("Server", "<color=\"red\">" + str + "</color>", Talker.Type.Normal);
        }

        /// <summary>
        ///     All events, put in order to the corresponding requests. These are "received" from the server
        ///     put logic here that you want to happen on the client AFTER getting the information from the server.
        /// </summary>
        public static void RPC_XRayEventAdminSync(long sender, ZPackage pkg)
        {
            XRayVisionPlugin.XRayLogger.LogInfo("This account is an admin.");
            Chat.m_instance.AddString("[XRay]", "<color=\"green\">" + "Admin permissions synced" + "</color>",
                Talker.Type.Normal);
            XRayVisionPlugin.isAdmin = true;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        public static class PlayerOnSpawnPatch
        {
            private static void Prefix()
            {
                if (!ZNet.instance.IsDedicated() && ZNet.instance.IsServer())
                {
                    XRayVisionPlugin.isAdmin = true;

                    XRayVisionPlugin.XRayLogger.LogInfo(
                        $"Local Play Detected setting Admin: {XRayVisionPlugin.isAdmin}");
                }

                if (ZRoutedRpc.instance == null || !ZNetScene.instance)
                    return;
                if (!XRayVisionPlugin.isAdmin)
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "XRayRequestAdminSync",
                        new ZPackage());
                }
            }
        }
    }

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
    internal static class FejdStartup_Start_Patch
    {
        private static void Postfix()
        {
            Console.SetConsoleEnabled(true);
        }
    }

    [HarmonyPatch]
    internal static class ClientResetPatches
    {
        [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Error))]
        private static void Prefix()
        {
            if (XRayVisionPlugin.isAdmin)
            {
                XRayVisionPlugin.isAdmin = false;
                XRayVisionPlugin.XRayLogger.LogDebug($"Admin Status changed to: {XRayVisionPlugin.isAdmin}");
            }
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    static class ClientRPC_Registrations
    {
        static void Prefix(Game __instance)
        {
            if (ZNet.m_isServer) return;
            ZRoutedRpc.instance.Register("XRayRequestAdminSync",
                new Action<long, ZPackage>(XRayClient.RPC_XRayRequestAdminSync));
            ZRoutedRpc.instance.Register("XRayEventAdminSync",
                new Action<long, ZPackage>(XRayClient.RPC_XRayEventAdminSync));
            ZRoutedRpc.instance.Register("XRayBadRequestMsg",
                new Action<long, ZPackage>(XRayClient.RPC_XRayBadRequestMsg));
        }
    }
}