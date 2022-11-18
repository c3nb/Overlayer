using HarmonyLib;
using Newtonsoft.Json.Utilities.LinqBridge;
using Overlayer.Core.JavaScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class PlanetConstructor : ClrFunction
    {
        public PlanetConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Planet", new Planet(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Planet Construct(int pt)
            => new Planet(InstancePrototype, pt);
    }
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
        }
        public scrPlanet planet;
        [JSFunction(Name = "getColor")]
        public Color GetColor()
        {
            var col = planet.sprite.color;
            return new ColorConstructor(Engine).Construct(col.r, col.g, col.b, col.a);
        }
        [JSFunction(Name = "setColor")]
        public void SetColor(Color col)
        {
            var c = new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
            planet.sprite.color = c;
            src(planet, c);
            planet.SetCoreColor(c);
            planet.SetTailColor(c);
            sfc(planet, c);
        }
        [JSFunction(Name = "getSprite")]
        public Sprite GetSprite()
        {
            var spr = new Sprite(Engine);
            spr.orig = planet.sprite.sprite;
            return spr;
        }
        [JSFunction(Name = "setSprite")]
        public void SetSprite(Sprite spr)
        {
            planet.sprite.sprite = spr.orig;
        }
    }
}
