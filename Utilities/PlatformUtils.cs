namespace XRayVision
{
    public static class PlatformUtils
    {
        public static PrivilegeManager.Platform GetPlatform(string hostname)
        {
            return PrivilegeManager.ParseUser(hostname).platform;
        }

        public static PrivilegeManager.Platform GetPlatform(ZNetPeer netPeer)
        {
            return GetPlatform(netPeer.m_socket.GetHostName());
        }
    }
}
