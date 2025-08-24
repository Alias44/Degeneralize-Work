using System;
using System.Collections.Generic;
using Verse;

namespace AliasDynamicPatch;
public class Settings : ModSettings
{
	public Dictionary<string, PatchMode> compatibilityMode = [];

	public PatchMode GetMode(string packageId)
	{
		// If no key hasn't been added to the settigns, assume that it should be in deafult (LightTouch) mode
		return compatibilityMode.GetValueOrDefault(packageId, default);
	}

	public void SetMode(string packageId, PatchMode mode)
	{
		compatibilityMode[packageId] = mode;
	}

	public override void ExposeData()
	{
		base.ExposeData();

		Scribe_Collections.Look(ref compatibilityMode, "compatibilityMode", LookMode.Value, LookMode.Value);

		if (Scribe.mode == LoadSaveMode.LoadingVars && compatibilityMode == null)
		{
			compatibilityMode = [];
		}
	}
}
