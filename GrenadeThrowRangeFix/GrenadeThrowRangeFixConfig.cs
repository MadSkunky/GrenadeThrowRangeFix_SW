using PhoenixPoint.Modding;

namespace GrenadeThrowRangeFix
{
	/// <summary>
	/// ModConfig is mod settings that players can change from within the game.
	/// Config is only editable from players in main menu.
	/// Only one config can exist per mod assembly.
	/// Config is serialized on disk as json.
	/// </summary>
	public class GrenadeThrowRangeFixConfig : ModConfig
	{
        /// Only public fields are serialized.
        [ConfigField(text: "Throwing range multiplier",
            description: "Multiplier (in %) to adjust the throwing range.\nDefault 100 = with 20 strength the throwing range is equal to the defind range of the grenade (see Info screen of the grenade).\nLower numbers will decrease the range of all grenades, higher numbers do the opposite.")]
		public int ThrowRangeMultiplier = 100;

		/// Supported types for in-game UI are:
		//public int IntegerValue;
		//public float FloatValue;
		//public bool BoolValue;
		//
		//public enum CustomEnum
		//{
		//	A, B ,C
		//}
		//public CustomEnum CustomEnumValue;
	}
}
