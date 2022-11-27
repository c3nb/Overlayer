using HarmonyLib;
using JSEngine.Library;
using Overlayer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace JSEngine.CustomLibrary
{
    public class PlanetConstructor : ClrFunction
    {
        public PlanetConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Planet", new Planet(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Planet Construct(int pt)
            => new Planet(InstancePrototype, pt);
    }
    public class Holder<T> : MonoBehaviour where T : Component
    {
        private T instance;
        public T Get() => instance;
        public void Set(T obj) => instance = obj;
    }
    public class SRHolder : Holder<SpriteRenderer> { }
    public class Planet : ObjectInstance
    {
        public static readonly FastInvokeHandler src = MethodInvoker.GetHandler(AccessTools.Method(typeof(scrPlanet), "SetRingColor"));
        public static readonly FastInvokeHandler sfc = MethodInvoker.GetHandler(AccessTools.Method(typeof(scrPlanet), "SetFaceColor"));
        public Planet(ObjectInstance engine) : base(engine) => PopulateFunctions();
        public Planet(ObjectInstance obj, int pt) : base(obj)
        {
            PlanetType pType = (PlanetType)pt;
            var ctrl = scrController.instance;
            planet = pType switch
            {
                PlanetType.Red => ctrl.redPlanet,
                PlanetType.Blue => ctrl.bluePlanet,
                PlanetType.Green => ctrl.planetGreen,
                PlanetType.Yellow => ctrl.planetYellow,
                PlanetType.Purple => ctrl.planetPurple,
                PlanetType.Pink => ctrl.planetPink,
                PlanetType.Orange => ctrl.planetOrange,
                PlanetType.Cyan => ctrl.planetCyan,
                PlanetType.Current => ctrl.chosenplanet,
                PlanetType.Other => ctrl.chosenplanet.other,
                _ => null
            };
            renderer = planet.GetComponent<SRHolder>()?.Get();
            if (renderer == null)
            {
                // From PlanetTweaks
                renderer = new UnityEngine.GameObject().AddComponent<SpriteRenderer>();
                var holder = planet.gameObject.AddComponent<SRHolder>();
                holder.Set(renderer);
                renderer.sortingOrder = planet.sprite.sortingOrder + 1;
                renderer.sortingLayerID = planet.faceDetails.sortingLayerID;
                renderer.sortingLayerName = planet.faceDetails.sortingLayerName;
                renderer.transform.position = planet.transform.position;
                renderer.transform.parent = planet.transform;
            }
            renderer.enabled = false;
        }
        public SpriteRenderer renderer;
        public scrPlanet planet;
        [JSFunction(Name = "getColor")]
        public Color GetColor()
        {
            var col = renderer.enabled ? renderer.color : planet.sprite.color;
            return new ColorConstructor(Engine).Construct(col.r, col.g, col.b, col.a);
        }
        [JSFunction(Name = "setColor")]
        public void SetColor(Color col)
        {
            if (renderer.enabled)
            {
                renderer.color = new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
                return;
            }
            var c = new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
            planet.sprite.color = c;
            src(planet, c);
            planet.SetCoreColor(c);
            planet.SetTailColor(c);
            sfc(planet, c);
        }
        [JSFunction(Name = "getSprite", Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Sprite GetSprite()
        {
            if (!renderer.enabled) return null;
            var spr = new Sprite(Engine);
            spr.orig = renderer.sprite;
            return spr;
        }
        [JSFunction(Name = "setSprite")]
        public void SetSprite(Sprite spr)
        {
            if (spr == null)
            {
                renderer.enabled = false;
                planet.sprite.enabled = true;
                return;
            }
            renderer.enabled = true;
            planet.sprite.enabled = false;
            renderer.sprite = spr.orig;
        }
        [JSFunction(Name = "getSpriteSize", Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Vector2 GetSize()
        {
            if (!renderer.enabled) return null;
            var vec2 = new Vector2Constructor(Engine).Construct(0, 0);
            var scale = renderer.transform.localScale;
            var w = renderer.sprite.texture.width;
            var h = renderer.sprite.texture.height;
            vec2.x = scale.x.Map(0, 1, 0, w);
            vec2.y = scale.y.Map(0, 1, 0, h);
            return vec2;
        }
        [JSFunction(Name = "setSpriteSize")]
        public void SetSize(Vector2 vec2)
        {
            if (!renderer.enabled) return;
            var scale = new UnityEngine.Vector2();
            var w = renderer.sprite.texture.width;
            var h = renderer.sprite.texture.height;
            scale.x = (float)vec2.x.Map(0, w, 0, 1);
            scale.y = (float)vec2.y.Map(0, h, 0, 1);
            renderer.transform.localScale = scale;
        }
    }
}
