using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DegeneralizeWork;

public class DWCore : Mod
{
	public Settings settings;
	public Dictionary<string, HashSet<PatchMode>> availablePatches = [];

	public DWCore(ModContentPack content) : base(content)
	{
		settings = GetSettings<Settings>();
	}
	public void AddMode(string modId, List<PatchMode> mode)
	{
		HashSet<PatchMode> newList;

		if (!availablePatches.ContainsKey(modId))
		{
			newList = mode.ToHashSet();
		}
		else
		{
			newList = availablePatches[modId];
			newList.AddRange(mode);
		}

		availablePatches[modId] = newList;
	}

	public override string SettingsCategory() => "SettingsCategory".PrefixTranslate();

	public override void DoSettingsWindowContents(Rect inRect)
	{
		Color defaultColor = GUI.color;
		Listing_Standard listing = new();
		listing.Begin(inRect);

		var pachableMods = LoadedModManager.RunningMods.Where(mod => availablePatches.ContainsKey(mod.PackageId));

		if (pachableMods.Count() == 0)
		{
			listing.Label("NoOptions".PrefixTranslate());
		}
		else
		{
			GUI.color = Color.yellow;
			listing.Label("NeedsRestart".PrefixTranslate());
			
			GUI.color = defaultColor;

			listing.Gap();

			listing.Label("Primer".PrefixTranslate());

			listing.Gap();

			foreach (var mode in Utility.AllEnumValues<PatchMode>())
			{
				listing.Label((mode.ToString() + "Description").PrefixTranslate());
			}

			listing.GapLine();

			foreach (var mod in pachableMods)
			{
				// reference DoMainMenuControls (language) for how the game does float menu buttons
				if (listing.ButtonTextLabeled("ModLabel".PrefixTranslate(mod.Name), settings.compatibilityMode.TryGetValue(mod.PackageId, PatchMode.LightTouch).ToString().PrefixTranslate(), tooltip: ("ModTooltip." + mod.PackageId).PrefixTranslate()))
				{
					List<FloatMenuOption> options = availablePatches.TryGetValue(mod.PackageId, [])
						.OrderBy(mode => mode)
						.Select(mode => new FloatMenuOption(mode.ToString().PrefixTranslate(), () => settings.compatibilityMode[mod.PackageId] = mode))
						.ToList();

					Find.WindowStack.Add(new FloatMenu(options));
				}
			}

		}
		listing.End();
	}
}
