using AliasDynamicPatch;
using UnityEngine;
using Verse;

namespace DegeneralizeWork;
public class DegeneralizeWork : DynamicPatch
{
	public DegeneralizeWork(ModContentPack content) : base(content)
	{
		settings = GetSettings<DWSettings>();
		translatePrefix = "DegeneralizeWork";
	}

	public override string SettingsCategory() => "DegeneralizeWork.SettingsCategory".Translate();
}

public class DWSettings : AliasDynamicPatch.Settings
{
}