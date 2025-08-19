using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DegeneralizeWork;
public static class Utility
{
	public static IEnumerable<T> AllEnumValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();

	public static string PrefixTranslate(this string key) => ("DegeneralizeWork." + key).Translate();

	public static string PrefixTranslate(this string key, NamedArgument arg1) => ("DegeneralizeWork." + key).Translate(arg1);
}
