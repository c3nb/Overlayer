using JSON;
using Overlayer.Core.Interfaces;
using Overlayer.Models;
using Overlayer.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Overlayer.Core
{
    public static class TextConfigImporter
    {
        public static TextConfig Import(JsonNode node)
        {
            var config = ModelUtils.Unbox<TextConfig>(node);
            var refsNode = (JsonArray)node["References"].IfNotExist(new JsonArray());
            var refs = ModelUtils.UnwrapList<Reference>(refsNode);
            if (refs.Any())
            {
                var refsDir = Path.Combine(Main.Mod.Path, "References");
                var fontsDir = Path.Combine(refsDir, "Fonts");
                Directory.CreateDirectory(refsDir);
                if (refs.Any(r => r.ReferenceType == Reference.Type.Font))
                    Directory.CreateDirectory(fontsDir);
                foreach (var @ref in refs)
                {
                    if (@ref.ReferenceType == Reference.Type.Font)
                    {
                        var targetPath = Path.Combine(fontsDir, @ref.Name);
                        File.WriteAllBytes(targetPath, @ref.Raw.Decompress());
                        if ((Path.GetFileName(config.Font?.Replace("{ModDir}", Main.Mod.Path)) ?? "") == @ref.Name)
                            config.Font = targetPath;
                    }
                }
            }
            return config;
        }
        public static JsonArray GetReferences(TextConfig text)
        {
            List<Reference> references = new List<Reference>();
            if (!string.IsNullOrWhiteSpace(text.Font) && text.Font != "Default")
                references.Add(Reference.GetReference(text.Font, Reference.Type.Font));
            return ModelUtils.WrapList(references.Where(r => r != null).Distinct().ToList());
        }
        public class Reference : IModel, ICopyable<Reference>
        {
            public enum Type
            {
                Font,
            }
            public Type ReferenceType;
            public string From;
            public string Name;
            public byte[] Raw;
            static Dictionary<string, Reference> refCache = new Dictionary<string, Reference>();
            public static Reference GetReference(string path, Type referenceType)
            {
                var target = path.Replace("{ModDir}", Main.Mod.Path);
                if (refCache.TryGetValue(target, out var reference)) return reference;
                var @ref = new Reference();
                @ref.From = target;
                @ref.Name = Path.GetFileName(target);
                @ref.ReferenceType = referenceType;
                if (File.Exists(target))
                {
                    @ref.Raw = File.ReadAllBytes(target).Compress();
                    return refCache[target] = @ref;
                }
                return null;
            }
            public static void Flush() => refCache.Clear();
            public JsonNode Serialize()
            {
                var node = JsonNode.Empty;
                node[nameof(ReferenceType)] = ReferenceType.ToString();
                node[nameof(From)] = From;
                node[nameof(Name)] = Name;
                node[nameof(Raw)] = Raw;
                node[nameof(Raw)].Inline = true;
                return node;
            }
            public void Deserialize(JsonNode node)
            {
                ReferenceType = EnumHelper<Type>.Parse(node[nameof(ReferenceType)]);
                From = node[nameof(From)];
                Name = node[nameof(Name)];
                Raw = node[nameof(Raw)];
            }
            public Reference Copy()
            {
                var newRef = new Reference();
                newRef.ReferenceType = ReferenceType;
                newRef.From = From;
                newRef.Name = Name;
                newRef.Raw = Raw;
                return newRef;
            }
        }
    }
}
