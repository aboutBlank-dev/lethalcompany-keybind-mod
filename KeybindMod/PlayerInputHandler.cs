using System;
using System.Reflection;
using GameNetcodeStuff;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace aboutblank.lethalcompany
{
    public class PlayerInputHandler: MonoBehaviour
    {
        private PlayerControllerB owner;
         
        private MethodInfo SwitchToSlotMethod;

        private AboutBlankPlayerInput input;

        void Awake()
        {
            owner = base.gameObject.GetComponent<PlayerControllerB>();
            SwitchToSlotMethod = typeof(PlayerControllerB).GetMethod("SwitchToItemSlot", BindingFlags.Instance | BindingFlags.NonPublic);

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(PlayerSwitchSlotChannel, ReceiveSwitchSlot);
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(PlayerSwitchSlotRequestChannel, ReceiveSwitchSlotRequest);
        }

        public void InitializeKeybinds()
        {
            owner = base.gameObject.GetComponent<PlayerControllerB>();
            input = new AboutBlankPlayerInput();
            input.Enable();

            RegisterCallbacks();
            SetupKeybinds();
        }

        private void RegisterCallbacks()
        {
            input.hotbar_Slot1.performed += OnHotbar1;
            input.hotbar_Slot2.performed += OnHotbar2;
            input.hotbar_Slot3.performed += OnHotbar3;
            input.hotbar_Slot4.performed += OnHotbar4;

            input.emotes_emote1.performed += OnEmote1;
            input.emotes_emote2.performed += OnEmote2;
        }

        private void UnregisterCallbacks()
        {
            input.hotbar_Slot1.performed -= OnHotbar1;
            input.hotbar_Slot2.performed -= OnHotbar2;
            input.hotbar_Slot3.performed -= OnHotbar3;
            input.hotbar_Slot4.performed -= OnHotbar4;

            input.emotes_emote1.performed -= OnEmote1;
            input.emotes_emote2.performed -= OnEmote2;
        }

        public void SetupKeybinds()
        {
            input.hotbar_Slot1.ChangeBinding(0).WithPath(KeybindMod.SlotKeybinds[0].ConfigEntry.Value);
            input.hotbar_Slot2.ChangeBinding(0).WithPath(KeybindMod.SlotKeybinds[1].ConfigEntry.Value);
            input.hotbar_Slot3.ChangeBinding(0).WithPath(KeybindMod.SlotKeybinds[2].ConfigEntry.Value);
            input.hotbar_Slot4.ChangeBinding(0).WithPath(KeybindMod.SlotKeybinds[3].ConfigEntry.Value);
            input.emotes_emote1.ChangeBinding(0).WithPath(KeybindMod.EmoteKeybinds[0].ConfigEntry.Value);
            input.emotes_emote2.ChangeBinding(0).WithPath(KeybindMod.EmoteKeybinds[1].ConfigEntry.Value);
        }

        public void OnHotbar1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RequestSlotChange(0);
            }
        }

        public void OnHotbar2(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RequestSlotChange(1);
            }
        }

        public void OnHotbar3(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RequestSlotChange(2);
            }
        }

        public void OnHotbar4(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RequestSlotChange(3);
            }
        }

        public void OnEmote1(InputAction.CallbackContext context)
        {
            owner?.PerformEmote(context, 1);
        }

        public void OnEmote2(InputAction.CallbackContext context)
        {
            owner?.PerformEmote(context, 2);
        }
        public void SwitchToSlot(int slot)
        {
            ShipBuildModeManager.Instance.CancelBuildMode();
            _ = owner.currentItemSlot;
            owner.playerBodyAnimator.SetBool("GrabValidated", value: false);
            object[] parameters = new object[2] { slot, null };
            SwitchToSlotMethod.Invoke(owner, parameters);
        }

        bool CanSwitchSlot(PlayerControllerB player)
        {
            Type typeFromHandle = typeof(PlayerControllerB);
            bool flag = (bool)typeFromHandle.GetField("throwingObject", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(player);
            float num = (float)typeFromHandle.GetField("timeSinceSwitchingSlots", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(player);
            if (((!player.IsOwner || !player.isPlayerControlled || (player.IsServer && !player.isHostPlayerObject)) && !player.isTestingPlayer) || num < 0.3f || player.isGrabbingObjectAnimation || player.inSpecialInteractAnimation || flag || player.isTypingChat || player.twoHanded || player.activatingItem || player.jetpackControls || player.disablingJetpackControls)
            {
                return false;
            }

            return true;
        }

        public static string PlayerSwitchSlotChannel => "PlayerChangeSlot";
        public static string PlayerSwitchSlotRequestChannel => "PlayerChangeSlotRequest";
        private void RequestSlotChange(int slot)
        {
            if (CanSwitchSlot(owner))
            {
                SwitchToSlot(slot);
                CustomMessagingManager customMessagingManager = NetworkManager.Singleton.CustomMessagingManager;
                FastBufferWriter messageStream = new FastBufferWriter(4, Allocator.Temp);
                messageStream.WriteValueSafe(in slot, default(FastBufferWriter.ForPrimitives));
                customMessagingManager.SendNamedMessage(PlayerSwitchSlotRequestChannel, 0uL, messageStream, NetworkDelivery.Reliable);
                typeof(PlayerControllerB).GetField("timeSinceSwitchingSlots", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(owner, 0f);
            }
        }

        public static void ReceiveSwitchSlotRequest(ulong senderID, FastBufferReader payload)
        {
            Console.WriteLine("RECEIVE_SWITCH_SLOT_REQUEST");

            if (NetworkManager.Singleton.IsServer)
            {
                payload.ReadValueSafe(out int value, default(FastBufferWriter.ForPrimitives));
                SwitchSlot_Server(value, senderID);
            }
        }

        public static void SwitchSlot_Server(int slot, ulong clientIDOfChagedSlot)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                CustomMessagingManager customMessagingManager = NetworkManager.Singleton.CustomMessagingManager;
                FastBufferWriter messageStream = new FastBufferWriter(12, Allocator.Temp);
                messageStream.WriteValueSafe(in slot, default(FastBufferWriter.ForPrimitives));
                messageStream.WriteValueSafe(in clientIDOfChagedSlot, default(FastBufferWriter.ForPrimitives));
                customMessagingManager.SendNamedMessageToAll(PlayerSwitchSlotChannel, messageStream, NetworkDelivery.Reliable);
            }
        }

        public static void ReceiveSwitchSlot(ulong senderID, FastBufferReader payload)
        {
            Console.WriteLine("RECEIVE_SWITCH_SLOT");

            payload.ReadValueSafe(out int value, default(FastBufferWriter.ForPrimitives));
            payload.ReadValueSafe(out ulong value2, default(FastBufferWriter.ForPrimitives));
            PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
            foreach (PlayerControllerB playerControllerB in allPlayerScripts)
            {
                if (playerControllerB.playerClientId == value2)
                {
                    playerControllerB.gameObject.GetComponent<PlayerInputHandler>().SwitchToSlot(value);
                    break;
                }
            }
        }

        public void OnEnable()
        {
            input?.Enable();
        }

        public void OnDisable()
        {
            input?.Disable();
        }

        public void Destroy()
        {
            input?.Dispose();
        }
    }
}
