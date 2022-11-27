/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using JSEngine;

namespace JSEngine.Library
{

	public partial class ErrorInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__ToString(ScriptEngine engine, object thisObj, object[] args)
		{
			return ToString(engine, thisObj);
		}
	}

}
