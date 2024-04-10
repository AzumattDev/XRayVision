using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public const string ModVersion = "2.2.2";
        public const string ModName = "XRayVision";
        internal const string Author = "Azumatt";
        internal const string ModGuid = "Azumatt.XRayVision";
        private static string ConfigFileName = ModGuid + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static bool IsAdmin = false;
        internal static bool IsModerator = false;
        internal static string ModeratorPermsConfigPath = null!;
        internal const string ModbypassFileName = "XRayVision_ModeratorList.yml";
        internal static SortedDictionary<string, ModeratorPerms> ModeratorConfigs = new();
        internal static string ConnectionError = "";
        internal static XRayVisionPlugin instance = null!;
        internal static GameObject RootObject = null!;
        internal static Harmony? _harmony;
        internal static readonly ManualLogSource XRayLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        internal static readonly ConfigSync configSync = new(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        internal static CustomSyncedValue<string> ModeratorPermsConfigData =
            new(configSync, "XRayModeratorPerms", "");

        protected internal static string ID = "";
        public static GameObject BorrowedTooltip = null!;
        public static GameObject ToolTipGameObject = null!;
        public static XRayProps PropertiesText = null!;
        public static string CleanCopy = "";

        public enum Toggle
        {
            Off,
            On
        }

        private void Awake()
        {
            instance = this;
            /* General */
            _serverConfigLocked = config("General", "Force Server Config", Toggle.On,
                new ConfigDescription("If on, the configuration is locked and can be changed by server admins only.",
                    null, new ConfigurationManagerAttributes { Category = "1 - General", Order = 4 }));
            _ = configSync?.AddLockingConfigEntry(_serverConfigLocked);
            ToggleTooltip = config("General", "Toggle Tooltip",
                Toggle.On,
                new ConfigDescription(
                    "If on, the tooltip will be visible when you hover over an item and the tooltip is toggled to show content. If off, the tooltip will be hidden until you hold down your Disable XRayVision Keyboard shortcut.",
                    null,
                    new ConfigurationManagerAttributes { Category = "1 - General", Order = 3 }), false);
            DisableVisuals = config("General", "Disable XRayVision", new KeyboardShortcut(KeyCode.G),
                new ConfigDescription(
                    "Custom shortcut to enable or disable the tooltip. Depending on the configuration, the tooltip will be hidden or visible when you hold down or press this key.",
                    new AcceptableShortcuts(),
                    new ConfigurationManagerAttributes { Category = "1 - General", Order = 2 }), false);
            CopyHotkey = config("General", "Copy Information Shortcut",
                new KeyboardShortcut(KeyCode.H),
                new ConfigDescription(
                    "Custom shortcut to copy the current tooltip information to the clipboard.",
                    null,
                    new ConfigurationManagerAttributes { Category = "1 - General", Order = 1 }), false);

            /* Colors */
            PrefabNameColor = config("Colors", "Prefab Name Color", new Color(0.2f, 0.62f, 0.4f, 1.0f), //"#339E66FF"
                new ConfigDescription("Color of the Prefab Name Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            ModSourceColor = config("Colors", "Mod Source Color", new Color(0.2f, 0.77f, 0.4f, 1.0f), //"#339E66FF"
                new ConfigDescription("Color of the Mod Source Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            PieceNameColor = config("Colors", "Piece Name Color", new Color(0.2f, 1f, 0.4f, 1.0f), // "#339E66FF"
                new ConfigDescription("Color of the Piece Name Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            CreatedColor = config("Colors", "Created Time Color", new Color(0.02f, 0.51f, 0.51f, 1.0f), // "#078282FF"
                new ConfigDescription("Color of the Created Time Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            CreatorIDColor = config("Colors", "Creator ID Color", new Color(0.0f, 0.686f, 0.83f, 1.0f), // "#00afd4"
                new ConfigDescription("Color of the Creator ID Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            CreatorNameColor = config("Colors", "Creator Name Color", new Color(0.0f, 0.686f, 0.83f, 1.0f), // "#00afd4"
                new ConfigDescription("Color of the Creator Name Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            CreatorSteamInfoColor = config("Colors", "Creator Steam Info Color", new Color(0.585f, 0.858f, 0.898f, 1.0f), // "#95DBE5FF"
                new ConfigDescription("Color of the Steam Information Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            OwnerColor = config("Colors", "Owner Info Color", new Color(0.756f, 0.917f, 0.941f, 1.0f), // "#c1eaf0"
                new ConfigDescription("Color of the Owner Hover text.", null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);
            ToolTipBkgColor = config("Attribute Wrapper", "Tooltip Background Color",
                new Color(0.0f, 0.0f, 0.0f, 0.945f),
                new ConfigDescription("Color of the background of the tooltip.",
                    null,
                    new ConfigurationManagerAttributes { Category = "2 - Colors" }), false);

            /* Attribute Wrapper */
            LeftSeperator = config("Attribute Wrapper", "Left", "「",
                new ConfigDescription("Text to be shown to the left of the attribute labels", null,
                    new ConfigurationManagerAttributes { Category = "3 - Attribute Wrapper" }), false);
            RightSeperator = config("Attribute Wrapper", "Right", "」",
                new ConfigDescription("Text to be shown between the attribute labels and the attribute information.",
                    null,
                    new ConfigurationManagerAttributes { Category = "3 - Attribute Wrapper" }), false);
            ToolTipPosition = config("Attribute Wrapper", "Tooltip Position", new Vector2(-500f, 50f),
                new ConfigDescription("Text to be shown between the attribute labels and the attribute information.",
                    null,
                    new ConfigurationManagerAttributes { Category = "3 - Attribute Wrapper" }), false);
            ToolTipTextSize = config("Attribute Wrapper", "Tooltip Text Size", 16,
                new ConfigDescription("Font size for the tooltip text",
                    new AcceptableValueRange<int>(1, int.MaxValue),
                    new ConfigurationManagerAttributes { Category = "3 - Attribute Wrapper" }), false);
            ToolTipTitleSize = config("Attribute Wrapper", "Tooltip Title Size", 18,
                new ConfigDescription("Font size for the tooltip title text",
                    new AcceptableValueRange<int>(1, int.MaxValue),
                    new ConfigurationManagerAttributes { Category = "3 - Attribute Wrapper" }), false);

            ToolTipBkgColor.SettingChanged += (_, _) =>
            {
                if (ToolTipGameObject != null)
                {
                    PropertiesText.backgroundcomp.color = ToolTipBkgColor.Value;
                }
            };
            ToolTipPosition.SettingChanged += (_, _) =>
            {
                if (ToolTipGameObject != null && ToolTipGameObject.TryGetComponent(out RectTransform transform))
                {
                    transform.anchoredPosition = ToolTipPosition.Value;
                }
            };
            ToolTipTextSize.SettingChanged += (_, _) =>
            {
                if (ToolTipGameObject != null)
                {
                    PropertiesText.textcomp.fontSize = ToolTipTextSize.Value;
                }
            };
            ToolTipTitleSize.SettingChanged += (_, _) =>
            {
                if (ToolTipGameObject != null)
                {
                    PropertiesText.titlecomp.fontSize = ToolTipTitleSize.Value;
                }
            };

            ModeratorPermsConfigData.ValueChanged += OnValChangedUpdate; // Check for file changes.
            LoadTooltipAsset("xraytooltip");
            ToolTipGameObject = new GameObject("XRayVisionPropertiesText");
            ToolTipGameObject.SetActive(false);
            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            SetupWatcher();
        }

        private void Start()
        {
            FileHandler.ModeratorListCreate();
            Game.isModded = true;
            AssetLoadTracker.MapPrefabsToBundles();
            AssetLoadTracker.MapBundlesToAssemblies();
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

        private void LoadTooltipAsset(string bundleName)
        {
            AssetBundle? assetBundle = GetAssetBundleFromResources(bundleName);
            BorrowedTooltip = assetBundle.LoadAsset<GameObject>("TESTTT");
            BorrowedTooltip.AddComponent<XRayProps>();
            assetBundle?.Unload(false);
        }

        private static AssetBundle GetAssetBundleFromResources(string filename)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));

            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }


        #region ConfigSetup

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        internal static ConfigEntry<Toggle> ToggleTooltip = null!;
        public static ConfigEntry<KeyboardShortcut> DisableVisuals = null!;
        public static ConfigEntry<KeyboardShortcut> CopyHotkey = null!;
        internal static ConfigEntry<Color> PrefabNameColor = null!;
        internal static ConfigEntry<Color> ModSourceColor = null!;
        internal static ConfigEntry<Color> PieceNameColor = null!;
        internal static ConfigEntry<Color> CreatedColor = null!;
        internal static ConfigEntry<Color> CreatorIDColor = null!;
        internal static ConfigEntry<Color> CreatorNameColor = null!;
        internal static ConfigEntry<Color> CreatorSteamInfoColor = null!;
        internal static ConfigEntry<Color> OwnerColor = null!;
        internal static ConfigEntry<string> LeftSeperator = null!;
        internal static ConfigEntry<string> RightSeperator = null!;
        internal static ConfigEntry<Vector2> ToolTipPosition = null!;
        internal static ConfigEntry<int> ToolTipTextSize = null!;
        internal static ConfigEntry<int> ToolTipTitleSize = null!;
        internal static ConfigEntry<Color> ToolTipBkgColor = null!;


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
            //public bool? Browsable = false;
            public int? Order;
            public string? Category;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
}