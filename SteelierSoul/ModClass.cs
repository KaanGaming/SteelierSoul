using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace SteelierSoul
{
    public class SteelierSoul : Mod, ITogglableMod
    {
        internal static SteelierSoul Instance;

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        //public SteelierSoul() : base("SteelierSoul")
        //{
        //    Instance = this;
        //}

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            On.HeroController.Die += HeroController_Die;
            On.HeroController.Respawn += HeroController_Respawn;

            Log("Initialized");
        }

        public void Unload()
        {
            On.HeroController.Die -= HeroController_Die;
            On.HeroController.Respawn -= HeroController_Respawn;

            Log("Unloaded");
        }

        public override string GetVersion() => "1.0.0";

        GlobalEnums.MapZone prevMapZone = GlobalEnums.MapZone.WYRMSKIN;

        private IEnumerator HeroController_Respawn(On.HeroController.orig_Respawn orig, HeroController self)
        {
            if (prevMapZone == GlobalEnums.MapZone.WYRMSKIN)
            {
                LogDebug("Previous map zone wasn't set, running orig");
                return orig(self);
            }
            var mz = GameManager.instance.sm.mapZone;
            if (mz == GlobalEnums.MapZone.NONE)
            {
                GameManager.instance.sm.mapZone = prevMapZone;
                LogDebug($"Player respawned, previous map zone was {prevMapZone}");
            }
            return orig(self);
        }

        private IEnumerator HeroController_Die(On.HeroController.orig_Die orig, HeroController self)
        {
            var mz = GameManager.instance.sm.mapZone;
            prevMapZone = mz;
            LogDebug($"Player died, previous map zone was {prevMapZone}");
            if (mz == GlobalEnums.MapZone.DREAM_WORLD || mz == GlobalEnums.MapZone.GODS_GLORY)
            {
                LogDebug($"Death detected in dream ({mz})");
                GameManager.instance.sm.mapZone = GlobalEnums.MapZone.NONE;
            }
            return orig(self);
        }
    }
}