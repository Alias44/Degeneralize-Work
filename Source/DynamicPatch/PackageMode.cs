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
/// Used to hold relationship between mod packageId(s) and <see cref="PatchMode"/>(s)
/// </summary>
/// <remarks>
/// note: <see cref="ModMetaData"/> has multiple fields / access methods for packageId with varying cases and suffixes, because
/// the game isn't picky about about casing when determining duplicate Id's (<see cref="ModMetaData.SamePackageId(string, bool)">SamePackageId</see>)
/// I don't want to be either. As such, use <see cref="StringComparer.CurrentCulture  IgnoreCase"/> when instantiating <see cref="anyMode"/> and <see cref="modes"/>
/// </remarks>
public class PackageMode
{
	Combination combinationMode = default;

	/// <summary>
	/// Stores packageIds that should apply in every patchMode
	/// </summary>
	/// <remarks>
	/// Dev note: Becasue <see cref="PackageMode"/> is unfied and reused for Mod Settings, the semantics are important here.
	/// A Patch that doesn't care what mode is set is <u>not</u> the same as a collection of Patches that comprose the set of all <see cref="PatchMode"/>s
	/// (the former doesn't need mod settings (because it always runs), while the latter should allow the user to pick the 
	///</remarks>
	HashSet<string> anyMode;

	/// <summary>
	/// Relates packageIds (Key) to a set of patchModes that apply (value)
	/// </summary>
	Dictionary<string, HashSet<PatchMode>> modes;

	public PackageMode()
	{
		anyMode = new(StringComparer.CurrentCultureIgnoreCase);
		modes = new(StringComparer.CurrentCultureIgnoreCase);
	}

	public struct ModMode
	{
		public ModContentPack mod;
		public IEnumerable<PatchMode> modes;
	}

	/// <summary>
	/// 
	/// </summary>
	public IEnumerable<ModMode> ConfigurableMods => LoadedModManager.RunningMods
		.Where(mod => modes.ContainsKey(mod.PackageId)) // Mods in anyMode should always be considered patchable and as a result should have no use configs
		.Select(content => new ModMode()
		{
			mod = content,
			modes = Get(content.PackageId)
		});

	public void LoadDataFromXmlCustom(XmlNode xmlRoot)
	{
		var nodes = xmlRoot.ChildNodes.Cast<XmlNode>()
			.Where(node => LoadedModManager.RunningMods.Any(mod => mod.ModMetaData.SamePackageId(node.Name))); // Only worry about mods that are running

		modes = nodes
			.Where(node => !node.InnerText.NullOrEmpty())
			.GroupBy(node => node.Name, node => Enum.Parse<PatchMode>(node.InnerText))
			.ToDictionary(group => group.Key, group => group.ToHashSet(), StringComparer.CurrentCultureIgnoreCase);

		// Semantically, nodes that group to be the set of all PatchMode's should be moved to anyMode, but I don't feel like complicating the logic
		// (and load time) for a feature that I'm not going to use (plus there's no reason to make XML more tedious to write/ read by forcing duplicate spam)

		// If no modes are provided, assume that the patch applies to every mode
		anyMode = nodes
			.Where(node => node.InnerText.NullOrEmpty() && !modes.ContainsKey(node.Name))
			.Select(node => node.Name)
			.ToHashSet(StringComparer.CurrentCultureIgnoreCase);
	}

	public bool ShouldApply(Settings settings)
	{
		// If no key hasn't been added to the settigns, assume that it should be in deafult (LightTouch) mode
		bool checkSettings(KeyValuePair<string, HashSet<PatchMode>> kv) => kv.Value.Contains(settings.GetMode(kv.Key));

		// Don't check the anyMode set as it will always apply
		return combinationMode switch
		{
			Combination.All => modes.All(checkSettings),
			Combination.Any => modes.Any(checkSettings),
			_ => false,
		};
	}


	/// <summary>
	/// Insert data from the passed <see cref="PackageMode"/> into the existing object
	/// </summary>
	/// <param name="newLoad">data to be added</param>
	public void Add(PackageMode newLoad)
	{
		anyMode.AddRange(newLoad.anyMode);

		foreach (var kv in newLoad.modes)
		{
			if (modes.ContainsKey(kv.Key))
			{
				modes[kv.Key].AddRange(kv.Value);
			}
			else
			{
				modes[kv.Key] = kv.Value;
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
		else if (modes.ContainsKey(modId))
		{
			return modes[modId];
		}
		
		return [];
	}
}
