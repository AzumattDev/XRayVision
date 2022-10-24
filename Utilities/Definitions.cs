using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XRayVision.Utilities
{
    [Serializable]
    public struct ModeratorPerms
    {
        public string? moderatorID = null!;

        public ModeratorPerms()
        {
            ShowSteamInformation = null;
        }

        [YamlMember(Alias = "Steam Display", ApplyNamingConventions = false)]
        public bool? ShowSteamInformation { get; set; }
    }

    [Serializable]
    public static class SerializeUtils
    {
        public static IEnumerable<ModeratorPerms> Parse(string yaml)
        {
            return new DeserializerBuilder().IgnoreFields().Build()
                .Deserialize<Dictionary<string, ModeratorPerms>>(yaml).Select(kv =>
                {
                    ModeratorPerms def = kv.Value;
                    def.moderatorID = kv.Key;
                    return def;
                });
        }

        public static string ToYaml(SortedDictionary<string, string> modDefaultList)
        {
            ISerializer serializer = new SerializerBuilder().DisableAliases()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            string stringResult = serializer.Serialize(modDefaultList);
            return stringResult;
        }

        public static string ToYAML(this ModeratorPerms self)
        {
            ISerializer serializer = new SerializerBuilder().DisableAliases()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            string stringResult = serializer.Serialize(self);
            return stringResult;
        }

        public static SortedDictionary<string, string> FromYaml(string yaml)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            //yaml contains a string containing your YAML
            SortedDictionary<string, string> p = deserializer.Deserialize<SortedDictionary<string, string>>(yaml);
            return p;
        }
    }
}