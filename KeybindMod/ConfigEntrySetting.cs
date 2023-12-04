using BepInEx.Configuration;

namespace aboutblank.lethalcompany
{
    public class ConfigEntrySetting<T>
    {
        private readonly string ExtraDescription;

        public T DefaultValue { get; private set; }

        public string ConfigName { get; private set; }

        public string ConfigDesc => $"{ExtraDescription}";

        public ConfigEntry<T> ConfigEntry { get; set; }

        public ConfigEntrySetting(string name, T defaultValue, string extraDescription = "")
        {
            ConfigName = name;
            DefaultValue = defaultValue;
            ExtraDescription = extraDescription;
        }
    }
}
