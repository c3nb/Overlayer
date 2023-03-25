using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Core;
using Overlayer.Core.Tags;

namespace Overlayer
{
    public class OverlayerText
    {
        public TextConfig config;
        public ShadowText Text { get; private set; } 
        public Replacer PlayingText { get; private set; }
        public Replacer NotPlayingText { get; private set; }
        public Replacer ShadowPlayingText { get; private set; }
        public Replacer ShadowNotPlayingText { get; private set; }
        public OverlayerText(TextConfig config = null)
        {
            this.config = config ?? new TextConfig();
            PlayingText = new Replacer(config.PlayingText, TagManager.All);
            NotPlayingText = new Replacer(config.NotPlayingText, TagManager.NP);
            ShadowPlayingText = new Replacer(config.PlayingText, TagManager.All);
            ShadowNotPlayingText = new Replacer(config.NotPlayingText, TagManager.NP);
            Text = ShadowText.NewText();
            Text.Init(config);
        }
        public void Apply()
        {
            PlayingText.Source = config.PlayingText;
            NotPlayingText.Source = config.NotPlayingText;
            ShadowPlayingText.Source = config.PlayingText;
            ShadowNotPlayingText.Source = config.NotPlayingText;
            PlayingText.SetReference(TagManager.All);
            NotPlayingText.SetReference(TagManager.NP);
            ShadowPlayingText.SetReference(TagManager.All);
            ShadowNotPlayingText.SetReference(TagManager.NP);
            Text.Main.lineSpacing = config.LineSpacing;
            Text.Shadow.lineSpacing = config.LineSpacing;
            Text.Main.colorGradient = config.TextColor;
            Text.Shadow.colorGradient = config.ShadowColor;
            Text.Center = Text.Position = config.Position;
            Text.FontSize = config.FontSize;
            Text.Alignment = config.Alignment;
        }
    }
}
