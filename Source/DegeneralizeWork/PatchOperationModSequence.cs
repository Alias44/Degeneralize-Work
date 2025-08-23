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

namespace DegeneralizeWork;

/// <summary>
/// An unholy abomination that is the combination of FindMod, PatchSequence, and MayRequire.
/// Works with mod settings to allow the user to configure which patches are run for the mods specified in <see cref="patchModes">patchModes</see>.
/// </summary>
public class PatchOperationModSequence : PatchOperationSequence
{
	protected PackageMode patchModes;

	protected override bool ApplyWorker(XmlDocument xml)
	{
		var mod = LoadedModManager.GetMod<DWCore>();

		mod.AddMode(patchModes);

		if (patchModes.ShouldApply(mod.settings))
		{
			return base.ApplyWorker(xml);
		}

		return true;
	}
}
