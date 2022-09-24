using System.Collections.Generic;
using HarmonyLib;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;
using UnityEngine;

public class JintPatch
{
    public static void DoPatching()
    {
        var harmony = new Harmony("me.top-cat.cmjs");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(ObjectInstance))]
[HarmonyPatch("Delete")]
class DeleteProperty
{
    public static bool Prefix(ObjectInstance __instance, JsValue property, ref bool __result)
    {
        if (__instance is ObjectWrapper wrapper)
        {
            if (wrapper.Target is JSONWrapper jsonWraper)
            {
                jsonWraper.DeleteProperty(property);
                __result = true;
                return false;
            }
        }

        return true;
    }
}

[HarmonyPatch(typeof(ObjectInstance))]
[HarmonyPatch("GetOwnPropertyKeys")]
class GetOwnPropertyKeys
{
    public static bool Prefix(ObjectInstance __instance, ref List<JsValue> __result)
    {
        if (__instance is ObjectWrapper wrapper)
        {
            if (wrapper.Target is JSONWrapper jsonWraper)
            {
                __result = new List<JsValue>();
                foreach (var wrappedKey in jsonWraper.wrapped.Keys)
                {
                    __result.Add(wrappedKey);
                }

                return false;
            }
        }

        return true;
    }
}
