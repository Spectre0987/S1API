using HarmonyLib;
using MelonLoader;

#if IL2CPPMELON || IL2CPPBEPINEX
using S1Time = Il2CppScheduleOne.GameTime;
#else
using S1Time = ScheduleOne.GameTime;
#endif

namespace S1API.Internal.Patches
{
    [HarmonyPatch(typeof(S1Time.TimeManager))]
    internal class TimePatches
    {

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void OnTimeInit(S1Time.TimeManager __instance) {
            GameTime.TimeManager.Setup(__instance);
        }

    }
}
