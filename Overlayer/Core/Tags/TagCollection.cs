using Overlayer.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Overlayer.Core;

namespace Overlayer.Core
{
    public class TagCollection : IEnumerable<Tag>
    {
        internal TagCollection(IEnumerable<Tag> tags = null)
            => nTags = tags?.ToDictionary(t => t.Name) ?? new Dictionary<string, Tag>();
        public void LoadTags(Assembly assembly = null)
        {
            foreach (Type type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
                LoadTags(type);
        }
        public event Action OnChange = delegate { };
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
                Tag tag = new Tag(cTagAttr.Name, valueGetter, cTagAttr.Hidden);
                IEnumerable<MethodInfo> threads = cTagAttr.Threads?.Select(st => type.GetMethod(st, (BindingFlags)15420));
                tag.Threads = threads?.Select(m => new Thread((ThreadStart)m.CreateDelegate(typeof(ThreadStart)))).ToArray();
                AddTag(tag);
                if (!cTagAttr.HasOtherTags)
                    goto Final;
            }
            foreach (MethodInfo method in methods)
            {
                TagAttribute tagAttr = method.GetCustomAttribute<TagAttribute>();
                if (tagAttr == null) continue;
                if (tagAttr.IsDefault) continue;
                Tag tag = new Tag(tagAttr.Name, method, tagAttr.Hidden);
                AddTag(tag);
            }
        Final:
            foreach (var tag in this)
                tag.Start();
            OnChange();
        }
        public void AddTag(Tag tag)
        {
            nTags.Add(tag.Name, tag);
            tag.Start();
            OnChange();
        }
        public void AddTags(IEnumerable<Tag> tags)
        {
            foreach (Tag tag in tags)
                AddTag(tag);
            tags.ForEach(t => t.Start());
            OnChange();
        }
        public void RemoveTag(string name)
        {
            if (nTags.Remove(name))
                OnChange();
        }
        private Dictionary<string, Tag> nTags;
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
                nTags[name] = value;
                OnChange();
            }
        }
        public void Clear() => nTags.Clear();
        public bool Contains(Tag tag)
            => nTags.TryGetValue(tag.Name, out _);
        public int Count => nTags.Count;
        IEnumerator IEnumerable.GetEnumerator() => nTags.Values.GetEnumerator();
        IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => nTags.Values.GetEnumerator();
        bool TagDesc = false;
        public void DescGUI()
        {
            GUILayout.BeginHorizontal();
            if (TagDesc = GUILayout.Toggle(TagDesc, "Tags"))
            {
                GUIUtils.IndentGUI(() =>
                {
                    foreach (Tag tag in nTags.Values)
                    {
                        if (tag.Hidden) continue;
                        if (tag.IsDynamic)
                            GUILayout.Label($"{tag} (Dynamic)");
                        else GUILayout.Label($"{tag} ({(tag.IsString ? "String" : "Number")})");
                        GUILayout.Space(1);
                    }
                }, 25f, 10f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public TagCollection Copy()
            => new TagCollection(nTags.Values);
    }
}
