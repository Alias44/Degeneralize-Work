using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace AliasDynamicPatch;

/// <summary>
/// Relates a mod's packageId to a set <see cref="PatchMode"/>(s)
/// </summary>
/// <remarks>
/// note: <see cref="ModMetaData"/> has multiple fields / access methods for packageId with varying cases and suffixes, because
/// the game isn't picky about about casing when determining duplicate Id's (<see cref="ModMetaData.SamePackageId(string, bool)">SamePackageId</see>)
/// I don't want to be either. As such, use <see cref="StringComparer.CurrentCulture  IgnoreCase"/> when instantiating <see cref="anyMode"/> and <see cref="configuredModes"/>
/// </remarks>
public class PackageMode
{
	/// <summary>
	/// Stores packageIds that should apply in every patchMode
	/// </summary>
	/// <remarks>
	/// Dev note: Becasue <see cref="PackageMode"/> is unfied and reused for Mod Settings, the semantics are important here.
	/// A Patch that doesn't care what mode is set is <u>not</u> the same as a collection of Patches that comprose the set of all <see cref="PatchMode"/>s
	/// (the former doesn't need mod settings (because it always runs), while the latter should allow the user to pick the prefered mode
	///</remarks>
	HashSet<string> anyMode;

	/// <summary>
	/// Relates packageIds (Key) to a set of <see cref="PatchMode"/>s that apply
	/// </summary>
	Dictionary<string, ModMode> configuredModes;

	IEnumerable<KeyValuePair<string, ModMode>> LoadedMods => configuredModes.Where(kv => kv.Value.mod != null);

	public PackageMode()
	{
		anyMode = new(StringComparer.CurrentCultureIgnoreCase);
		configuredModes = new(StringComparer.CurrentCultureIgnoreCase);
	}

	public struct ModMode
	{
		public ModContentPack mod;
		public HashSet<PatchMode> modes;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Mods in the <see cref="anyMode"/>set should always be considered patchable and as a result should have no use configs</remarks>
	public IEnumerable<ModMode> ConfigurableMods => LoadedMods.Select(kv => kv.Value);

	public void LoadDataFromXmlCustom(XmlNode xmlRoot)
	{
		var nodes = xmlRoot.ChildNodes.Cast<XmlNode>();

		configuredModes = nodes
			.Where(node => !node.InnerText.NullOrEmpty())
			.GroupBy(node => node.Name, node => Enum.Parse<PatchMode>(node.InnerText))
			.ToDictionary(group => group.Key, group => new ModMode
			{
				mod = LoadedModManager.RunningMods.First(mod => mod.ModMetaData.SamePackageId(group.Key)),
				modes = group.ToHashSet()
			}, StringComparer.CurrentCultureIgnoreCase);

		// Semantically, nodes that group to be the set of all PatchMode's should be moved to anyMode, but I don't feel like complicating the logic
		// (and load time) for a feature that I'm not going to use (plus there's no reason to make XML more tedious to write/ read by allowing duplicate spam)

		// If no configuredModes are provided, assume that the patch applies to every mode
		anyMode = nodes
			.Where(node => node.InnerText.NullOrEmpty() && !configuredModes.ContainsKey(node.Name))
			.Select(node => node.Name)
			.ToHashSet(StringComparer.CurrentCultureIgnoreCase);
	}

	public bool ShouldApply(Settings settings, bool requireAllLoaded = false)
	{
		bool checkSettings(KeyValuePair<string, ModMode> kv) => kv.Value.modes.Contains(settings.GetMode(kv.Key));

		if (requireAllLoaded && (anyMode.Any(packageId => !Utility.IsLoaded(packageId)) || configuredModes.Values.Any(package => package.mod == null)))
		{
			return false;
		}

		// Don't check the anyMode set as it will always apply
		return LoadedMods.All(checkSettings);
	}


	/// <summary>
	/// Insert data from the passed <see cref="PackageMode"/> into the existing object
	/// </summary>
	/// <param name="newLoad">data to be added</param>
	public void Add(PackageMode newLoad)
	{
		anyMode.AddRange(newLoad.anyMode);

		foreach (var kv in newLoad.configuredModes)
		{
			if (configuredModes.ContainsKey(kv.Key))
			{
				configuredModes[kv.Key].modes.AddRange(kv.Value.modes);
			}
			else
			{
				configuredModes[kv.Key] = kv.Value;
			}

		}
	}

	/// <summary>
	/// Returns all possible <see cref="PatchMode"/>(s) that apply to a given modId
	/// </summary>
	/// <param name="modId"></param>
	/// <returns></returns>
	public IEnumerable<PatchMode> Get(string modId)
	{
		if(anyMode.Contains(modId))
		{
			return Utility.AllEnumValues<PatchMode>();
		}
		else if (configuredModes.ContainsKey(modId))
		{
			return configuredModes[modId].modes;
		}
		
		return [];
	}
}
