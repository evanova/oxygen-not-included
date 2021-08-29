using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace BuildingAutoSwitch {

    public sealed class BuildingAutoSwitch 
    {
        
        [HarmonyPatch(typeof(Toggleable))]
        [HarmonyPatch("QueueToggle")]
        [HarmonyPatch(new Type[] { typeof(int) })]
        public static class ToggleablePatch 
        {
            [HarmonyPrefix]
            public static bool TogglePatch(Toggleable __instance, ref int targetIdx) 
            {
                if (__instance.IsToggleQueued(targetIdx)) 
                {
                    return false;
                }

                FieldInfo info = __instance.GetType().GetField(
                    "targets",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                List<KeyValuePair<IToggleHandler, Chore>> targets = info.GetValue(__instance) as List<KeyValuePair<IToggleHandler, Chore>>;
                targets[targetIdx].Key.HandleToggle();
                return false;
            }
        }
    }
}