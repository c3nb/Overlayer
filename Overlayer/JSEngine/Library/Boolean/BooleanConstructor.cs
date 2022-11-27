﻿using System;
using System.Collections.Generic;

namespace JSEngine.Library
{
    /// <summary>
    /// Represents the built-in javascript Boolean object.
    /// </summary>
    public partial class BooleanConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Boolean object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal BooleanConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = new List<PropertyNameAndValue>(3);
            InitializeConstructorProperties(properties, "Boolean", 1, BooleanInstance.CreatePrototype(Engine, this));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Boolean object is invoked like a function, e.g. var x = Boolean("5").
        /// Converts the given argument into a boolean value (not a Boolean object).
        /// </summary>
        [JSCallFunction]
        public bool Call(bool value)
        {
            // Note: the parameter conversion machinery handles the required conversion.
            return value;
        }

        /// <summary>
        /// Creates a new Boolean instance and initializes it to the given value.
        /// </summary>
        /// <param name="value"> The value to initialize to.  Defaults to false. </param>
        [JSConstructorFunction]
        public BooleanInstance Construct(bool value = false)
        {
            return new BooleanInstance(this.InstancePrototype, value);
        }

    }
}
