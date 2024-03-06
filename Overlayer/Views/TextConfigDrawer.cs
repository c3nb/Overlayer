using Overlayer.Core;
using Overlayer.Models;
using Overlayer.Tags;
using Overlayer.Unity;
using SFB;
using System.IO;
using UnityEngine;
using TKTC = Overlayer.Core.Translation.TranslationKeys.TextConfig;

namespace Overlayer.Views
{
    public class TextConfigDrawer : ModelDrawable<TextConfig>
    {
        public OverlayerText text;
        private bool[] colorsExpanded = new bool[4];
        public TextConfigDrawer(TextConfig config) : base(config, L(TKTC.Prefix, config.Name)) => text = TextManager.Find(config);
        public override void Draw()
        {
            if (Drawer.DrawBool(L(TKTC.Active), ref model.Active))
                text.gameObject.SetActive(model.Active);
            bool changed = false;
            Drawer.ButtonLabel($"Available Tags: {TagManager.Count}", Main.OpenDiscordLink);
            changed |= Drawer.DrawVector2(L(TKTC.Position), ref model.Position, 0, 1);
            changed |= Drawer.DrawVector2(L(TKTC.Pivot), ref model.Pivot, 0, 1);
            changed |= Drawer.DrawVector3(L(TKTC.Rotation), ref model.Rotation, -180, 180);
            changed |= Drawer.DrawString(L(TKTC.Font), ref model.Font);
            changed |= Drawer.DrawSingleWithSlider(L(TKTC.FontSize), ref model.FontSize, 0, 100, 300f);
            Drawer.DrawBool(L(TKTC.EditThis, L(TKTC.TextColor)), ref colorsExpanded[0]);
            if (colorsExpanded[0])
            {
                GUILayoutEx.BeginIndent();
                changed |= Drawer.DrawGColor(ref model.TextColor, true);
                GUILayoutEx.EndIndent();
            }
            Drawer.DrawBool(L(TKTC.EditThis, L(TKTC.ShadowColor)), ref colorsExpanded[1]);
            if (colorsExpanded[1])
            {
                GUILayoutEx.BeginIndent();
                changed |= Drawer.DrawGColor(ref model.ShadowColor, false);
                GUILayoutEx.EndIndent();
            }
            Drawer.DrawBool(L(TKTC.EditThis, L(TKTC.OutlineColor)), ref colorsExpanded[2]);
            if (colorsExpanded[2])
            {
                GUILayoutEx.BeginIndent();
                changed |= Drawer.DrawGColor(ref model.OutlineColor, false);
                GUILayoutEx.EndIndent();
            }
            changed |= Drawer.DrawSingleWithSlider(L(TKTC.OutlineWidth), ref model.OutlineWidth, 0, 1, 300f);

            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(L(TKTC.Alignment), Main.OpenDiscordLink);
            changed |= Drawer.DrawEnum(L(TKTC.Alignment), ref model.Alignment);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            changed |= Drawer.DrawString(L(TKTC.PlayingText), ref model.PlayingText, true);
            changed |= Drawer.DrawString(L(TKTC.NotPlayingText), ref model.NotPlayingText, true);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(L(TKTC.Export)))
            {
                string target = StandaloneFileBrowser.SaveFilePanel(L(TKTC.SelectText), Persistence.GetLastUsedFolder(), $"{model.Name}.json", "json");
                if (!string.IsNullOrWhiteSpace(target))
                {
                    var node = model.Serialize();
                    node["References"] = TextConfigImporter.GetReferences(model);
                    File.WriteAllText(target, node.ToString(4));
                }
            }
            if (GUILayout.Button(L(TKTC.Reset)))
            {
                changed = true;
                text.Config = model = new TextConfig();
            }
            if (GUILayout.Button(L(TKTC.Destroy)))
            {
                TextManager.DestroyText(text);
                Main.GUI.Skip(frames: 2);
                Main.GUI.Pop();
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (changed) text.ApplyConfig();
        }
    }
}
