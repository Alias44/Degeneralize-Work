using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace AliasDynamicPatch;

/// <summary>
/// An unholy abomination that is the combination of FindMod, PatchSequence, and MayRequire.
/// Works with mod settings to allow the user to configure which patches are run for the mods specified in <see cref="patchModes">patchModes</see>.
/// </summary>
public class PatchOperationModSequence<T> : PatchOperationSequence where T : DynamicPatch
{
	bool requireAllLoaded = false;
	/// <summary>
	/// Indicates which <see cref="PatchMode"/>(s) a given mod should be in for a patch to apply.<br/>
	/// </summary>
	protected PackageMode patchModes;

	protected override bool ApplyWorker(XmlDocument xml)
	{
		var mod = LoadedModManager.GetMod<T>();

		mod.AddMode(patchModes);

		if (patchModes.ShouldApply(mod.settings, requireAllLoaded))
		{
			return base.ApplyWorker(xml);
		}

		return true;
	}
}
