using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON;

namespace AdofaiMapConverter.Types
{
    public struct Vector2
    {
        public double x;
        public double y;
        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly Vector2 MOne = new Vector2(-1, -1);
        public static readonly Vector2 Hrd = new Vector2(100, 100);
        public override bool Equals(object obj)
        {
            if (obj is Vector2 vec)
                return vec.x == x && vec.y == y;
            return false;
        }
        public static bool operator ==(Vector2 lvec, Vector2 rvec)
            => lvec.Equals(rvec);
        public static bool operator !=(Vector2 lvec, Vector2 rvec)
            => !lvec.Equals(rvec);
        public override int GetHashCode()
            => x.GetHashCode() + y.GetHashCode();
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static Vector2 FromNode(JsonNode node)
            => new Vector2(node[0].AsDouble, node[1].AsDouble);
        public JsonNode ToNode()
        {
            JsonArray newArr = new JsonArray();
            newArr.Inline = true;
            newArr[0] = x;
            newArr[1] = y;
            return newArr;
        }
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        return;
                    case 1:
                        y = value;
                        return;
                    default:
                        return;
                }
            }
        }
        public override string ToString() => $"[{x}, {y}]";
    }
}
