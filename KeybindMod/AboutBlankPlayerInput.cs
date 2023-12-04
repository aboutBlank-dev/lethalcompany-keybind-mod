using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace aboutblank.lethalcompany
{
    public class AboutBlankPlayerInput : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
    {
        public InputActionAsset asset { get; }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputBinding? bindingMask { get => asset.bindingMask; set => asset.bindingMask = value; }
        public ReadOnlyArray<InputDevice>? devices { get => asset.devices; set => asset.devices = value; }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public readonly InputActionMap hotbar;

        public readonly InputAction hotbar_Slot1;

        public readonly InputAction hotbar_Slot2;

        public readonly InputAction hotbar_Slot3;

        public readonly InputAction hotbar_Slot4;

        public readonly InputActionMap emotes;

        public readonly InputAction emotes_emote1;

        public readonly InputAction emotes_emote2;

        public AboutBlankPlayerInput()
        {
            Assembly assembly = Assembly.Load("aboutblank-lethalcompany-keybindmod");
            Stream stream = assembly.GetManifestResourceStream("aboutblank.lethalcompany.InputActionAsset.json");
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            asset = InputActionAsset.FromJson(json);

            hotbar = asset.FindActionMap("Hotbar");
            hotbar_Slot1 = hotbar.FindAction("Hotbar1");
            hotbar_Slot2 = hotbar.FindAction("Hotbar2");
            hotbar_Slot3 = hotbar.FindAction("Hotbar3");
            hotbar_Slot4 = hotbar.FindAction("Hotbar4");

            emotes = asset.FindActionMap("Emotes", throwIfNotFound: true);
            emotes_emote1 = emotes.FindAction("Emote1", throwIfNotFound: true);
            emotes_emote2 = emotes.FindAction("Emote2", throwIfNotFound: true);
        }
        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }
    }
}
