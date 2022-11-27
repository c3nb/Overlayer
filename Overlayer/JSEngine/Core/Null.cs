﻿using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace JSEngine
{

    /// <summary>
    /// Represents the JavaScript "null" type and provides the one and only instance of that type.
    /// </summary>
    public sealed class Null
    {
        /// <summary>
        /// Creates a new Null instance.
        /// </summary>
        private Null()
        {
        }

        /// <summary>
        /// Gets the one and only "null" instance.
        /// </summary>
        public static readonly Null Value = new Null();

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        public override string ToString()
        {
            return "null";
        }
    }

}
