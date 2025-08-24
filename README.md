# De-generalize Work
RimWorld version 1.1 lumped SculptingSpeed, SmithingSpeed and TailoringSpeed into one stat (GeneralLaborSpeed), unfortunately this change is a huge blow to trait mods (including my own Additional Traits mod) that used those stats to give the game just a little bit more depth. As such, this mod adds those stats back as dependents of GeneralLaborSpeed, this means that changes to GeneralLaborSpeed will be passed down to SculptingSpeed, SmeltingSpeed, SmithingSpeed and TailorSpeed, thus (hopefully) ensuring compatibility.

## Incompatibilities & Interactions:
Vanilla Skills Expanded - also adds stats for Tailoring and Sculpting, this mod is configured with three patch mods for handling these duplicates:
	- Light touch mode will keep duplicate stats (adjusting them to fall under the De-generalized stats instead of Global Work Speed) and only target recipes that VS skills hasn't touched.
	- Best guess mode is will remove the duplicate stats and attempt to supplant them with the stats with the ones defined by this mod (how the mod was configured in previous versions). This should almost always work, but may throw errors if another mod attempts to use one of the removed VE stats (and I don't have a patch for it)
	- Heavy handed mode does the same thing as Best guess mode, but uses a recursive patch that should find and replace all instances of the removed VE stats. It is unlikely to cause errors, but is orders of magnitude slower that Best guess (and will only be worse when used with large mod lists). I am including these, but commenting them out by default, so that people don't turn them on and then complain that the mod is slow...

FrozenSnowFox's Vanilla Bionics Expansion and Advanced Bionics Expansion (with VE Skills)
	- Adjusts FSFBionicsCraftingSpeed to fall under SmithingSpeed
	- Replaces VSE_TailoringSpeed and VSE_ArtSpeed with De-generalized stats when VE Skills is in Best Guess or Heavy handed mode

## Modders
See the [project wiki](https://github.com/Alias44/Degeneralize-Work/wiki/Modding-usage) for a quick start integration guide 

## Licensing
This mod is licensed under the GNU General Public License v2.0

## Thanks
* to all the wonderful people on the RimWorld Discord

## Links
[Steam link](https://steamcommunity.com/sharedfiles/filedetails/?id=2011655761)
