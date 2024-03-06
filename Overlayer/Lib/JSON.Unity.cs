using UnityEngine;

// Original Is SimpleJSON.Unity
// Modified By CSNB (c3nb)
// https://github.com/c3nb
namespace JSON
{
    public enum JsonContainerType { Array, Object }
    public partial class JsonNode
    {
        public static byte Color32DefaultAlpha = 255;
        public static float ColorDefaultAlpha = 1f;
        public static JsonContainerType VectorContainerType = JsonContainerType.Array;
        public static JsonContainerType QuaternionContainerType = JsonContainerType.Array;
        public static JsonContainerType RectContainerType = JsonContainerType.Array;
        public static JsonContainerType ColorContainerType = JsonContainerType.Array;
        private static JsonNode GetContainer(JsonContainerType aType)
        {
            if (aType == JsonContainerType.Array)
                return new JsonArray();
            return new JsonObject();
        }
        #region implicit conversion operators
        public static implicit operator JsonNode(Vector2 aVec)
        {
            JsonNode n = GetContainer(VectorContainerType);
            n.WriteVector2(aVec);
            return n;
        }
        public static implicit operator JsonNode(Vector3 aVec)
        {
            JsonNode n = GetContainer(VectorContainerType);
            n.WriteVector3(aVec);
            return n;
        }
        public static implicit operator JsonNode(Vector4 aVec)
        {
            JsonNode n = GetContainer(VectorContainerType);
            n.WriteVector4(aVec);
            return n;
        }
        public static implicit operator JsonNode(Color aCol)
        {
            JsonNode n = GetContainer(ColorContainerType);
            n.WriteColor(aCol);
            return n;
        }
        public static implicit operator JsonNode(Color32 aCol)
        {
            JsonNode n = GetContainer(ColorContainerType);
            n.WriteColor32(aCol);
            return n;
        }
        public static implicit operator JsonNode(Quaternion aRot)
        {
            JsonNode n = GetContainer(QuaternionContainerType);
            n.WriteQuaternion(aRot);
            return n;
        }
        public static implicit operator JsonNode(Rect aRect)
        {
            JsonNode n = GetContainer(RectContainerType);
            n.WriteRect(aRect);
            return n;
        }
        public static implicit operator JsonNode(RectOffset aRect)
        {
            JsonNode n = GetContainer(RectContainerType);
            n.WriteRectOffset(aRect);
            return n;
        }
        public static implicit operator JsonNode(Matrix4x4 m4x4)
        {
            JsonNode n = GetContainer(JsonContainerType.Array);
            n.WriteMatrix(m4x4);
            return n;
        }

        public static implicit operator Vector2(JsonNode aNode)
        {
            return aNode.ReadVector2();
        }
        public static implicit operator Vector3(JsonNode aNode)
        {
            return aNode.ReadVector3();
        }
        public static implicit operator Vector4(JsonNode aNode)
        {
            return aNode.ReadVector4();
        }
        public static implicit operator Color(JsonNode aNode)
        {
            return aNode.ReadColor();
        }
        public static implicit operator Color32(JsonNode aNode)
        {
            return aNode.ReadColor32();
        }
        public static implicit operator Quaternion(JsonNode aNode)
        {
            return aNode.ReadQuaternion();
        }
        public static implicit operator Rect(JsonNode aNode)
        {
            return aNode.ReadRect();
        }
        public static implicit operator RectOffset(JsonNode aNode)
        {
            return aNode.ReadRectOffset();
        }
        public static implicit operator Matrix4x4(JsonNode aNode)
        {
            return aNode.ReadMatrix();
        }
        #endregion implicit conversion operators
        #region Vector2
        public Vector2 ReadVector2(Vector2 aDefault)
        {
            if (IsObject)
                return new Vector2(this["x"].AsFloat, this["y"].AsFloat);
            if (IsArray)
                return new Vector2(this[0].AsFloat, this[1].AsFloat);
            return aDefault;
        }
        public Vector2 ReadVector2(string aXName, string aYName)
        {
            if (IsObject)
            {
                return new Vector2(this[aXName].AsFloat, this[aYName].AsFloat);
            }
            return Vector2.zero;
        }

