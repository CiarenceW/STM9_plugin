using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Receiver2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace STM9_plugin
{
    [BepInPlugin("Ciarencew.MagReplacer", "Dreaming Magazine Replacer", "1.0.0")]
    public class MagReplacer : BaseUnityPlugin
    {
        public static float small_mag_chance = 0.7f;

        private void Awake()
        {
            Logger.LogInfo($"Plugin Dreaming Magazine Replacer is loaded!");

            Harmony.CreateAndPatchAll(this.GetType());
        }

        [HarmonyPatch(typeof(RuntimeTileLevelGenerator), 
            nameof(RuntimeTileLevelGenerator.instance.InstantiateMagazine),
            new[] {typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(MagazineClass)}
            )]
        [HarmonyPostfix]
        private static void patchInstantiateMagazine(ref GameObject __result)
        {
            var RCS = ReceiverCoreScript.Instance();
            if (RCS.CurrentLoadout.gun_internal_name != "Ciarencew.STM9") return; //this whole thing should only be active when the STM-9 is held, but still.
            if (Probability.Chance(small_mag_chance)) return;

            GameObject magObj;
            magObj = __result;
            Destroy(__result);

            magObj.name = "Hey! this shouldn't show up, I mean you shouldn't even see this, at all.";

            var magScript = magObj.GetComponent<MagazineScript>();
            RCS.TryGetMagazinePrefabFromRoot("stm9_mag", magScript.MagazineClass, out var magPrefab);
            var replacedMag = RuntimeTileLevelGenerator.instance.InstantiateMagazine(magObj.transform.position, magObj.transform.rotation, magObj.transform.parent, magPrefab);
            replacedMag.GetComponent<MagazineScript>().SetRoundCount(UnityEngine.Random.Range(0, magScript.kMaxRounds));
        }
    }
}
