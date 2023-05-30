using System;
using System.Collections.Generic;
using System.Text;

namespace JSEngine.Compiler
{
    /// <summary>
    /// Represents a javascript primitive type.
    /// </summary>
    internal enum PrimitiveType
    {
        Undefined,          // JSEngine.Undefined
        Null,               // JSEngine.Null
        Any,                // System.Object
        Bool,               // System.Boolean
        Number,             // System.Double
        Int32,              // System.Int32
        UInt32,             // System.UInt32
        String,             // System.String
        ConcatenatedString, // JSEngine.ConcatenatedString
        Object,             // JSEngine.Library.ObjectInstance
        FUCK
    }
}
