using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BasicDropdownsReplaceStuffPatch;

[HarmonyPatch]
public static class Patch_DesignatorBuildDropdownStuffFix
{
    private const string SanityCheckTypeName = "Replace_Stuff.CoolersOverWalls.DesignatorBuildDropdownStuffFix";

    private static readonly string[] OverWallDefNames =
    {
        "Cooler_Over",
        "Cooler_Over2W",
        "Vent_Over",
        "Vent_Over2W",
    };

    public static bool Prepare()
    {
        var type = AccessTools.TypeByName(SanityCheckTypeName);
        if (type != null && AccessTools.DeclaredMethod(type, "SanityCheck") != null)
            return true;

        Log.Warning("[Basic Dropdowns Replace Stuff Patch] Could not find Replace Stuff - Continued SanityCheck; dropdown unpacking is unchanged.");
        return false;
    }

    public static MethodBase TargetMethod()
    {
        return AccessTools.DeclaredMethod(AccessTools.TypeByName(SanityCheckTypeName), "SanityCheck");
    }

    public static bool Prefix()
    {
        if (ModLister.GetActiveModWithIdentifier("cedaro.material.submenu", true) != null)
            return false;

        UnpackOnlyReplaceStuffOverWallDropdowns();
        return false;
    }

    private static void UnpackOnlyReplaceStuffOverWallDropdowns()
    {
        foreach (var catDef in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
        {
            var designators = catDef.AllResolvedDesignators;
            for (var i = 0; i < designators.Count; i++)
            {
                if (designators[i] is not Designator_Dropdown dropdown)
                    continue;
                if (!dropdown.Elements.All(IsReplaceStuffOverWallDesignator))
                    continue;
                if (!dropdown.Elements.Any(d => d is Designator_Build build && build.PlacingDef.MadeFromStuff))
                    continue;

                designators.RemoveAt(i);
                foreach (var element in dropdown.Elements)
                    designators.Insert(i, element);
            }
        }
    }

    private static bool IsReplaceStuffOverWallDesignator(Designator designator)
    {
        return designator is Designator_Build build
            && build.PlacingDef is ThingDef def
            && OverWallDefNames.Contains(def.defName);
    }
}
