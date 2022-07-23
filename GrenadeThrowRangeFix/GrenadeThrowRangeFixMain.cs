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
        /// Safely disabled mods can be reenabled again. Unsafely disabled mods will need game restart ot take effect.
        /// Unsafely disabled mods usually cannot revert thier changes in OnModDisabled
        public override bool CanSafelyDisable => true;

        public static ModMain Main { get; private set; }

        public new Harmony HarmonyInstance => (Harmony)base.HarmonyInstance;

        /// <summary>
        /// Callback for when mod is enabled. Called even on game startup.
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
        /// Harmony patch that fixes the vanilla throw range calculation.
        /// The attenuation tag allows Harmony to find the targeted class/object method and apply the patch from the following class.
        /// </summary>
        [HarmonyPatch(typeof(Weapon), "GetThrowingRange")]
        /// Class can be any name, but must be static.
        internal static class Weapon_GetThrowingRange_Patch
        {
            /// Using Postfix patch to be guaranteed to get executed.
            public static void Postfix(ref float __result, Weapon __instance, float rangeMultiplier)
            {
                try
                {
                    float num = __instance.TacticalActor.CharacterStats.Endurance * __instance.TacticalActor.TacticalActorDef.EnduranceToThrowMultiplier;
                    float num2 = __instance.TacticalActor.CharacterStats.BonusAttackRange.CalcModValueBasedOn(num);
                    // MadSkunky: Extension of calculation with range multiplier divided by 12 for normalization and multiplier from configuration.
                    num *= __instance.GetDamagePayload().Range / 12f;
                    float multiplier = (Main.Config as GrenadeThrowRangeFixConfig).ThrowRangeMultiplier / 100f;
                    __result = ((num / __instance.Weight * rangeMultiplier) + num2) * multiplier;
                    // End of changes
                }
                catch (Exception e)
                {
                    Main.Logger.LogError("GrenadeThrowRangeFix mod ERROR:\n", e);
                }
            }
        }
    }
}
