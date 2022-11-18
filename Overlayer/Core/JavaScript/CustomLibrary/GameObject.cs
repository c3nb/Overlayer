using Overlayer.Core.JavaScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using GO = UnityEngine.GameObject;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class GameObjectConstructor : ClrFunction
    {
        public GameObjectConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "GameObject", new GameObject(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public GameObject Construct(Sprite sprite)
            => new GameObject(InstancePrototype, sprite);
        public GameObject Wrap(SpriteRenderer sr)
            => new GameObject(InstancePrototype, sr);
    }
    public class GameObject : ObjectInstance
    {
        public GameObject(ObjectInstance engine) : base(engine)
        {
            PopulateFunctions();
        }
        public GameObject(ObjectInstance obj, Sprite spr) : base(obj)
        {
            go = new GO();
            UnityEngine.Object.DontDestroyOnLoad(go);
            sr = go.AddComponent<SpriteRenderer>();
            this.spr = spr;
            sr.sprite = spr.orig;
        }
        public GameObject(ObjectInstance obj, SpriteRenderer sr) : base(obj)
        {
            go = sr.gameObject;
            this.sr = sr;
            spr = new Sprite(Engine);
            spr.orig = sr.sprite;
        }
        public Sprite spr;
        public GO go;
        public SpriteRenderer sr;

        [JSFunction(Name = "getPosition")]
        public Vector3 GetPosition()
        {
            var vec3 = go.transform.position;
            return new Vector3Constructor(Engine).Construct(vec3.x, vec3.y, vec3.z);
        }
        [JSFunction(Name = "setPosition")]
        public void SetPosition(Vector3 vec3)
        {
            go.transform.position = vec3;
        }
        [JSFunction(Name = "getColor")]
        public Color GetColor()
        {
            var col = sr.color;
            return new ColorConstructor(Engine).Construct(col.r, col.g, col.b, col.a);
        }
        [JSFunction(Name = "setColor")]
        public void SetColor(Color col)
        {
            sr.color = new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
        }
        [JSFunction(Name = "getSprite")]
        public Sprite GetSprite() => spr;
        [JSFunction(Name = "setSprite")]
        public void SetSprite(Sprite spr)
        {
            this.spr = spr;
            sr.sprite = spr.orig;
        }
    }
}
