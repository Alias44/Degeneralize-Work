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
public class PatchOperationModSequence : PatchOperationSequence
{
	protected string packageId;

	protected List<PatchMode> patchModes;

	protected override bool ApplyWorker(XmlDocument xml)
	{
		var mod = LoadedModManager.GetMod<DWCore>();
		var currentMode = mod.settings.compatibilityMode.GetValueOrDefault(packageId, PatchMode.LightTouch);

		// A packageId must be specified
		if (packageId.NullOrEmpty())
		{
			return false;
		}

		// If no modes are provided, assume that the patch applies to every mode
		if(patchModes.NullOrEmpty())
		{
			patchModes = Utility.AllEnumValues<PatchMode>().ToList();
		}

		mod.AddMode(packageId, patchModes);

		if (patchModes.Contains(currentMode))
		{
			return base.ApplyWorker(xml);
		}

		return true;
	}
}
