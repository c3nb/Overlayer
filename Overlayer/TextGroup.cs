using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using TinyJson;

namespace Overlayer
{
    public class TextGroup
    {
        public class Setting
        {
            public string Name;
            public List<int> Indexes;
        }
        public static List<TextGroup> Groups = new List<TextGroup>();
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "TextGroups.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
            {
                List<Setting> settings = File.ReadAllText(JsonPath).FromJson<List<Setting>>();
                for (int i = 0; i < settings.Count; i++)
                    new TextGroup(settings[i]);
            }
        }
        public static void Save()
        {
            if (!Groups.Any()) return;
            List<Setting> settings = new List<Setting>();
            foreach (TextGroup text in Groups)
                settings.Add(text.TSetting);
            File.WriteAllText(JsonPath, settings.ToJson());
        }
        private readonly List<Text> group;
        public Setting TSetting;
        public TextGroup(Setting setting = null)
        {
            setting = setting ?? new Setting();
            group = setting.Indexes.Select(x => Text.Texts[x]).ToList();
            Groups.Add(this);
        }
        public void AddText(int index)
        {
            group.Add(Text.Texts[index]);
            TSetting.Indexes.Add(index);
        }
        public void AddPosition(float x, float y)
        {
            group.ForEach(text =>
            {
                text.TSetting.Position[0] += x;
                text.TSetting.Position[1] += y;
            });
        }
        public bool IsExpanded;
        public void GUI()
        {
            if (IsExpanded = GUILayout.Toggle(IsExpanded, TSetting.Name))
            {

            }
        }
    }
}
