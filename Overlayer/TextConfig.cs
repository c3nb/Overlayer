using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

namespace Overlayer
{
    public struct Color2
    {
        public Color2(float r, float g, float b, float a = 1)
        {
            this.r = r;
            this.g = g;
            this.b = b; 
            this.a = a;
        }
        public float r, g, b, a;
        public static implicit operator Color(Color2 color) => new Color(color.r, color.g, color.b, color.a);
        public static implicit operator Color2(Color color) => new Color2(color.r, color.g, color.b, color.a);
    }
    public struct VertexGradient2
    {
        public VertexGradient2(Color2 color) : this(color, color, color, color) { }
        public VertexGradient2(Color2 topLeft, Color2 topRight, Color2 bottomLeft, Color2 bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
        }
        public Color2 topLeft;
        public Color2 topRight;
        public Color2 bottomLeft;
        public Color2 bottomRight;
        public static implicit operator VertexGradient(VertexGradient2 color) => new VertexGradient(color.topLeft, color.topRight, color.bottomLeft, color.bottomRight);
        public static implicit operator VertexGradient2(VertexGradient color) => new VertexGradient2(color.topLeft, color.topRight, color.bottomLeft, color.bottomRight);
    }
    public class TextConfig
    {
        #region ShadowTextConfig
        public VertexGradient2 TextColor
        {
            get => TextColor_G;
            set => TextColor_G = value;
        }
        public VertexGradient2 ShadowColor
        {
            get => ShadowColor_G;
            set => ShadowColor_G = value;
        }
        internal VertexGradient TextColor_G = new VertexGradient(Color.white);
        internal VertexGradient ShadowColor_G = new VertexGradient(Color.black.WithAlpha(0.4f));
        internal Color TextColor_
        {
            get => TextColor_G.topLeft;
            set
            {
                TextColor_G.topLeft = value;
                TextColor_G.topRight = value;
                TextColor_G.bottomLeft = value;
                TextColor_G.bottomRight = value;
            }
        }
        internal Color ShadowColor_
        {
            get => ShadowColor_G.topLeft;
            set
            {
                ShadowColor_G.topLeft = value;
                ShadowColor_G.topRight = value;
                ShadowColor_G.bottomLeft = value;
                ShadowColor_G.bottomRight = value;
            }
        }
        public float LineSpacing = -25f;
        #endregion
        #region OverlayerTextConfig
        public bool Active = true;
        public bool IsExpanded = false;
        public bool Gradient = false;
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
