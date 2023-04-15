using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyExLib
{
	/// <summary>A reverse patcher</summary>
	/// 
	public class ReversePatcher
	{
		readonly HarmonyEx instance;
		readonly MethodBase original;
		readonly HarmonyExMethod standin;

		/// <summary>Creates a reverse patcher</summary>
		/// <param name="instance">The HarmonyEx instance</param>
		/// <param name="original">The original method/constructor</param>
		/// <param name="standin">Your stand-in stub method as <see cref="HarmonyExMethod"/></param>
		///
		public ReversePatcher(HarmonyEx instance, MethodBase original, HarmonyExMethod standin)
		{
			this.instance = instance;
			this.original = original;
			this.standin = standin;
		}

		/// <summary>Applies the patch</summary>
		/// <param name="type">The type of patch, see <see cref="HarmonyExReversePatchType"/></param>
		/// <returns>The generated replacement method</returns>
		///
		public MethodInfo Patch(HarmonyExReversePatchType type = HarmonyExReversePatchType.Original)
		{
			if (original is null)
				throw new NullReferenceException($"Null method for {instance.Id}");

			standin.reversePatchType = type;
			var transpiler = GetTranspiler(standin.method);
			return PatchFunctions.ReversePatch(standin, original, transpiler);
		}

		internal static MethodInfo GetTranspiler(MethodInfo method)
		{
			var methodName = method.Name;
			var type = method.DeclaringType;
			var methods = AccessTools.GetDeclaredMethods(type);
			var ici = typeof(IEnumerable<CodeInstruction>);
			return methods.FirstOrDefault(m =>
			{
				if (m.ReturnType != ici) return false;
				return m.Name.StartsWith($"<{methodName }>");
			});
		}
	}
}
