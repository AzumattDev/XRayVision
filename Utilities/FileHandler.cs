using System.IO;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.Rendering;

namespace XRayVision.Utilities
{
    public class FileHandler
    {
        internal static void ModeratorListCreate()
        {
            //if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) return;

            XRayVisionPlugin.ModeratorPermsConfigPath =
                Paths.ConfigPath + Path.DirectorySeparatorChar + XRayVisionPlugin.ModbypassFileName;
            if (!File.Exists(Paths.ConfigPath + Path.DirectorySeparatorChar + XRayVisionPlugin.ModbypassFileName))
            {
                using StreamWriter streamWriter =
                    File.CreateText(Paths.ConfigPath + Path.DirectorySeparatorChar +
                                    XRayVisionPlugin.ModbypassFileName);
                streamWriter.Write(new StringBuilder()
                    .AppendLine(
                        "# Configure your per Moderator permissions in this file. Currently there is only one, but we can add more as the suggestions are made.")
                    .AppendLine(
                        "# Fill out more moderators just like the example given. This uses their Steam64ID as the key.")
                    .AppendLine("")
                    .AppendLine("12345678998765432:")
                    .AppendLine(
                        "  Steam Display: false  # When set to true, moderators can see the user's steam information.")
                    .AppendLine(""));
                streamWriter.Close();
            }
        }
    }
}