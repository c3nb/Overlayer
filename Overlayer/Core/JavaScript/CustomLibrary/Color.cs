using Overlayer.Core.JavaScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class ColorConstructor : ClrFunction
    {
        public ColorConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Color", new GameObject(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Color Construct(double r, double g, double b, double a = 1)
            => new Color(InstancePrototype, r, g, b, a);
    }
    public class Color : ObjectInstance
    {
        public Color(ObjectInstance engine) : base(engine)
        {
            PopulateFields();
        }
        public Color(ObjectInstance obj, double r, double g, double b, double a = 1) : base(obj)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        [JSField]
        public double r;
        [JSField]
        public double g;
        [JSField]
        public double b;
        [JSField]
        public double a;
    }
}
