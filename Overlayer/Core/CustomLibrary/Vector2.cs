using JSEngine.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UVec2 = UnityEngine.Vector2;

namespace JSEngine.CustomLibrary
{
    public class Vector2Constructor : ClrFunction
    {
        public Vector2Constructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Vector2", new Vector2(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Vector2 Construct(double x, double y)
            => new Vector2(InstancePrototype, x, y);
    }
    public class Vector2 : ObjectInstance
    {
        public Vector2(ObjectInstance engine) : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
        }
        public Vector2(ObjectInstance obj, double x, double y) : base(obj)
        {
            this.x = x;
            this.y = y;
        }
        [JSField]
        public double x;
        [JSField]
        public double y;
        [JSFunction(Name = "normalize")]
        public void Normalize()
        {
            UVec2 vec = new UVec2((float)x, (float)y);
            vec.Normalize();
            x = vec.x;
            y = vec.y;
        }
        public static implicit operator UVec2(Vector2 v2) => new UVec2((float)v2.x, (float)v2.y);
    }
}
