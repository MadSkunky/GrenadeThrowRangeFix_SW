using Base.Levels;
using HarmonyLib;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;

namespace GrenadeThrowRangeFix
{
    /// <summary>
    /// This is the main mod class. Only one can exist per assembly.
    /// If no ModMain is detected in assembly, then no other classes/callbacks will be called.
    /// </summary>
    public class GrenadeThrowRangeFixMain : ModMain
    {
        /// Config is accessible at any time, if any is declared.
        public new GrenadeThrowRangeFixConfig Config => (GrenadeThrowRangeFixConfig)base.Config;

        /// This property indicates if mod can be Safely Disabled from the game.
        /// Safely sisabled mods can be reenabled again. Unsafely disabled mods will need game restart ot take effect.
        /// Unsafely disabled mods usually cannot revert thier changes in OnModDisabled
        public override bool CanSafelyDisable => true;

        public static ModMain Main { get; private set; }

        public new Harmony HarmonyInstance => (Harmony)base.HarmonyInstance;

        /// <summary>
        /// Callback for when mod is enabled. Called even on game starup.
        /// </summary>
        public override void OnModEnabled()
        {
            Main = this;
            HarmonyInstance.PatchAll(GetType().Assembly);
        }

        /// <summary>
        /// Callback for when mod is disabled. This will be called even if mod cannot be safely disabled.
        /// Guaranteed to have OnModEnabled before.
        /// </summary>
        public override void OnModDisabled()
        {
            /// Undo any game modifications if possible. Else "CanSafelyDisable" must be set to false.
            /// ModGO will be destroyed after OnModDisabled.
            HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
            Main = null;
        }

        /// <summary>
        /// Callback for when any property from mod's config is changed.
        /// </summary>
        public override void OnConfigChanged()
        {
            /// Config is accessible at any time.
        }


        /// <summary>
        /// In Phoenix Point there can be only one active level at a time. 
        /// Levels go through different states (loading, unloaded, start, etc.).
        /// General puprose level state change callback.
        /// </summary>
        /// <param name="level">Level being changed.</param>
        /// <param name="prevState">Old state of the level.</param>
        /// <param name="state">New state of the level.</param>
        public override void OnLevelStateChanged(Level level, Level.State prevState, Level.State state)
        {
            /// Alternative way to access current level at any time.
            //Level l = GetLevel();
        }

        /// <summary>
        /// Useful callback for when level is loaded, ready, and starts.
        /// Usually game setup is executed.
        /// </summary>
        /// <param name="level">Level that starts.</param>
        public override void OnLevelStart(Level level)
        {
        }

        /// <summary>
        /// Useful callback for when level is ending, before unloading.
        /// Usually game cleanup is executed.
        /// </summary>
        /// <param name="level">Level that ends.</param>
        public override void OnLevelEnd(Level level)
        {
        }

        // This "tag" allows Harmony to find this class and apply it as a patch.
        [HarmonyPatch(typeof(Weapon), "GetThrowingRange")]
        // Class can be any name, but must be static.
        internal static class GetThrowingRange_fix
        {
            // Overwrite original fuction to calculate throwing range for grenades.
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static bool Prefix(ref float __result, Weapon __instance, float rangeMultiplier)
            {
                try
                {
                    float num = __instance.TacticalActor.CharacterStats.Endurance * __instance.TacticalActor.TacticalActorDef.EnduranceToThrowMultiplier;
                    float num2 = __instance.TacticalActor.CharacterStats.BonusAttackRange.CalcModValueBasedOn(num);
                    // MadSkunky: adding range multiplier and divisor
                    num *= __instance.GetDamagePayload().Range / 12f;
                    float multiplierPerc = (Main.Config as GrenadeThrowRangeFixConfig).ThrowRangeMultiplier / 100f;
                    __result = ((num / __instance.Weight * rangeMultiplier) + num2) * multiplierPerc;
                    // End of changes, the rest is vanilla code
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }
    }
}
