using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;
using UnityEngine;

namespace XRayVision
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class XRayVisionPlugin : BaseUnityPlugin
    {
        public const string ModVersion = "1.2.0";
        public const string ModName = "XRayVision";
        internal const string Author = "Azumatt";
        private const string ModGuid = "azumatt.XRayVision";
        private static string ConfigFileName = ModGuid + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static bool isAdmin = false;
        internal static string ConnectionError = "";
        internal static Dictionary<long, string> PList = new();

        internal static Assembly? WardIsLoveAssembly;

        private Harmony? _harmony;
        internal static readonly ManualLogSource XRayLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync? configSync = new(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        private void Awake()
        {
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = configSync?.AddLockingConfigEntry(_serverConfigLocked);

            _disableVisuals = config("General", "Disable XRayVision", KeyboardShortcut.Empty,
                new ConfigDescription(
                    "Custom shortcut to enable or disable the hover text.",
                    new AcceptableShortcuts()), false);
            _prefabNameColor = config("Colors", "Prefab Name Color", "#339E66FF",
                "Color of the Prefab Name Hover text.", false);
            _pieceNameColor = config("Colors", "Piece Name Color", "#339E66FF",
                "Color of the Piece Name Hover text.", false);
            _createdColor = config("Colors", "Created Time Color", "#078282FF",
                "Color of the Created Time Hover text.", false);
            _creatorIDColor = config("Colors", "Creator ID Color", "#00afd4",
                "Color of the Creator ID Hover text.", false);
            _creatorNameColor = config("Colors", "Creator Name Color", "#00afd4",
                "Color of the Creator Name Hover text.", false);
            _creatorSteamInfoColor = config("Colors", "Creator Steam Info Color", "#95DBE5FF",
                "Color of the Steam Information Hover text.", false);
            _leftSeperator = config("Attribute Wrapper", "Left", "「",
                "Text to be shown to the left of the attribute labels", false);
            _rightSeperator = config("Attribute Wrapper", "Right", "」",
                "Text to be shown between the attribute labels and the attribute information.", false);

            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            SetupWatcher();
        }
        

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                XRayLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                XRayLogger.LogError($"There was an issue loading your {ConfigFileName}");
                XRayLogger.LogError("Please check your config entries for spelling and format!");
            }
        }
        

        #region ConfigSetup

        private static ConfigEntry<bool>? _serverConfigLocked;
        public static ConfigEntry<KeyboardShortcut>? _disableVisuals;
        internal static ConfigEntry<string>? _prefabNameColor;
        internal static ConfigEntry<string>? _pieceNameColor;
        internal static ConfigEntry<string>? _createdColor;
        internal static ConfigEntry<string>? _creatorIDColor;
        internal static ConfigEntry<string>? _creatorNameColor;
        internal static ConfigEntry<string>? _creatorSteamInfoColor;
        internal static ConfigEntry<string>? _leftSeperator;
        internal static ConfigEntry<string>? _rightSeperator;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}