        public Vector2 ReadVector2()
        {
            return ReadVector2(Vector2.zero);
        }
        public JsonNode WriteVector2(Vector2 aVec, string aXName = "x", string aYName = "y")
        {
            if (IsObject)
            {
                Inline = true;
                this[aXName] = aVec.x;
                this[aYName] = aVec.y;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aVec.x;
                this[1] = aVec.y;
            }
            return this;
        }
        #endregion Vector2
        #region Vector3
        public Vector3 ReadVector3(Vector3 aDefault)
        {
            if (IsObject)
                return new Vector3(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat);
            if (IsArray)
                return new Vector3(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat);
            return aDefault;
        }
        public Vector3 ReadVector3(string aXName, string aYName, string aZName)
        {
            if (IsObject)
                return new Vector3(this[aXName].AsFloat, this[aYName].AsFloat, this[aZName].AsFloat);
            return Vector3.zero;
        }
        public Vector3 ReadVector3()
        {
            return ReadVector3(Vector3.zero);
        }
        public JsonNode WriteVector3(Vector3 aVec, string aXName = "x", string aYName = "y", string aZName = "z")
        {
            if (IsObject)
            {
                Inline = true;
                this[aXName] = aVec.x;
                this[aYName] = aVec.y;
                this[aZName] = aVec.z;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aVec.x;
                this[1] = aVec.y;
                this[2] = aVec.z;
            }
            return this;
        }
        #endregion Vector3
        #region Vector4
        public Vector4 ReadVector4(Vector4 aDefault)
        {
            if (IsObject)
                return new Vector4(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat, this["w"].AsFloat);
            if (IsArray)
                return new Vector4(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
            return aDefault;
        }
        public Vector4 ReadVector4()
        {
            return ReadVector4(Vector4.zero);
        }
        public JsonNode WriteVector4(Vector4 aVec)
        {
            if (IsObject)
            {
                Inline = true;
                this["x"] = aVec.x;
                this["y"] = aVec.y;
                this["z"] = aVec.z;
                this["w"] = aVec.w;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aVec.x;
                this[1] = aVec.y;
                this[2] = aVec.z;
                this[3] = aVec.w;
            }
            return this;
        }
        #endregion Vector4
        #region Color / Color32
        public Color ReadColor(Color aDefault)
        {
            if (IsObject)
                return new Color(this["r"].AsFloat, this["g"].AsFloat, this["b"].AsFloat, HasKey("a") ? this["a"].AsFloat : ColorDefaultAlpha);
            if (IsArray)
                return new Color(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, (Count > 3) ? this[3].AsFloat : ColorDefaultAlpha);
            return aDefault;
        }
        public Color ReadColor()
        {
            return ReadColor(Color.clear);
        }
        public JsonNode WriteColor(Color aCol)
        {
            if (IsObject)
            {
                Inline = true;
                this["r"] = aCol.r;
                this["g"] = aCol.g;
                this["b"] = aCol.b;
                this["a"] = aCol.a;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aCol.r;
                this[1] = aCol.g;
                this[2] = aCol.b;
                this[3] = aCol.a;
            }
            return this;
        }

        public Color32 ReadColor32(Color32 aDefault)
        {
            if (IsObject)
                return new Color32((byte)this["r"].AsInt, (byte)this["g"].AsInt, (byte)this["b"].AsInt, (byte)(HasKey("a") ? this["a"].AsInt : Color32DefaultAlpha));
            if (IsArray)
                return new Color32((byte)this[0].AsInt, (byte)this[1].AsInt, (byte)this[2].AsInt, (byte)((Count > 3) ? this[3].AsInt : Color32DefaultAlpha));
            return aDefault;
        }
        public Color32 ReadColor32()
        {
            return ReadColor32(new Color32());
        }
        public JsonNode WriteColor32(Color32 aCol)
        {
            if (IsObject)
            {
                Inline = true;
                this["r"] = aCol.r;
                this["g"] = aCol.g;
                this["b"] = aCol.b;
                this["a"] = aCol.a;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aCol.r;
                this[1] = aCol.g;
                this[2] = aCol.b;
                this[3] = aCol.a;
            }
            return this;
        }

        #endregion Color / Color32
        #region Quaternion
        public Quaternion ReadQuaternion(Quaternion aDefault)
        {
            if (IsObject)
                return new Quaternion(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat, this["w"].AsFloat);
            if (IsArray)
                return new Quaternion(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
            return aDefault;
        }
        public Quaternion ReadQuaternion()
        {
            return ReadQuaternion(Quaternion.identity);
        }
        public JsonNode WriteQuaternion(Quaternion aRot)
        {
            if (IsObject)
            {
                Inline = true;
                this["x"] = aRot.x;
                this["y"] = aRot.y;
                this["z"] = aRot.z;
                this["w"] = aRot.w;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aRot.x;
                this[1] = aRot.y;
                this[2] = aRot.z;
                this[3] = aRot.w;
            }
            return this;
        }
        #endregion Quaternion
        #region Rect
        public Rect ReadRect(Rect aDefault)
        {
            if (IsObject)
                return new Rect(this["x"].AsFloat, this["y"].AsFloat, this["width"].AsFloat, this["height"].AsFloat);
            if (IsArray)
                return new Rect(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
            return aDefault;
        }
        public Rect ReadRect()
        {
            return ReadRect(new Rect());
        }
        public JsonNode WriteRect(Rect aRect)
        {
            if (IsObject)
            {
                Inline = true;
                this["x"] = aRect.x;
                this["y"] = aRect.y;
                this["width"] = aRect.width;
                this["height"] = aRect.height;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aRect.x;
                this[1] = aRect.y;
                this[2] = aRect.width;
                this[3] = aRect.height;
            }
            return this;
        }
        #endregion Rect
        #region RectOffset
        public RectOffset ReadRectOffset(RectOffset aDefault)
        {
            if (this is JsonObject)
                return new RectOffset(this["left"].AsInt, this["right"].AsInt, this["top"].AsInt, this["bottom"].AsInt);
            if (this is JsonArray)
                return new RectOffset(this[0].AsInt, this[1].AsInt, this[2].AsInt, this[3].AsInt);
            return aDefault;
        }
        public RectOffset ReadRectOffset()
        {
            return ReadRectOffset(new RectOffset());
        }
        public JsonNode WriteRectOffset(RectOffset aRect)
        {
            if (IsObject)
            {
                Inline = true;
                this["left"] = aRect.left;
                this["right"] = aRect.right;
                this["top"] = aRect.top;
                this["bottom"] = aRect.bottom;
            }
            else if (IsArray)
            {
                Inline = true;
                this[0] = aRect.left;
                this[1] = aRect.right;
                this[2] = aRect.top;
                this[3] = aRect.bottom;
            }
            return this;
        }
        #endregion RectOffset
        #region Matrix4x4
        public Matrix4x4 ReadMatrix()
        {
            Matrix4x4 result = Matrix4x4.identity;
            if (IsArray)
            {
                for (int i = 0; i < 16; i++)
                {
                    result[i] = this[i].AsFloat;
                }
            }
            return result;
        }
        public JsonNode WriteMatrix(Matrix4x4 aMatrix)
        {
            if (IsArray)
            {
                Inline = true;
                for (int i = 0; i < 16; i++)
                {
                    this[i] = aMatrix[i];
                }
            }
            return this;
        }
        #endregion Matrix4x4
    }
}