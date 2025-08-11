# Dev note
## Patches
A note for myself: there are several ways to do patches for this mod, not all of them fast. I'm writing them down that I prevent myself from retreading explored ground in the event that I the bright idea to "clean up" the patches again. XPATH profiling is done on RimWorld 1.6 with SmashPhil's [XmlPatchHelper](https://github.com/SmashPhil/XmlPatchHelper] at 100 iterations and a minimal mod list. Results are subject to change from run to run, but the average gives a rough idea how things compare (and ensures that the patch is matching all of the correct nodes).

The problem: RimWorld stores recipes for things under both the `RecipeDef` and `ThingDef/recipeMaker` nodes (internally, recipeMaker is just a renamed RecipeDef), which makes selecting *all* of the recipes to de-generalize somewhat tricky. Naïvely, I could do a patch operation for each branch, but if one of the branches happens to have no results, the game will complain (as is the case for Sculpting in the vanilla game, which only has ThingDef/recipeMaker recipes). Plus, the selection logic for the patch is kind of ugly and I hate repeating myself if I can avoid it.

### Implementations
#### Baseline
```
Matching Defs/RecipeDef[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat
Profiling with 100 iterations.
Matched Nodes: 4
Average: 21602.61 ticks (1.49ms)
```

```
Matching Defs/ThingDef/recipeMaker[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat
Profiling with 100 iterations.
Matched Nodes: 47
Average: 32527.95 ticks (2.72ms)
```


#### 1
```
Matching Defs//*[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat
Profiling with 100 iterations.
Matched Nodes: 51
Average: 1214741.77 ticks (120.94ms)
```

I like how clean this reads, but is by far the slowest due to `//*` effectively trying to explore every node to look for match

#### 2
```
Matching `Defs/*[self::ThingDef or self::RecipeDef]/descendant-or-self::node()[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat`
Profiling with 100 iterations.
Matched Nodes: 51
Average: 464217.84 ticks (45.91ms)
```

This is slightly faster due to being restricted down to just `ThingDef` and `RecipeDef`. I'm not a huge fan of `descendant-or-self`, but since the XPATH 1.0 spec doesn't have a child-or-self this is the next best option.

#### 3
```
Matching `Defs/*[self::ThingDef/recipeMaker or self::RecipeDef]/descendant-or-self::node()[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat`
Profiling with 100 iterations.
Matched Nodes: 51
Average: 182158.55 ticks (17.61ms)
```

Minor change over the previous implementation, but by only selecting ThingDefs with recipes (`self::ThingDef/recipeMaker`), `descendant-or-self` is much more likely to find a hit (the search space shrinks from about 2132 to 261 results (effectively `Defs/ThingDef` vs `Defs/ThingDef/recipeMaker`))

#### 4
```
Matching `Defs/RecipeDef[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat|Defs/ThingDef/recipeMaker[workSpeedStat="GeneralLaborSpeed" and (effectWorking="Smith" or effectWorking="Smelt" or effectWorking="Cook")]/workSpeedStat
Profiling with 100 iterations.
Matched Nodes: 51
Average: 51346.23 ticks (4.71ms)
```

This is technically the fastest implementation (since it's just a union between the two baseline queries), but I hate how ugly it is


### Conclusion
There's no reason to waste computation, but I generally favor readability over speed when it comes to code, so for the time being I'm going to go with the third implementation. Mostly because it allows me to easily tweak the logic without having to update it in multiple places. For similar reasons it also makes it easy to test the patch, since I can just drop the trailing `/workSpeedStat` to see what parent nodes are being operated on

`Disclaimer: averages are measuring timer ticks (not RimWorld ticks)`

## Usages

Core 1.0 usages for reference

### SculptingSpeed
- SculptureBase
- Apparel_WarMask

### SmithingSpeed
- component
- advanced component
- BodyPartProstheticBase
- MakeableShellBase
- shield belt
- SmokepopBelt
- ArmorSmithableBase
- ArmorMachineableBase
- BaseMakeableGun
- BaseMeleeWeapon
- BaseMakeableGrenade
- BaseWeaponNeolithic

### TailoringSpeed
- Make_Patchleather
- ApparelMakeableBase

### GeneralLaborSpeed (1.6)
- BodyPartBionicBase
- BodyPartProstheticMakeableBase
- CremateCorpse
- BurnApparel
- BurnWeapon
- BurnDrugs
- MakeStoneBlocksBase
- Make_ChemfuelFromWood
- Make_ChemfuelFromOrganics
- Make_ComponentIndustrial
- Make_ComponentSpacer
- Make_Patchleather
- ArtBuildingBase
- MusicalInstrumentBase
- MakeableShellBase
- Apparel_ShieldBelt
- Apparel_SmokepopBelt
- Apparel_FirefoampopPack
- Apparel_WarMask
- ApparelMakeableBase
- ArmorSmithableBase
- ArmorMachineableBase
- BaseMakeableGun
- BaseMeleeWeapon
- BaseMakeableGrenade
- BaseWeaponNeolithic