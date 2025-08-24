## Changes
v2.0.0
- Updated selection patches, they can now target both ThingDef/recipeMaker and RecipeDefs based definitions and are no longer restricted to specific workbenches (nothing can hide from the gaze of my XPATH!)
- Added custom code to allow the user to choose how aggressively to patch other mods
- Vanilla Skills Expanded patches tweaked to use new custom patch code
	- Light touch mode will keep duplicate Sculpting and Tailoring stats (adjusting them to fall under the De-generalized stats instead of Global Work Speed) and only target recipes that VS skills hasn't touched.
	- Best guess mode is will remove the duplicate stats and attempt to supplant them with the stats with the ones defined by this mod (how the mod was configured in previous versions). This should almost always work, but may throw errors if another mod attempts to use one of the removed VE stats (and I don't have a patch for it)
	- Heavy handed mode does the same thing as Best guess mode, but uses a recursive patch that should find and replace all instances of the removed VE stats. It is unlikely to cause errors, but is orders of magnitude slower that Best guess (and will only be worse when used with large mod lists). I am including these, but commenting them out by default, so that people don't turn them on and then complain that the mod is slow...
- Added compatibility for FrozenSnowFox's Vanilla Bionics Expansion and Advanced Bionics Expansion

v1.0.6
- v1.6 update (no changes needed)

v1.0.5
- v1.5 update (no changes needed)

v1.0.4
- Vanilla Skills Expanded compatibility

v1.0.3
- v1.4 update (no changes needed)

v1.0.2
- v1.3 update (no changes needed)

v1.0.1
- v1.2 update

v1.0.0
- Initial release