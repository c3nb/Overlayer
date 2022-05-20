using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.KeyViewer
{
    internal class KeyViewerTweaks
    {
        public static readonly KeyViewerTweaks Instance = new KeyViewerTweaks();
        private Settings Settings => Settings.Instance;
        private KeyViewerProfile CurrentProfile => Settings.CurrentProfile;
        private Dictionary<KeyCode, bool> keyState;
        private KeyViewer keyViewer;
        public void OnUpdate()
        {
            UpdateRegisteredKeys();
            UpdateKeyState();
        }
        private void UpdateRegisteredKeys()
        {
            if (!Settings.Instance.IsListening) return;
            bool changed = false;
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(code)) continue;
                if (CurrentProfile.ActiveKeys.Contains(code))
                {
                    CurrentProfile.ActiveKeys.Remove(code);
                    changed = true;
                }
                else
                {
                    CurrentProfile.ActiveKeys.Add(code);
                    changed = true;
                }
            }
            if (changed) keyViewer.UpdateKeys();
        }
        private void UpdateKeyState()
        {
            UpdateViewerVisibility();
            foreach (KeyCode code in CurrentProfile.ActiveKeys)
                keyState[code] = Input.GetKey(code);
            keyViewer.UpdateState(keyState);
        }
        public void OnHideGUI() => Settings.IsListening = false;
        public void OnSettingsGUI()
        {
            MoreGUILayout.BeginIndent();
            DrawProfileSettingsGUI();
            GUILayout.Space(12f);
            MoreGUILayout.HorizontalLine(1f, 400f);
            GUILayout.Space(8f);
            DrawKeyRegisterSettingsGUI();
            GUILayout.Space(8f);
            DrawKeyViewerSettingsGUI();
            MoreGUILayout.EndIndent();
        }
        private void DrawProfileSettingsGUI()
        {
            GUILayout.Space(4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New"))
            {
                Settings.Profiles.Add(new KeyViewerProfile());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += "Profile " + Settings.Profiles.Count;
                keyViewer.Profile = Settings.CurrentProfile;
            }
            if (GUILayout.Button("Duplicate"))
            {
                Settings.Profiles.Add(Settings.CurrentProfile.Copy());
                Settings.ProfileIndex = Settings.Profiles.Count - 1;
                Settings.CurrentProfile.Name += " Copy";
                keyViewer.Profile = Settings.CurrentProfile;
            }

            if (Settings.Profiles.Count > 1
                && GUILayout.Button("Delete"))
            {
                Settings.Profiles.RemoveAt(Settings.ProfileIndex);
                Settings.ProfileIndex =
                    Math.Min(Settings.ProfileIndex, Settings.Profiles.Count - 1);
                keyViewer.Profile = Settings.CurrentProfile;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            Settings.CurrentProfile.Name =
                MoreGUILayout.NamedTextField("Profile name:", Settings.CurrentProfile.Name, 400f);
            GUILayout.Label("Profiles:");
            int selected = Settings.ProfileIndex;
            if (MoreGUILayout.ToggleList(Settings.Profiles, ref selected, p => p.Name))
            {
                Settings.ProfileIndex = selected;
                keyViewer.Profile = Settings.CurrentProfile;
            }
        }
        private void DrawKeyRegisterSettingsGUI()
        {
            // List of registered keys
            GUILayout.Label("Registered Keys:");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            foreach (KeyCode code in CurrentProfile.ActiveKeys)
            {
                GUILayout.Label(code.ToString());
                GUILayout.Space(8f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            // Record keys toggle
            GUILayout.BeginHorizontal();
            if (Settings.IsListening)
            {
                if (GUILayout.Button("Done"))
                {
                    Settings.IsListening = false;
                }
                GUILayout.Label("Press a key to register/unregister it...");
            }
            else
            {
                if (GUILayout.Button("Change Keys"))
                    Settings.IsListening = true;
            }
            if (GUILayout.Button("Clear key press count"))
                keyViewer.ClearCounts();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void DrawKeyViewerSettingsGUI()
        {
            MoreGUILayout.BeginIndent();
            CurrentProfile.ViewerOnlyGameplay = GUILayout.Toggle(CurrentProfile.ViewerOnlyGameplay, "Only show during gameplay");
            CurrentProfile.AnimateKeys = GUILayout.Toggle(CurrentProfile.AnimateKeys, "Animate key presses");
            bool newShowTotal =
                GUILayout.Toggle(CurrentProfile.ShowKeyPressTotal, "Show key press total");
            if (newShowTotal != CurrentProfile.ShowKeyPressTotal)
            {
                CurrentProfile.ShowKeyPressTotal = newShowTotal;
                keyViewer.UpdateLayout();
            }
            float newSize =
                MoreGUILayout.NamedSlider("Size:", CurrentProfile.KeyViewerSize, 10f, 200f, 300f, roundNearest: 1f);
            if (newSize != CurrentProfile.KeyViewerSize)
            {
                CurrentProfile.KeyViewerSize = newSize;
                keyViewer.UpdateLayout();
            }
            float newX =
                MoreGUILayout.NamedSlider(
                    "X Position:",
                    CurrentProfile.KeyViewerXPos,
                    0f,
                    1f,
                    300f,
                    roundNearest: 0.01f,
                    valueFormat: "{0:0.##}");
            if (newX != CurrentProfile.KeyViewerXPos)
            {
                CurrentProfile.KeyViewerXPos = newX;
                keyViewer.UpdateLayout();
            }
            float newY =
                MoreGUILayout.NamedSlider("X Position:", CurrentProfile.KeyViewerYPos, 0f, 1f, 300f, roundNearest: 0.01f, valueFormat: "{0:0.##}");
            if (newY != CurrentProfile.KeyViewerYPos)
            {
                CurrentProfile.KeyViewerYPos = newY;
                keyViewer.UpdateLayout();
            }
            GUILayout.Space(8f);
            Color newPressed, newReleased;
            string newPressedHex, newReleasedHex;
            GUILayout.BeginHorizontal();
            GUILayout.Label(
                "Pressed outline color:",
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label(
                "Released outline color:",
                GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedOutlineColor, CurrentProfile.ReleasedOutlineColor);
            if (newPressed != CurrentProfile.PressedOutlineColor)
            {
                CurrentProfile.PressedOutlineColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedOutlineColor)
            {
                CurrentProfile.ReleasedOutlineColor = newReleased;
                keyViewer.UpdateLayout();
            }
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedOutlineColorHex,
                    CurrentProfile.ReleasedOutlineColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedOutlineColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
            {
                CurrentProfile.PressedOutlineColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedOutlineColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
            {
                CurrentProfile.ReleasedOutlineColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedOutlineColorHex = newPressedHex;
            CurrentProfile.ReleasedOutlineColorHex = newReleasedHex;
            MoreGUILayout.EndIndent();
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pressed text color:", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label("Released text color:", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedBackgroundColor, CurrentProfile.ReleasedBackgroundColor);
            if (newPressed != CurrentProfile.PressedBackgroundColor)
            {
                CurrentProfile.PressedBackgroundColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedBackgroundColor)
            {
                CurrentProfile.ReleasedBackgroundColor = newReleased;
                keyViewer.UpdateLayout();
            }
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedBackgroundColorHex,
                    CurrentProfile.ReleasedBackgroundColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedBackgroundColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
            {
                CurrentProfile.PressedBackgroundColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedBackgroundColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
            {
                CurrentProfile.ReleasedBackgroundColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedBackgroundColorHex = newPressedHex;
            CurrentProfile.ReleasedBackgroundColorHex = newReleasedHex;
            MoreGUILayout.EndIndent();
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pressed background color:", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(8f);
            GUILayout.Label("Released background color:", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            MoreGUILayout.BeginIndent();

            // Text color RGBA sliders
            (newPressed, newReleased) =
                MoreGUILayout.ColorRgbaSlidersPair(
                    CurrentProfile.PressedTextColor, CurrentProfile.ReleasedTextColor);
            if (newPressed != CurrentProfile.PressedTextColor)
            {
                CurrentProfile.PressedTextColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleased != CurrentProfile.ReleasedTextColor)
            {
                CurrentProfile.ReleasedTextColor = newReleased;
                keyViewer.UpdateLayout();
            }

            // Text color hex
            (newPressedHex, newReleasedHex) =
                MoreGUILayout.NamedTextFieldPair(
                    "Hex:",
                    "Hex:",
                    CurrentProfile.PressedTextColorHex,
                    CurrentProfile.ReleasedTextColorHex,
                    100f,
                    40f);
            if (newPressedHex != CurrentProfile.PressedTextColorHex
                && ColorUtility.TryParseHtmlString($"#{newPressedHex}", out newPressed))
            {
                CurrentProfile.PressedTextColor = newPressed;
                keyViewer.UpdateLayout();
            }
            if (newReleasedHex != CurrentProfile.ReleasedTextColorHex
                && ColorUtility.TryParseHtmlString($"#{newReleasedHex}", out newReleased))
            {
                CurrentProfile.ReleasedTextColor = newReleased;
                keyViewer.UpdateLayout();
            }
            CurrentProfile.PressedTextColorHex = newPressedHex;
            CurrentProfile.ReleasedTextColorHex = newReleasedHex;

            MoreGUILayout.EndIndent();

            MoreGUILayout.EndIndent();
        }
        public void OnEnable()
        {
            if (Settings.Profiles.Count == 0)
                Settings.Profiles.Add(new KeyViewerProfile() { Name = "Default Profile" });
            if (Settings.ProfileIndex < 0 || Settings.ProfileIndex >= Settings.Profiles.Count)
                Settings.ProfileIndex = 0;
            GameObject keyViewerObj = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(keyViewerObj);
            keyViewer = keyViewerObj.AddComponent<KeyViewer>();
            keyViewer.Profile = CurrentProfile;
            UpdateViewerVisibility();
            keyState = new Dictionary<KeyCode, bool>();
        }
        public void OnDisable() => UnityEngine.Object.Destroy(keyViewer.gameObject);
        private void UpdateViewerVisibility()
        {
            bool showViewer = true;
            if (CurrentProfile.ViewerOnlyGameplay
                && scrController.instance
                && scrConductor.instance)
            {
                bool playing = !scrController.instance.paused && scrConductor.instance.isGameWorld;
                showViewer &= playing;
            }
            if (showViewer != keyViewer.gameObject.activeSelf)
            {
                keyViewer.gameObject.SetActive(showViewer);
            }
        }
    }
}
