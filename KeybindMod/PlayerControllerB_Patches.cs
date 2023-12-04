using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace aboutblank.lethalcompany
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public static class PlayerControllerB_Patches
    {
        private static PlayerInputHandler inputHandler;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(PlayerControllerB __instance)
        {
            __instance.gameObject.AddComponent<PlayerInputHandler>();
        }

        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void AddHotkeys(PlayerControllerB __instance)
        {
            SetupKeybinds(__instance);
        }

        public static void SetupKeybinds(PlayerControllerB __instance)
        {
            __instance.playerActions.Movement.Emote1.Disable();
            __instance.playerActions.Movement.Emote2.Disable();

            if (IsLocalPlayer(__instance))
            {
                inputHandler = __instance.gameObject.GetComponent<PlayerInputHandler>();
                inputHandler.InitializeKeybinds();
            }
        }

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable(PlayerControllerB __instance)
        {
            if (IsLocalPlayer(__instance))
            {
                inputHandler?.OnEnable();
            }
        }

        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        private static void OnDisable(PlayerControllerB __instance)
        {
            if (IsLocalPlayer(__instance))
            {
                inputHandler?.OnDisable();
            }
        }

        [HarmonyPatch("OnDestroy")]
        [HarmonyPrefix]
        public static void OnDestroy(PlayerControllerB __instance)
        {
            if (IsLocalPlayer(__instance))
            {
                inputHandler?.Destroy();
                inputHandler = null;
            }
        }

        static bool IsLocalPlayer(PlayerControllerB player)
        {
            return player == GameNetworkManager.Instance.localPlayerController;
        }
    }
}
