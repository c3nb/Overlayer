using Overlayer.Core.JavaScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class HSVConstructor : ClrFunction
    {
        public HSVConstructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "HSV", new GameObject(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public HSV Construct(double h, double s, double v)
            => new HSV(InstancePrototype, h, s, v);
    }
    public class HSV : ObjectInstance
    {
        public HSV(ObjectInstance engine) : base(engine)
        {
            PopulateFields();
        }
        public HSV(ObjectInstance obj, double h, double s, double v) : base(obj)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }
        [JSField]
        public double h;
        [JSField]
        public double s;
        [JSField]
        public double v;
        public static implicit operator UnityEngine.Color(HSV hsv) => UnityEngine.Color.HSVToRGB((float)hsv.h, (float)hsv.s, (float)hsv.v);
    }
}
