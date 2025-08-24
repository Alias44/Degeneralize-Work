using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AliasDynamicPatch;
public static class Utility
{
	public static int PatchModeCount = AllEnumValues<PatchMode>().Count();

	public static IEnumerable<T> AllEnumValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();

	public static string PrefixTranslate(this string key) => ("AliasDynamicPatch." + key).Translate();

	public static string PrefixTranslate(this string key, NamedArgument arg1) => ("AliasDynamicPatch." + key).Translate(arg1);

	public static bool IsLoaded(string packageId) => LoadedModManager.RunningMods.Any(mod => mod.ModMetaData.SamePackageId(packageId));
}