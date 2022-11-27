﻿using System;

namespace JSEngine.Library
{
    /// <summary>
    /// The base class of the javascript function attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class BaseJSFunctionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new BaseJSFunctionAttribute instance with no flags.
        /// </summary>
        protected BaseJSFunctionAttribute()
            : this(JSFunctionFlags.None)
        {
        }

        /// <summary>
        /// Creates a new BaseJSFunctionAttribute instance.
        /// </summary>
        /// <param name="flags"> One or more flags. </param>
        protected BaseJSFunctionAttribute(JSFunctionFlags flags)
        {
            this.Flags = flags;
        }

        /// <summary>
        /// Gets or sets the flags associated with the function.
        /// </summary>
        public JSFunctionFlags Flags
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Marks a method as being visible to javascript code.
    /// </summary>
    public class JSFunctionAttribute : BaseJSFunctionAttribute
    {
        /// <summary>
        /// Creates a new JSFunctionAttribute instance with no flags.
        /// </summary>
        public JSFunctionAttribute()
            : base(JSFunctionFlags.None)
        {
            this.Length = -1;
            this.IsWritable = true;
            this.IsConfigurable = true;
        }

        /// <summary>
        /// Get or sets the name of the function, as exposed to javascript.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the function is non-standard.
        /// </summary>
        public bool NonStandard
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the function is deprecated and should not be used.
        /// </summary>
        public bool Deprecated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the "typical" number of arguments expected by the function.
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the property value is writable.  If this flag is not set,
        /// attempting to modify the property will fail.  The default value of this property
        /// is <c>true</c>.
        /// </summary>
        /// <seealso cref="PropertyAttributes"/>
        public bool IsWritable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the property should be enumerable (exposed via the 
        /// <c>for...in</c> construct) in JavaScript code.  The default value of this
        /// property is <c>false</c>.
        /// </summary>
        /// <seealso cref="PropertyAttributes"/>
        public bool IsEnumerable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the property should be configurable, that is, whether
        /// the property may be changed or have its descriptor changed by JavaScript
        /// code.  The default value of this property is <c>true</c>.
        /// </summary>
        /// <seealso cref="PropertyAttributes"/>
        public bool IsConfigurable
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Marks a method as being visible to javascript code.
    /// Used internally - has different defaults to what you would expect.
    /// </summary>
    internal class JSInternalFunctionAttribute : JSFunctionAttribute
    {
        /// <summary>
        /// Gets or sets the number of parameters that are required.  If the function is called
        /// with fewer than this number of arguments, then a TypeError will be thrown.
        /// </summary>
        public int RequiredArgumentCount
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Marks a property as being visible to JavaScript code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JSPropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="JSPropertyAttribute"/>
        /// </summary>
        public JSPropertyAttribute()
        {
            this.IsConfigurable = true;
        }

        /// <summary>
        /// Gets or sets the name of the property as exposed to JavaScript code.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the property should be enumerable (exposed via the 
        /// <c>for...in</c> construct) in JavaScript code.  The default value of this
        /// property is <c>false</c>.
        /// </summary>
        /// <seealso cref="PropertyAttributes"/>
        public bool IsEnumerable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the property should be configurable, that is, whether
        /// the property may be changed or have its descriptor changed by JavaScript
        /// code.  The default value of this property is <c>true</c>.
        /// </summary>
        /// <seealso cref="PropertyAttributes"/>
        public bool IsConfigurable
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Some built-in objects act like both classes and functions depending on whether the
    /// <c>new</c> operator is used (for example, the Number object acts this way).  This
    /// property indicates that the method should be called when an object is called like
    /// a function.
    /// </summary>
    public sealed class JSCallFunctionAttribute : BaseJSFunctionAttribute
    {
    }

    /// <summary>
    /// Indicates that the method should be called when the <c>new</c> keyword is used.
    /// </summary>
    public sealed class JSConstructorFunctionAttribute : BaseJSFunctionAttribute
    {
    }

    /// <summary>
    /// Marks a field as being visible to javascript code.  Currently only <c>const</c> fields
    /// are supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JSFieldAttribute : Attribute
    {
    }
}
