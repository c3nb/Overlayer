using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager.UI;

namespace TagLib.Utils
{
    public static class GUIUtils
    {
        public static void IndentGUI(Action GUI, float verticalSpace = 0f, float indentSize = 20f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentSize);
            GUILayout.BeginVertical();
            GUILayout.Space(verticalSpace);
            GUI();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        public static bool DrawColor(ref float[] color, GUIStyle style = null, params GUILayoutOption[] option) => DrawFloatMultiField(ref color, new string[]
            {
        "<color=#FF0000>R</color>",
        "<color=#00FF00>G</color>",
        "<color=#0000FF>B</color>",
        "A"
            }, style, option);
        public static bool DrawTextArea(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[]
            {
            GUILayout.ExpandWidth(false)
            });
            string text = GUILayout.TextArea(value, style ?? GUI.skin.textArea, option);
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
        public static bool DrawTextField(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            string text = GUILayout.TextField(value, style ?? GUI.skin.textArea, option);
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
        public static bool DrawEnum<T>(string title, ref T @enum) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            string[] names = values.Select(x => x.ToString()).ToArray();
            int selected = Array.IndexOf(values, @enum);
            bool result = PopupToggleGroup(ref selected, names, title);
            @enum = values[selected];
            return result;
        }
        public static T GetThis<T>(this T @this, out T t) => t = @this;
    }
    public class GUIPage : IDictionary<string, Action>
    {
        static readonly Action None = () => { };
        public GUIPage()
        {
            pages = new Dictionary<string, Action>();
            pagesCount = 0;
            page = None;
        }
        private Dictionary<string, Action> pages;
        private Action page;
        private int pagesCount;
        public Action this[string name]
        {
            get
            {
                if (pages.TryGetValue(name, out var action))
                    return action;
                return null;
            }
            set => pages[name] = value;
        }
        public bool AddPage(string name, Action page)
        {
            if (pages.TryGetValue(name, out _))
                return false;
            pages.Add(name, page);
            if (pagesCount <= 0)
                this.page = page;
            pagesCount++;
            return true;
        }
        public bool RemovePage(string name)
        {
            bool result = pages.Remove(name);
            if (result)
                pagesCount--;
            return result;
        }
        public bool Goto(string name)
        {
            bool result = pages.TryGetValue(name, out Action pg);
            if (result)
                page = pg;
            return result;
        }
        public void Render() => page();
        #region IDictionaryImpl
        ICollection<string> IDictionary<string, Action>.Keys => pages.Keys;
        ICollection<Action> IDictionary<string, Action>.Values => pages.Values;
        int ICollection<KeyValuePair<string, Action>>.Count => pages.Count;
        bool ICollection<KeyValuePair<string, Action>>.IsReadOnly => false;
        Action IDictionary<string, Action>.this[string key] { get => this[key]; set => this[key] = value; }
        bool IDictionary<string, Action>.ContainsKey(string key) => pages.ContainsKey(key);
        void IDictionary<string, Action>.Add(string key, Action value) => AddPage(key, value);
        bool IDictionary<string, Action>.Remove(string key) => RemovePage(key);
        bool IDictionary<string, Action>.TryGetValue(string key, out Action value) => pages.TryGetValue(key, out value);
        void ICollection<KeyValuePair<string, Action>>.Add(KeyValuePair<string, Action> item) => AddPage(item.Key, item.Value);
        void ICollection<KeyValuePair<string, Action>>.Clear() => pages.Clear();
        bool ICollection<KeyValuePair<string, Action>>.Contains(KeyValuePair<string, Action> item) => ((ICollection<KeyValuePair<string, Action>>)pages).Contains(item);
        void ICollection<KeyValuePair<string, Action>>.CopyTo(KeyValuePair<string, Action>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, Action>>)pages).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<string, Action>>.Remove(KeyValuePair<string, Action> item) => ((ICollection<KeyValuePair<string, Action>>)pages).Remove(item);
        IEnumerator<KeyValuePair<string, Action>> IEnumerable<KeyValuePair<string, Action>>.GetEnumerator() => pages.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => pages.GetEnumerator();
        #endregion
    }
}
