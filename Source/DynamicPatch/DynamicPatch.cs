using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AliasDynamicPatch;

public class DynamicPatch(ModContentPack content) : Mod(content)
{
	public Settings settings;
	protected string translatePrefix;

	/// <summary>
	/// Stores the union of all <see cref="PatchOperationModSequence.appliesTo"/> from each patch loaded (or in other words,
	/// the set of <see cref="PatchMode"/>(s) available for a given mod).<br/>
	/// Used to drive available settings in menu.
	/// </summary>
	/// <remarks>this should every patche, even if it is not applied due to PatchMode criteria</remarks>
	PackageMode packageMode = new();

	public void AddMode(PackageMode patch)
	{
		packageMode.Add(patch);
	}

	/// <remarks>
	/// Dev note: reference <see cref="MainMenuDrawer.DoMainMenuControls(Rect, bool)">DoMainMenuControls</see> (language) for how the base game does float menu buttons
	/// </remarks>
	public override void DoSettingsWindowContents(Rect inRect)
	{
		Color defaultColor = GUI.color;
		Listing_Standard listing = new();
		listing.Begin(inRect);

		var pachableMods = packageMode.ConfigurableMods;

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
				if (listing.ButtonTextLabeled("ModLabel".PrefixTranslate(mod.mod.Name), settings.GetMode(mod.mod.PackageId).ToString().PrefixTranslate(), tooltip: $"{translatePrefix}.ModTooltip.{mod.mod.PackageId}".Translate()))
				{
					List<FloatMenuOption> options = mod.modes
						.OrderBy(mode => mode)
						.Select(mode => new FloatMenuOption(mode.ToString().PrefixTranslate(), () => settings.SetMode(mod.mod.PackageId, mode)))
						.ToList();

					Find.WindowStack.Add(new FloatMenu(options));
				}
			}

		}
		listing.End();
	}
}
