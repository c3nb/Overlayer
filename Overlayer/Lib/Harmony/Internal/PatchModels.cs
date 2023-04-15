using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyExLib
{
	// PatchJobs holds the information during correlation
	// of methods and patches while processing attribute patches
	//
	internal class PatchJobs<T>
	{
		internal class Job
		{
			internal MethodBase original;
			internal T replacement;
			internal List<HarmonyExMethod> prefixes = new();
			internal List<HarmonyExMethod> postfixes = new();
			internal List<HarmonyExMethod> transpilers = new();
			internal List<HarmonyExMethod> finalizers = new();

			internal void AddPatch(AttributePatch patch)
			{
				switch (patch.type)
				{
					case HarmonyExPatchType.Prefix:
						prefixes.Add(patch.info);
						break;
					case HarmonyExPatchType.Postfix:
						postfixes.Add(patch.info);
						break;
					case HarmonyExPatchType.Transpiler:
						transpilers.Add(patch.info);
						break;
					case HarmonyExPatchType.Finalizer:
						finalizers.Add(patch.info);
						break;
				}
			}
		}

		internal Dictionary<MethodBase, Job> state = new();

		internal Job GetJob(MethodBase method)
		{
			if (method is null) return null;
			if (state.TryGetValue(method, out var job) is false)
			{
				job = new Job() { original = method };
				state[method] = job;
			}
			return job;
		}

		internal List<Job> GetJobs()
		{
			return state.Values.Where(job =>
				job.prefixes.Count +
				job.postfixes.Count +
				job.transpilers.Count +
				job.finalizers.Count > 0
			).ToList();
		}

		internal List<T> GetReplacements()
		{
			return state.Values.Select(job => job.replacement).ToList();
		}
	}

	// AttributePatch contains all information for a patch defined by attributes
	//
	internal class AttributePatch
	{
		static readonly HarmonyExPatchType[] allPatchTypes = new[] {
			HarmonyExPatchType.Prefix,
			HarmonyExPatchType.Postfix,
			HarmonyExPatchType.Transpiler,
			HarmonyExPatchType.Finalizer,
			HarmonyExPatchType.ReversePatch,
		};

		internal HarmonyExMethod info;
		internal HarmonyExPatchType? type;

		static readonly string harmonyAttributeName = typeof(HarmonyExAttribute).FullName;
		internal static AttributePatch Create(MethodInfo patch)
		{
			if (patch is null)
				throw new NullReferenceException("Patch method cannot be null");

			var allAttributes = patch.GetCustomAttributes(true);
			var methodName = patch.Name;
			var type = GetPatchType(methodName, allAttributes);
			if (type is null)
				return null;

			if (type != HarmonyExPatchType.ReversePatch && patch.IsStatic is false)
				throw new ArgumentException("Patch method " + patch.FullDescription() + " must be static");

			var list = allAttributes
				.Where(attr => attr.GetType().BaseType.FullName == harmonyAttributeName)
				.Select(attr =>
				{
					var f_info = AccessTools.Field(attr.GetType(), nameof(HarmonyExAttribute.info));
					return f_info.GetValue(attr);
				})
				.Select(harmonyInfo => AccessTools.MakeDeepCopy<HarmonyExMethod>(harmonyInfo))
				.ToList();
			var info = HarmonyExMethod.Merge(list);
			info.method = patch;

			return new AttributePatch() { info = info, type = type };
		}

		static HarmonyExPatchType? GetPatchType(string methodName, object[] allAttributes)
		{
			var harmonyAttributes = new HashSet<string>(allAttributes
				.Select(attr => attr.GetType().FullName)
				.Where(name => name.StartsWith("HarmonyEx")));

			HarmonyExPatchType? type = null;
			foreach (var patchType in allPatchTypes)
			{
				var name = patchType.ToString();
				if (name == methodName || harmonyAttributes.Contains($"HarmonyExLib.HarmonyEx{name}"))
				{
					type = patchType;
					break;
				}
			}
			return type;
		}
	}
}
