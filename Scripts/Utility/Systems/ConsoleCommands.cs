#region Namespaces
using UnityEngine;
using Utility.Tools;
#endregion
namespace Utility.Systems
{
    /// <summary> Commands used by the console </summary>
    public class ConsoleCommands : MonoBehaviour
    {
        #region Variables
        GameObject _player;  // The player
        bool godMode;
        #endregion

        #region Commands
        [Command("Toggles God Mode for the player")]
        public void God() 
        { 
            godMode = !godMode; 
            _player.GetComponent<IGodable>().God(godMode);
            if (godMode) 
                Console.instance.AddToHistory("God Mode Active");
            else
                Console.instance.AddToHistory("God Mode Deactive");
        }

        [Command("Quits the game")]
        public void Quit() => Application.Quit();

        [Command("Sets the time multiplier to <multiplier> where 1.0 = normal")]
        public void TM(float multiplier = 1.0f) => Time.timeScale = multiplier;

        [Command("Teleports the player to <x>, <y>, <z>")]
        public void GoTo(float x = 0.0f, float y = 0.0f, float z = 0.0f) => _player.SetPos(x, y, z);

        #endregion

        #region Methods
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        #endregion
    }
}
#region Credits
/// Script created by Tyler Nichols
#endregion
