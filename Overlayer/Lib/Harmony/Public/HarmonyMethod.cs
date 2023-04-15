using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyExLib
{

	/// <summary>A wrapper around a method to use it as a patch (for example a Prefix)</summary>
	/// 
	public class HarmonyExMethod
	{
		/// <summary>The original method</summary>
		/// 
		public MethodInfo method; // need to be called 'method'

		/// <summary>Patch Category</summary>
		/// 
		public string category = null;

		/// <summary>Class/type declaring this patch</summary>
		/// 
		public Type declaringType;

		/// <summary>Patch method name</summary>
		/// 
		public string methodName;

		/// <summary>Optional patch <see cref="MethodType"/></summary>
		/// 
		public MethodType? methodType;

		/// <summary>Array of argument types of the patch method</summary>
		/// 
		public Type[] argumentTypes;

		/// <summary><see cref="Priority"/> of the patch</summary>
		/// 
		public int priority = -1;

		/// <summary>Install this patch before patches with these HarmonyEx IDs</summary>
		/// 
		public string[] before;

		/// <summary>Install this patch after patches with these HarmonyEx IDs</summary>
		/// 
		public string[] after;

		/// <summary>Reverse patch type, see <see cref="HarmonyExReversePatchType"/></summary>
		/// 
		public HarmonyExReversePatchType? reversePatchType;

		/// <summary>Create debug output for this patch</summary>
		/// 
		public bool? debug;

		/// <summary>Whether to use <see cref="MethodDispatchType.Call"/> (<c>true</c>) or <see cref="MethodDispatchType.VirtualCall"/> (<c>false</c>) mechanics
		/// for <see cref="HarmonyExDelegate"/>-attributed delegate</summary>
		/// 
		public bool nonVirtualDelegate;

		/// <summary>Default constructor</summary>
		/// 
		public HarmonyExMethod()
		{
		}

		void ImportMethod(MethodInfo theMethod)
		{
			method = theMethod;
			if (method is object)
			{
				var infos = HarmonyExMethodExtensions.GetFromMethod(method);
				if (infos is object)
					Merge(infos).CopyTo(this);
			}
		}

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="method">The original method</param>
		///
		public HarmonyExMethod(MethodInfo method)
		{
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			ImportMethod(method);
		}

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="delegate">The original method</param>
		///
		public HarmonyExMethod(Delegate @delegate)
			: this(@delegate.Method)
		{ }

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="method">The original method</param>
		/// <param name="priority">The patch <see cref="Priority"/></param>
		/// <param name="before">A list of harmony IDs that should come after this patch</param>
		/// <param name="after">A list of harmony IDs that should come before this patch</param>
		/// <param name="debug">Set to true to generate debug output</param>
		///
		public HarmonyExMethod(MethodInfo method, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
		{
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			ImportMethod(method);
			this.priority = priority;
			this.before = before;
			this.after = after;
			this.debug = debug;
		}

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="delegate">The original method</param>
		/// <param name="priority">The patch <see cref="Priority"/></param>
		/// <param name="before">A list of harmony IDs that should come after this patch</param>
		/// <param name="after">A list of harmony IDs that should come before this patch</param>
		/// <param name="debug">Set to true to generate debug output</param>
		///
		public HarmonyExMethod(Delegate @delegate, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
			: this(@delegate.Method, priority, before, after, debug)
		{ }

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="methodType">The patch class/type</param>
		/// <param name="methodName">The patch method name</param>
		/// <param name="argumentTypes">The optional argument types of the patch method (for overloaded methods)</param>
		///
		public HarmonyExMethod(Type methodType, string methodName, Type[] argumentTypes = null)
		{
			var result = AccessTools.Method(methodType, methodName, argumentTypes);
			if (result is null)
				throw new ArgumentException($"Cannot not find method for type {methodType} and name {methodName} and parameters {argumentTypes?.Description()}");
			ImportMethod(result);
		}

		/// <summary>Gets the names of all internal patch info fields</summary>
		/// <returns>A list of field names</returns>
		///
		public static List<string> HarmonyExFields()
		{
			return AccessTools
				.GetFieldNames(typeof(HarmonyExMethod))
				.Where(s => s != "method")
				.ToList();
		}

		/// <summary>Merges annotations</summary>
		/// <param name="attributes">The list of <see cref="HarmonyExMethod"/> to merge</param>
		/// <returns>The merged <see cref="HarmonyExMethod"/></returns>
		///
		public static HarmonyExMethod Merge(List<HarmonyExMethod> attributes)
		{
			var result = new HarmonyExMethod();
			if (attributes is null) return result;
			var resultTrv = Traverse.Create(result);
			attributes.ForEach(attribute =>
			{
				var trv = Traverse.Create(attribute);
				HarmonyExFields().ForEach(f =>
				{
					var val = trv.Field(f).GetValue();
					// The second half of this if is needed because priority defaults to -1
					// This causes the value of a HarmonyExPriority attribute to be overriden by the next attribute if it is not merged last
					// should be removed by making priority nullable and default to null at some point
					if (val is object && (f != nameof(HarmonyExMethod.priority) || (int)val != -1))
						HarmonyExMethodExtensions.SetValue(resultTrv, f, val);
				});
			});
			return result;
		}

		/// <summary>Returns a string that represents the annotation</summary>
		/// <returns>A string representation</returns>
		///
		public override string ToString()
		{
			var result = "";
			var trv = Traverse.Create(this);
			HarmonyExFields().ForEach(f =>
			{
				if (result.Length > 0) result += ", ";
				result += $"{f}={trv.Field(f).GetValue()}";
			});
			return $"HarmonyExMethod[{result}]";
		}

		// used for error reporting
		internal string Description()
		{
			var cName = declaringType is object ? declaringType.FullName : "undefined";
			var mName = methodName ?? "undefined";
			var tName = methodType.HasValue ? methodType.Value.ToString() : "undefined";
			var aName = argumentTypes is object ? argumentTypes.Description() : "undefined";
			return $"(class={cName}, methodname={mName}, type={tName}, args={aName})";
		}

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="method">The original method</param>
		///
		public static implicit operator HarmonyExMethod(MethodInfo method)
		{
			return new HarmonyExMethod(method);
		}

		/// <summary>Creates a patch from a given method</summary>
		/// <param name="delegate">The original method</param>
		///
		public static implicit operator HarmonyExMethod(Delegate @delegate)
		{
			return new HarmonyExMethod(@delegate);
		}
	}

	/// <summary>Annotation extensions</summary>
	/// 
	public static class HarmonyExMethodExtensions
	{
		internal static void SetValue(Traverse trv, string name, object val)
		{
			if (val is null) return;
			var fld = trv.Field(name);
			if (name == nameof(HarmonyExMethod.methodType) || name == nameof(HarmonyExMethod.reversePatchType))
			{
				var enumType = Nullable.GetUnderlyingType(fld.GetValueType());
				val = Enum.ToObject(enumType, (int)val);
			}
			_ = fld.SetValue(val);
		}

		/// <summary>Copies annotation information</summary>
		/// <param name="from">The source <see cref="HarmonyExMethod"/></param>
		/// <param name="to">The destination <see cref="HarmonyExMethod"/></param>
		///
		public static void CopyTo(this HarmonyExMethod from, HarmonyExMethod to)
		{
			if (to is null) return;
			var fromTrv = Traverse.Create(from);
			var toTrv = Traverse.Create(to);
			HarmonyExMethod.HarmonyExFields().ForEach(f =>
			{
				var val = fromTrv.Field(f).GetValue();
				if (val is object)
					SetValue(toTrv, f, val);
			});
		}

		/// <summary>Clones an annotation</summary>
		/// <param name="original">The <see cref="HarmonyExMethod"/> to clone</param>
		/// <returns>A copied <see cref="HarmonyExMethod"/></returns>
		///
		public static HarmonyExMethod Clone(this HarmonyExMethod original)
		{
			var result = new HarmonyExMethod();
			original.CopyTo(result);
			return result;
		}

		/// <summary>Merges annotations</summary>
		/// <param name="master">The master <see cref="HarmonyExMethod"/></param>
		/// <param name="detail">The detail <see cref="HarmonyExMethod"/></param>
		/// <returns>A new, merged <see cref="HarmonyExMethod"/></returns>
		///
		public static HarmonyExMethod Merge(this HarmonyExMethod master, HarmonyExMethod detail)
		{
			if (detail is null) return master;
			var result = new HarmonyExMethod();
			var resultTrv = Traverse.Create(result);
			var masterTrv = Traverse.Create(master);
			var detailTrv = Traverse.Create(detail);
			HarmonyExMethod.HarmonyExFields().ForEach(f =>
			{
				var baseValue = masterTrv.Field(f).GetValue();
				var detailValue = detailTrv.Field(f).GetValue();
				// This if is needed because priority defaults to -1
				// This causes the value of a HarmonyExPriority attribute to be overriden by the next attribute if it is not merged last
				// should be removed by making priority nullable and default to null at some point
				if (f != nameof(HarmonyExMethod.priority) || (int)detailValue != -1)
					SetValue(resultTrv, f, detailValue ?? baseValue);
			});
			return result;
		}

		static HarmonyExMethod GetHarmonyExMethodInfo(object attribute)
		{
			var f_info = attribute.GetType().GetField(nameof(HarmonyExAttribute.info), AccessTools.all);
			if (f_info is null) return null;
			if (f_info.FieldType.FullName != typeof(HarmonyExMethod).FullName) return null;
			var info = f_info.GetValue(attribute);
			return AccessTools.MakeDeepCopy<HarmonyExMethod>(info);
		}

		/// <summary>Gets all annotations on a class/type</summary>
		/// <param name="type">The class/type</param>
		/// <returns>A list of all <see cref="HarmonyExMethod"/></returns>
		///
		public static List<HarmonyExMethod> GetFromType(Type type)
		{
			return type.GetCustomAttributes(true)
						.Select(attr => GetHarmonyExMethodInfo(attr))
						.Where(info => info is object)
						.ToList();
		}

		/// <summary>Gets merged annotations on a class/type</summary>
		/// <param name="type">The class/type</param>
		/// <returns>The merged <see cref="HarmonyExMethod"/></returns>
		///
		public static HarmonyExMethod GetMergedFromType(Type type)
		{
			return HarmonyExMethod.Merge(GetFromType(type));
		}

		/// <summary>Gets all annotations on a method</summary>
		/// <param name="method">The method/constructor</param>
		/// <returns>A list of <see cref="HarmonyExMethod"/></returns>
		///
		public static List<HarmonyExMethod> GetFromMethod(MethodBase method)
		{
			return method.GetCustomAttributes(true)
						.Select(attr => GetHarmonyExMethodInfo(attr))
						.Where(info => info is object)
						.ToList();
		}

		/// <summary>Gets merged annotations on a method</summary>
		/// <param name="method">The method/constructor</param>
		/// <returns>The merged <see cref="HarmonyExMethod"/></returns>
		///
		public static HarmonyExMethod GetMergedFromMethod(MethodBase method)
		{
			return HarmonyExMethod.Merge(GetFromMethod(method));
		}
	}
}
