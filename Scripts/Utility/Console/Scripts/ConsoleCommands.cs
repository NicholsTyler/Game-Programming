#region Namespaces
	using UnityEngine;
	using Utility.Tools;
#endregion

namespace Utility.Console
{
    public class ConsoleCommands : Singleton<ConsoleCommands>
    {
    	#region Attributes
			GameObject _player;
			bool _godMode;
		#endregion
		
		#region Unity Methods
			override protected void Awake()
			{
				base.Awake();
				_player = GameObject.FindGameObjectWithTag("Player");
			}
		#endregion

		/* ---------- Commands ----------*/

		#region Basic
			[Command("Displays available commands")]
			public static void Help() => ConsoleManager.Instance.DisplayHelp();

			[Command("Clears console history")]
			public static void Clear() => ConsoleManager.Instance.ClearConsole();

			[Command("Quits the game")]
			public void Quit() => Application.Quit();
		#endregion

		#region Player
			[Command("Kills the player")]
			public void Player_Kill() => _player.GetComponent<IKillable>().Kill();

			[Command("Damages the player by <amount>")]
			public void Player_Damage(float amount = 1) => _player.GetComponent<IDamageable<float>>().Damage(amount);

			[Command("Heals the player by <amount>")]
			public void Player_Heal(float amount = 1) => _player.GetComponent<IHealable<float>>().Heal(amount);

			[Command("Toggles God Mode for the player")]
			public void God()
			{
				_godMode = !_godMode;
				_player.GetComponent<IInvincable>().Invincible = _godMode;
				ConsoleManager.Instance.AddToHistory("God Mode " + (_godMode ? "Active" : "Deactive"));
			}

			[Command("Teleports the player to <x>, <y>, <z>")]
			public void GoTo(float x, float y, float z = 0.0f) => _player.SetPos(x, y, z);
		#endregion

		#region Entities
			[Command("Kills <entity>")]
			public void Kill(GameObject entity) => entity.GetComponent<IKillable>().Kill();

			[Command("Damages <entity> by <amount>")]
			public void Damage(GameObject entity, float amount = 1) => entity.GetComponent<IDamageable<float>>().Damage(amount);

			[Command("Heals <entity> by <amount>")]
			public void Heal(GameObject entity, float amount = 1) => entity.GetComponent<IHealable<float>>().Heal(amount);
		#endregion

		#region Other
			[Command("Sets the time multiplier to <multiplier> where 1.0 = normal")]
			public void TM(float multiplier = 1.0f) => Time.timeScale = multiplier;

			[Command("Takes a screenshot")]
			public void ScreenShot()
			{
				ConsoleManager.Instance.ToggleConsole();
				Global.Capture();
			}
		#endregion
    }
}

#region Credits
	/// Script created by Tyler Nichols
#endregion
