using System;
using System.Collections.Generic;
using Verse;

namespace DegeneralizeWork;
public class Settings : ModSettings
{
	public Dictionary<string, PatchMode> compatibilityMode = [];

	public override void ExposeData()
	{
		base.ExposeData();

		Scribe_Collections.Look(ref compatibilityMode, "DW.compatibilityMode", LookMode.Value, LookMode.Value);

		if (Scribe.mode == LoadSaveMode.LoadingVars && compatibilityMode == null)
		{
			compatibilityMode = [];
		}
	}
}
