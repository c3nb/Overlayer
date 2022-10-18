using System;
using System.Collections.Generic;
using System.Text;

namespace Overlayer.Core.JavaScript.Compiler
{
    /// <summary>
    /// Represents a javascript primitive type.
    /// </summary>
    internal enum PrimitiveType
    {
        Undefined,          // Overlayer.Core.JavaScript.Undefined
        Null,               // Overlayer.Core.JavaScript.Null
        Any,                // System.Object
        Bool,               // System.Boolean
        Number,             // System.Double
        Int32,              // System.Int32
        UInt32,             // System.UInt32
        String,             // System.String
        ConcatenatedString, // Overlayer.Core.JavaScript.ConcatenatedString
        Object,             // Overlayer.Core.JavaScript.Library.ObjectInstance
    }
}
