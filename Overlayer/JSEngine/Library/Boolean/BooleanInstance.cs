﻿namespace JSEngine.Library
{
    /// <summary>
    /// Represents an instance of the JavaScript Boolean object.
    /// </summary>
    public partial class BooleanInstance : ObjectInstance
    {
        private readonly bool value;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new boolean instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The value to initialize the instance with. </param>
        public BooleanInstance(ObjectInstance prototype, bool value)
            : base(prototype)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates the Boolean prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, BooleanConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.InitializeProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the primitive value of this object.
        /// </summary>
        public bool Value
        {
            get { return this.value; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the underlying primitive value of the current object.
        /// </summary>
        /// <returns> The underlying primitive value of the current object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new bool ValueOf()
        {
            return this.value;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS()
        {
            return this.value ? "true" : "false";
        }
    }
}
