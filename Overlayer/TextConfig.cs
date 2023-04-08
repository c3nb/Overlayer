using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

namespace Overlayer
{
    public class TextConfig
    {
        #region ShadowTextConfig
        public VertexGradient TextColor = new VertexGradient(Color.white);
        public VertexGradient ShadowColor = new VertexGradient(Color.black.WithAlpha(0.4f));
        internal Color TextColor_
        {
            get => TextColor.topLeft;
            set
            {
                TextColor.topLeft = value;
                TextColor.topRight = value;
                TextColor.bottomLeft = value;
                TextColor.bottomRight = value;
            }
        }
        internal Color ShadowColor_
        {
            get => ShadowColor.topLeft;
            set
            {
                ShadowColor.topLeft = value;
                ShadowColor.topRight = value;
                ShadowColor.bottomLeft = value;
                ShadowColor.bottomRight = value;
            }
        }
        public float LineSpacing = 10f;
        #endregion
        #region OverlayerTextConfig
        public bool Active = true;
        public bool IsExpanded = false;
        public string Font = "Default";
        public string Name = string.Empty;
        public string PlayingText = "<color=#{FOHex}>{Overloads}</color> <color=#{TEHex}>{CurTE}</color> <color=#{VEHex}>{CurVE}</color> <color=#{EPHex}>{CurEP}</color> <color=#{PHex}>{CurP}</color> <color=#{LPHex}>{CurLP}</color> <color=#{VLHex}>{CurVL}</color> <color=#{TLHex}>{CurTL}</color> <color=#{FMHex}>{MissCount}</color>";
        public string NotPlayingText = string.Empty;
        public Vector2 Position = Vector2.zero;
        public float FontSize = 44;
        public TextAlignmentOptions Alignment = TextAlignmentOptions.TopLeft;
        #endregion
    }
}
