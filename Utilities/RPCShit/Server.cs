using System;
using HarmonyLib;

namespace XRayVision.Utilities.RPCShit
{
    [HarmonyPatch]
    public class XRayServer
    {
        /// <summary>
        ///     All requests, put in order to the corresponding events. These are "sent" to the server
        /// </summary>
        public static void RPC_XRayRequestAdminSync(long sender, ZPackage pkg)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (peer != null)
            {
                string peerSteamID = peer.m_rpc.GetSocket().GetHostName();
                if (!ZNet.instance.IsDedicated() && ZNet.instance.IsServer())
                {
                    XRayVisionPlugin.IsAdmin = true;
                    XRayVisionPlugin.XRayLogger.LogDebug(
                        $"Local Play Detected setting Admin: {XRayVisionPlugin.IsAdmin}");
                }


                if (ZNet.instance.m_adminList != null &&
                    ZNet.instance.ListContainsId(ZNet.instance.m_adminList, peerSteamID))
                {
                    XRayVisionPlugin.XRayLogger.LogInfo($"Admin Detected: {peer.m_playerName} ({peerSteamID})");
                    pkg.Write(peerSteamID);
                    pkg.Write("Admin");
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "XRayEventAdminSync", pkg);
                }

                // Check if the user is found inside the server's moderator file
                XRayVisionPlugin.ReadYamlConfigFile(null!, null!);
                if (XRayVisionPlugin.ModeratorConfigs.ContainsKey(peerSteamID))
                {
                    pkg.Write(peerSteamID);
                    pkg.Write("Moderator");
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "XRayEventAdminSync", pkg);
                }
            }
            else
            {
                ZPackage zpackage = new();
                zpackage.Write("You aren't an Admin!");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "XRayBadRequestMsg", zpackage);
            }
        }

        /// <summary>
        ///     All events, put in order to the corresponding requests. These are "received" from the server
        ///     put logic here that you want to happen on the client AFTER getting the information from the server.
        /// </summary>
        public static void RPC_XRayEventAdminSync(long sender, ZPackage pkg)
        {
        }
    }

    [HarmonyPatch]
    public class ServerRPC_Registrations
    {
        [HarmonyPatch(typeof(Game), nameof(Game.Start))]
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (!ZNet.m_isServer) return;
            ZRoutedRpc.instance.Register("XRayRequestAdminSync",
                new Action<long, ZPackage>(XRayServer.RPC_XRayRequestAdminSync));
            ZRoutedRpc.instance.Register("XRayEventAdminSync",
                new Action<long, ZPackage>(XRayServer.RPC_XRayEventAdminSync));
        }
    }
}