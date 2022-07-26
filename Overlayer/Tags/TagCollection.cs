using Overlayer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Overlayer.Tags
{
    public class TagCollection : IEnumerable<Tag>, IEnumerator<Tag>
    {
        internal TagCollection(IEnumerable<Tag> tags = null)
        {
            if (tags == null)
            {
                this.tags = new Tag[0];
                length = 0;
                nTags = new Dictionary<string, Tag>();
            }
            else
            {
                this.tags = tags.ToArray();
                length = this.tags.Length;
                nTags = tags.ToDictionary(t => t.Name);
            }
            position = -1;
        }
        public void LoadTags(Assembly assembly = null)
        {
            foreach (Type type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
                LoadTags(type);
        }
        public void LoadTags(Type type)
        {
            ClassTagAttribute cTagAttr = type.GetCustomAttribute<ClassTagAttribute>();
            MethodInfo[] methods = type.GetMethods((BindingFlags)15420);
            if (cTagAttr != null)
            {
                MethodInfo valueGetter = methods.FirstOrDefault(m =>
                {
                    TagAttribute ta = m.GetCustomAttribute<TagAttribute>();
                    if (ta is TagAttribute f && f.IsDefault)
                        return true;
                    return false;
                });
                if (valueGetter == null)
                    throw new InvalidOperationException("ClassTag Must Have ValueGetter Method!");
                if (!valueGetter.IsStatic)
                    throw new InvalidOperationException("ValueGetter Must Be Static!");
                Tag tag = new Tag(cTagAttr.Name, cTagAttr.Description, valueGetter, cTagAttr.NumberFormat);
                IEnumerable<MethodInfo> threads = cTagAttr.Threads?.Select(st => type.GetMethod(st, (BindingFlags)15420));
                tag.Threads = threads?.Select(m => new Thread((ThreadStart)m.CreateDelegate(typeof(ThreadStart)))).ToArray();
                Array.Resize(ref tags, length + 1);
                tags[length++] = tag;
                if (!cTagAttr.HasOtherTags)
                    goto Final;
            }
            foreach (MethodInfo method in methods)
            {
                TagAttribute tagAttr = method.GetCustomAttribute<TagAttribute>();
                if (tagAttr == null) continue;
                if (tagAttr.IsDefault) continue;
                Tag tag = new Tag(tagAttr.Name, tagAttr.Description, method, tagAttr.NumberFormat);
                Array.Resize(ref tags, length + 1);
                tags[length++] = tag;
            }
        Final:
            nTags = tags.ToDictionary(t =>
            {
                t.Start();
                return t.Name;
            });
        }
        public void AddTag(Tag tag)
        {
            tags = tags.Add(tag);
            nTags = tags.ToDictionary(t => t.Name);
            tag.Start();
            length++;
        }
        public void RemoveTag(string name)
        {
            if (!nTags.TryGetValue(name, out _)) return;
            int index = Array.FindIndex(tags, t => t.Name == name);
            length--;
            for (int a = index; a < length; a++)
                tags[a] = tags[a + 1];
            Array.Resize(ref tags, length);
            nTags.Remove(name);
        }
        private Dictionary<string, Tag> nTags;
        private Tag[] tags;
        private int length;
        private int position;
        public Tag this[string name]
        {
            get
            {
                if (nTags.TryGetValue(name, out Tag tag))
                    return tag;
                return null;
            }
            set
            {
                int index = Array.FindIndex(tags, t => t.Name == name);
                if (index != -1)
                    tags[index] = value;
                else
                {
                    tags = tags.Add(value);
                    length++;
                }
                nTags[name] = value;
            }
        }
        public Tag this[int index]
        {
            get
            {
                if (index >= length || index < 0)
                    return null;
                return tags[index];
            }
            set
            {
                if (index != -1)
                    tags[index] = value;
                nTags = tags.ToDictionary(t => t.Name);
            }
        }
        public void Clear()
        {
            for (int i = 0; i < length; i++)
                tags[i].Stop();
            Array.Clear(tags, 0, length);
            nTags.Clear();
            tags = null;
            nTags = null;
            position = -1;
            length = 0;
        }
        public int Count => length;
        IEnumerator IEnumerable.GetEnumerator() => this;
        IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => this;
        object IEnumerator.Current => tags[position];
        Tag IEnumerator<Tag>.Current => tags[position];
        void IDisposable.Dispose() { }
        bool IEnumerator.MoveNext() => length > ++position;
        void IEnumerator.Reset() => position = -1;
        bool TagDesc = false;
        public void DescGUI()
        {
            GUILayout.BeginHorizontal();
            if (TagDesc = GUILayout.Toggle(TagDesc, "Tags"))
            {
                GUIUtils.IndentGUI(() =>
                {
                    for (int i = 0; i < length; i++)
                    {
                        var tag = tags[i];
                        GUILayout.Label(tag.ToString());
                        GUILayout.Space(1);
                    }
                }, 25f, 10f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
