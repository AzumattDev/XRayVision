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
using UnityEngine.Rendering;
using XRayVision.Utilities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XRayVision
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class XRayVisionPlugin : BaseUnityPlugin
    {
        public const string ModVersion = "1.5.1";
        public const string ModName = "XRayVision";
        internal const string Author = "Azumatt";
        private const string ModGuid = "azumatt.XRayVision";
        private static string ConfigFileName = ModGuid + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static bool IsAdmin = false;
        internal static bool IsModerator = false;
        internal static string ModeratorPermsConfigPath = null!;
        internal const string ModbypassFileName = "XRayVision_ModeratorList.yml";
        internal static SortedDictionary<string, ModeratorPerms> ModeratorConfigs = new();
        internal static string ConnectionError = "";

        private Harmony? _harmony;
        internal static readonly ManualLogSource XRayLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync? configSync = new(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        internal static CustomSyncedValue<string> ModeratorPermsConfigData =
            new(configSync, "XRayModeratorPerms", "");

        protected internal static string ID = "";

        private void Awake()
        {
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = configSync?.AddLockingConfigEntry(_serverConfigLocked);

            DisableVisuals = config("General", "Disable XRayVision", KeyboardShortcut.Empty,
                new ConfigDescription(
                    "Custom shortcut to enable or disable the hover text.",
                    new AcceptableShortcuts()), false);
            PrefabNameColor = config("Colors", "Prefab Name Color", "#339E66FF",
                "Color of the Prefab Name Hover text.", false);
            PieceNameColor = config("Colors", "Piece Name Color", "#339E66FF",
                "Color of the Piece Name Hover text.", false);
            CreatedColor = config("Colors", "Created Time Color", "#078282FF",
                "Color of the Created Time Hover text.", false);
            CreatorIDColor = config("Colors", "Creator ID Color", "#00afd4",
                "Color of the Creator ID Hover text.", false);
            CreatorNameColor = config("Colors", "Creator Name Color", "#00afd4",
                "Color of the Creator Name Hover text.", false);
            CreatorSteamInfoColor = config("Colors", "Creator Steam Info Color", "#95DBE5FF",
                "Color of the Steam Information Hover text.", false);
            OwnerColor = config("Colors", "Owner Info Color", "#c1eaf0",
                "Color of the Owner Hover text.", false);
            LeftSeperator = config("Attribute Wrapper", "Left", "「",
                "Text to be shown to the left of the attribute labels", false);
            RightSeperator = config("Attribute Wrapper", "Right", "」",
                "Text to be shown between the attribute labels and the attribute information.", false);

            ModeratorPermsConfigData.ValueChanged += OnValChangedUpdate; // Check for file changes.

            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            SetupWatcher();
        }

        private void Start()
        {
            FileHandler.ModeratorListCreate();
            Game.isModded = true;
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

            FileSystemWatcher moderatorfilewatcher = new(Paths.ConfigPath, ModbypassFileName);
            moderatorfilewatcher.Changed += ReadYamlConfigFile;
            moderatorfilewatcher.Created += ReadYamlConfigFile;
            moderatorfilewatcher.Renamed += ReadYamlConfigFile;
            moderatorfilewatcher.IncludeSubdirectories = true;
            moderatorfilewatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            moderatorfilewatcher.EnableRaisingEvents = true;
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

        internal static void ReadYamlConfigFile(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ModeratorPermsConfigPath)) return;
            try
            {
                XRayLogger.LogDebug("ReadYamlConfigFile called");
                StreamReader file = File.OpenText(ModeratorPermsConfigPath);
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                SortedDictionary<string, ModeratorPerms> tmp =
                    deserializer.Deserialize<SortedDictionary<string, ModeratorPerms>>(file);
                ModeratorConfigs = tmp;
                file.Close();
                ModeratorPermsConfigData.AssignLocalValue(
                    File.ReadAllText(ModeratorPermsConfigPath));
            }
            catch
            {
                XRayLogger.LogError(
                    $"There was an issue loading your {ModbypassFileName}");
                XRayLogger.LogError(
                    "Please check your config entries for spelling and format!");
            }
        }

        private static void OnValChangedUpdate()
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            XRayLogger.LogDebug("Moderator Perms OnValChanged called");
            try
            {
                ModeratorConfigs = new SortedDictionary<string, ModeratorPerms>(
                    deserializer.Deserialize<Dictionary<string, ModeratorPerms>?>(ModeratorPermsConfigData.Value) ??
                    new Dictionary<string, ModeratorPerms>());
                foreach (ModeratorPerms moderatorConfig in ModeratorConfigs.Values)
                {
                    ModeratorPerms modconfig1 = moderatorConfig;
                    modconfig1.ShowSteamInformation ??= new bool();
                }

                IsModerator = ModeratorConfigs.ContainsKey(ID);
            }
            catch (Exception e)
            {
                XRayLogger.LogError($"Failed to deserialize Moderator Config File: {e}");
            }
        }


        #region ConfigSetup

        private static ConfigEntry<bool>? _serverConfigLocked;
        public static ConfigEntry<KeyboardShortcut>? DisableVisuals;
        internal static ConfigEntry<string>? PrefabNameColor;
        internal static ConfigEntry<string>? PieceNameColor;
        internal static ConfigEntry<string>? CreatedColor;
        internal static ConfigEntry<string>? CreatorIDColor;
        internal static ConfigEntry<string>? CreatorNameColor;
        internal static ConfigEntry<string>? CreatorSteamInfoColor;
        internal static ConfigEntry<string>? OwnerColor;
        internal static ConfigEntry<string>? LeftSeperator;
        internal static ConfigEntry<string>? RightSeperator;

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