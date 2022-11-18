using Overlayer.Core.JavaScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVec3 = UnityEngine.Vector3;

namespace Overlayer.Core.JavaScript.CustomLibrary
{
    public class Vector3Constructor : ClrFunction
    {
        public Vector3Constructor(ScriptEngine engine) : base(engine.Function.InstancePrototype, "Vector3", new Vector3(engine.Object.InstancePrototype)) { }
        [JSConstructorFunction]
        public Vector3 Construct(double x, double y, double z)
            => new Vector3(InstancePrototype, x, y, z);
    }
    public class Vector3 : ObjectInstance
    {
        public Vector3(ObjectInstance engine) : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
        }
        public Vector3(ObjectInstance obj, double x, double y, double z) : base(obj)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        [JSField]
        public double x;
        [JSField]
        public double y;
        [JSField]
        public double z;
        [JSFunction(Name = "normalize")]
        public void Normalize()
        {
            UVec3 vec = new UVec3((float)x, (float)y, (float)z);
            vec = UVec3.Normalize(vec);
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }
        public static implicit operator UVec3(Vector3 v3) => new UVec3((float)v3.x, (float)v3.y, (float)v3.z);
    }
}
