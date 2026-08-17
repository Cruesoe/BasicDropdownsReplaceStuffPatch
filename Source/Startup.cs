using HarmonyLib;
using Verse;

namespace BasicDropdownsReplaceStuffPatch;

[StaticConstructorOnStartup]
public static class Startup
{
    static Startup()
    {
        new Harmony("cruesoe.basicdropdownsreplacestuffpatch").PatchAll();
    }
}
