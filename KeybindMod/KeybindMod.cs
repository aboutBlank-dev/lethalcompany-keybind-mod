using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace aboutblank.lethalcompany
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class KeybindMod : BaseUnityPlugin
    {
        public static KeybindMod Instance;

        public const string pluginGuid = "aboutBlank.lethalcompany.keybindmod";
        public const string pluginName = "Keybind Mod";
        public const string pluginVersion = "1.0.0";

        public ConfigFile ModConfig => configFile;
        private ConfigFile configFile;

        public static ConfigEntrySetting<string>[] SlotKeybinds = new ConfigEntrySetting<string>[4]
        {
            new ConfigEntrySetting<string>("Slot1", "<Keyboard>/1", "Left-Most Slot in your Inventory"),
            new ConfigEntrySetting<string>("Slot2", "<Keyboard>/2", ""),
            new ConfigEntrySetting<string>("Slot3", "<Keyboard>/3", ""),
            new ConfigEntrySetting<string>("Slot4", "<Keyboard>/4", "Right-Most Slot in your Inventory")
        };

        public static ConfigEntrySetting<string>[] EmoteKeybinds = new ConfigEntrySetting<string>[2]
        {
            new ConfigEntrySetting<string>("Dance", "<Keyboard>/Y", ""),
            new ConfigEntrySetting<string>("Point", "<Keyboard>/U", "")
        };

        public void Awake()
        {
            Instance = this;

            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "keybindmod.cfg"), true);

            for (int i = 0; i < SlotKeybinds.Length; i++)
            {
                ConfigEntrySetting<string> config = SlotKeybinds[i];
                ConfigEntry<string> configEntry = configFile.Bind("KeyBinds", config.ConfigName, config.DefaultValue, config.ConfigDesc);
                config.ConfigEntry = configEntry;
            }

            for (int i = 0; i < EmoteKeybinds.Length; i++)
            {
                ConfigEntrySetting<string> config = EmoteKeybinds[i];
                ConfigEntry<string> configEntry = configFile.Bind("Keybinds 2", config.ConfigName, config.DefaultValue, config.ConfigDesc);
                config.ConfigEntry = configEntry;
            }

            Harmony.CreateAndPatchAll(typeof(PlayerControllerB_Patches));

            Logger.LogInfo("[KeybindMod]: Successfully Loaded!");
        }
    }
}
