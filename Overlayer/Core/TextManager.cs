using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core
{
    public static class TextManager
    {
        public static List<OverlayerText> Texts = new List<OverlayerText>();
        public static void CreateText(TextConfig config = null)
        {
            OverlayerText text = new OverlayerText(config);
            Texts.Add(text);
            Refresh();
        }
        public static void RemoveText(OverlayerText text)
        {
            text.Text.Destroy();
            UnityEngine.Object.Destroy(text.Text.gameObject);
            ShadowText.TotalCount--;
            Texts.Remove(text);
            Refresh();
        }
        public static void Refresh()
        {
            Texts.ForEach(t => t.Apply());
            Texts = Texts.OrderBy(x => x.config.Name).ToList();
        }
        public static void GUI()
        {

        }
    }
}
