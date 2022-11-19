using Overlayer.Core.JavaScript.Library;
using UnityEngine;
using UnityEngine.UI;
using GO = UnityEngine.GameObject;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class GameObjectConstructor : ClrFunction
    {
        public GameObjectConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "GameObject", new GameObject(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public GameObject Construct(Sprite sprite)
            => new GameObject(InstancePrototype, sprite);
    }
    public class GameObject : ObjectInstance
    {
        public GameObject(ObjectInstance engine) : base(engine)
        {
            PopulateFunctions();
        }
        public GameObject(ObjectInstance obj, Sprite spr) : base(obj)
        {
            var go = new GO();
            go.transform.SetParent(ShadowText.PublicCanvas.transform);
            img = go.AddComponent<Image>();
            this.spr = spr;
            img.sprite = spr.orig;
            img.rectTransform.sizeDelta = new UnityEngine.Vector2(spr.orig.texture.width, spr.orig.texture.height);
        }
        public Sprite spr;
        public Image img;
        [JSFunction(Name = "getPosition")]
        public Vector2 GetPosition()
        {
            var vec3 = img.rectTransform.position;
            return new Vector2Constructor(Engine).Construct(vec3.x, vec3.y);
        }
        [JSFunction(Name = "setPosition")]
        public void SetPosition(Vector2 vec2)
        {
            var value = (UnityEngine.Vector2)vec2;
            img.rectTransform.anchorMin = value;
            img.rectTransform.anchorMax = value;
            img.rectTransform.pivot = value;
            img.rectTransform.anchoredPosition = value;
        }
        [JSFunction(Name = "getColor")]
        public Color GetColor()
        {
            var col = img.color;
            return new ColorConstructor(Engine).Construct(col.r, col.g, col.b, col.a);
        }
        [JSFunction(Name = "setColor")]
        public void SetColor(Color col)
        {
            img.color = new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
        }
        [JSFunction(Name = "getSize")]
        public Vector2 GetSize()
        {
            var vec2 = img.rectTransform.sizeDelta;
            return new Vector2Constructor(Engine).Construct(vec2.x, vec2.y);
        }
        [JSFunction(Name = "setSize")]
        public void SetSize(Vector2 vec2)
        {
            img.rectTransform.sizeDelta = vec2;
        }
        [JSFunction(Name = "getSprite")]
        public Sprite GetSprite() => spr;
        [JSFunction(Name = "setSprite")]
        public void SetSprite(Sprite spr)
        {
            this.spr = spr;
            img.sprite = spr.orig;
        }
    }
}
