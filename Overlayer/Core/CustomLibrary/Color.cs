using JSEngine.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JSEngine.CustomLibrary
{
    public class ColorConstructor : ClrFunction
    {
        public ColorConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Color", new Color(engine.Object.InstancePrototype)) { }
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
        public static implicit operator UnityEngine.Color(Color col) => new UnityEngine.Color((float)col.r, (float)col.g, (float)col.b, (float)col.a);
    }
}
