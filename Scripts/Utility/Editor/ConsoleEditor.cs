#region Namespaces
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Utility.Tools;
#endregion
namespace Utility.Systems
{
    /// <summary> Creates a custom inspector for Console.cs </summary>
    [CustomEditor(typeof(Console))]
    public class ConsoleEditor : Editor
    {
        #region Variables
        /* Command Categories */
        enum DisplayCategory { Basic, Player, Entities, All }
        DisplayCategory categoryToDisplay;

        /* Info Sections */
        enum DisplaySection { General, Setup, Commands, Controls }
        DisplaySection sectionToDisplay;

        /* Toggle States */
        bool showSettings;
        bool showInfo;

        /* Basic Toggle States */
        bool helpToggle;
        bool clearToggle;

        /* Player Toggle States */
        bool pKillToggle;
        bool pDmgToggle;
        bool pHealToggle;

        /* Entity Toggle States */
        bool eKillToggle;
        bool eDmgToggle;
        bool eHealToggle;

        /* Other */
        const string FilePath = "Assets/Utility/Systems/ConsoleCommands.cs";
        #endregion

        #region Unity Methods
        public override void OnInspectorGUI()
        {
            showInfo = EditorGUILayout.Foldout(showInfo, "Information", true, EditorStyles.foldoutHeader);

            if (showInfo)
            {
                EditorGUILayout.Space();
                Information();
            }

            EditorGUILayout.Space(10);

            showSettings = EditorGUILayout.Foldout(showSettings, "Console Settings", true, EditorStyles.foldoutHeader);

            if (showSettings)
            {
                EditorGUILayout.Space();
                CommandsToGenerate();
            }

            EditorGUILayout.Space(10);

            // Saves to the inspector
            serializedObject.ApplyModifiedProperties();

            // Generates the Commands Script
            GenerateScript();
        }

        void OnEnable()
        {
            /* Basic */
            helpToggle = EditorPrefs.GetBool("HelpToggle", true);
            clearToggle = EditorPrefs.GetBool("ClearToggle", true);

            /* Player */
            pKillToggle = EditorPrefs.GetBool("PlayerKillToggle", false);
            pDmgToggle = EditorPrefs.GetBool("PlayerDamageToggle", false);
            pHealToggle = EditorPrefs.GetBool("PlayerHealToggle", false);

            /* Entities */
            eKillToggle = EditorPrefs.GetBool("EntityKillToggle", false);
            eDmgToggle = EditorPrefs.GetBool("EntityDamageToggle", false);
            eHealToggle = EditorPrefs.GetBool("EntityHealToggle", false);
        }
        #endregion

        #region Custom Methods
        void DisplayBasic()
        {
            // Help Command
            helpToggle = EditorGUILayout.Toggle(new GUIContent("Help", "Displays all available commands"), helpToggle);
            EditorPrefs.SetBool("HelpToggle", helpToggle);

            // Clear Command
            clearToggle = EditorGUILayout.Toggle(new GUIContent("Clear", "Clears the console"), clearToggle);
            EditorPrefs.SetBool("ClearToggle", clearToggle);
        }
        void DisplayPlayer()
        {
            // Kill Command
            pKillToggle = EditorGUILayout.Toggle(new GUIContent("Player_Kill", "Kills the player"), pKillToggle);
            EditorPrefs.SetBool("PlayerKillToggle", pKillToggle);

            // Damage Command
            pDmgToggle = EditorGUILayout.Toggle(new GUIContent("Player_Damage <amount>", "Damages the player by <amount>"), pDmgToggle);
            EditorPrefs.SetBool("PlayerDamageToggle", pDmgToggle);

            // Heal Command
            pHealToggle = EditorGUILayout.Toggle(new GUIContent("Player_Heal <amount>", "Heals the player by <amount>"), pHealToggle);
            EditorPrefs.SetBool("PlayerHealToggle", pHealToggle);
        }
        void DisplayEntities()
        {
            // Kill Command
            eKillToggle = EditorGUILayout.Toggle(new GUIContent("Kill", "Kills <entity>"), eKillToggle);
            EditorPrefs.SetBool("EntityKillToggle", eKillToggle);

            // Damage Command
            eDmgToggle = EditorGUILayout.Toggle(new GUIContent("Damage <entity> <amount>", "Damages <entity> by <amount>"), eDmgToggle);
            EditorPrefs.SetBool("EntityDamageToggle", eDmgToggle);

            // Heal Command
            eHealToggle = EditorGUILayout.Toggle(new GUIContent("Heal <entity> <amount>", "Heals <entity> by <amount>"), eHealToggle);
            EditorPrefs.SetBool("EntityHealToggle", eHealToggle);
        }

        void Information()
        {
            // Displays the sections of information
            sectionToDisplay = (DisplaySection)EditorGUILayout.EnumPopup("Sections", sectionToDisplay);

            EditorGUILayout.Space();

            // Determine what section to display
            switch (sectionToDisplay)
            {
                case DisplaySection.General:
                    EditorGUILayout.LabelField("Features", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("- Support for an endless number of commands");
                    EditorGUILayout.LabelField("- Pre-generated popular commands");
                    EditorGUILayout.LabelField("- Extremely easy to create custom commands");
                    EditorGUILayout.LabelField("- Any number of parameters on commands");
                    EditorGUILayout.LabelField("- Navigate through command history");
                    EditorGUILayout.LabelField("- Auto-complete");
                    EditorGUILayout.Space(20);
                    EditorGUILayout.LabelField("Note: This console uses Unity's new input system");
                    EditorGUILayout.LabelField("Created by Tyler Nichols");
                    break;
                case DisplaySection.Setup:
                    EditorGUILayout.LabelField("First Time Setup", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("1.) Expand the console settings below");
                    EditorGUILayout.LabelField("            - Choose which pre-generated commands to use");
                    EditorGUILayout.LabelField("            - Hover a command for more information");
                    EditorGUILayout.LabelField("2.) Click \"Generate Script\"");
                    break;
                case DisplaySection.Commands:
                    EditorGUILayout.LabelField("Creating Custom Commands", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("1.) Open the generated script");
                    EditorGUILayout.LabelField("            - Assets/Utility/Systems/ConsoleCommands.cs");
                    EditorGUILayout.LabelField("2.) Expand the \"Commands\" region");
                    EditorGUILayout.LabelField("3.) Use the following pattern:");
                    EditorGUILayout.LabelField("            - Note: Parameters are not required");
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("--------------------------------------------------");
                    EditorGUILayout.LabelField("            [Command(\"DEFINITION\")]");
                    EditorGUILayout.LabelField("            public void NAME (PARAMS)");
                    EditorGUILayout.LabelField("            {");
                    EditorGUILayout.LabelField("                    FUNCTION");
                    EditorGUILayout.LabelField("            }");
                    EditorGUILayout.LabelField("--------------------------------------------------");
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Supported Parameter types:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("int, float, bool, GameObject, Color, String");
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("To Display Text in the Console:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Console.instance.AddToHistory(\"TEXT\");");
                    break;
                case DisplaySection.Controls:
                    EditorGUILayout.LabelField("Open/Close Console = (~)");
                    EditorGUILayout.LabelField("Auto-Complete command = (Tab)");
                    EditorGUILayout.LabelField("Navigate command history = (Up/Down Arrow Keys)");     
                    break;
            }
        }
        void GenerateScript()
        {
            if (GUILayout.Button("Generate Script"))
            {
                if (File.Exists(FilePath))
                {
                    Debug.LogError("Script was not generated to prevent custom commands from being overwritten.");
                    Debug.LogWarning("If you really want to do this, delete the generated script and try again.");
                }
                else
                {
                    string classContent = InitCommands();
                    File.WriteAllText(FilePath, classContent);
                    AssetDatabase.ImportAsset(FilePath);
                }
            }
        }
        void CommandsToGenerate()
        {
            // Displays the category of Commands to include
            categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Commands to Generate", categoryToDisplay);

            EditorGUILayout.Space();

            // Determine what category to display
            switch (categoryToDisplay)
            {
                case DisplayCategory.Basic:
                    DisplayBasic();
                    break;

                case DisplayCategory.Player:
                    DisplayPlayer();
                    break;

                case DisplayCategory.Entities:
                    DisplayEntities();
                    break;

                case DisplayCategory.All:
                    DisplayBasic();
                    DisplayPlayer();
                    DisplayEntities();
                    break;
            }
        }
        string InitCommands()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ScriptFirst);

            /* Note */
            if (pKillToggle || pDmgToggle || pHealToggle ||
                eKillToggle || eDmgToggle || eHealToggle)
            {
                sb.Append(ScriptNote);
            }

            /* References */
            bool pEnabled = pKillToggle || pDmgToggle || pHealToggle;
            if (pEnabled) { sb.Append(Player_Ref); }

            sb.Append(ScriptSecond);

            /* Basic Commands */
            if (helpToggle) { sb.Append(Help); }
            if (clearToggle) { sb.Append(Clear); }

            /* Player Commands */
            if (pKillToggle) { sb.Append(Player_Kill); }
            if (pDmgToggle) { sb.Append(Player_Damage); }
            if (pHealToggle) { sb.Append(Player_Heal); }

            /* Entity Commands */
            if (eKillToggle) { sb.Append(Kill); }
            if (eDmgToggle) { sb.Append(Damage); }
            if (eHealToggle) { sb.Append(Heal); }

            sb.Append(ScriptThird);
            bool awakeEnabled = pKillToggle || pDmgToggle || pHealToggle;
            if (awakeEnabled) { sb.Append(ScriptFourth); }

            /* Methods */
            if (pEnabled) { sb.Append(Player_Init); }

            if (awakeEnabled) { sb.Append(ScriptFifth); }
            sb.Append(ScriptSixth);
            return sb.ToString();
        }
        #endregion

        #region Strings
const string ScriptFirst =
@"#region Namespaces
using UnityEngine;
using Utility.Tools;
#endregion
namespace Utility.Systems
{
    /// <summary> Commands used by the console </summary>
    public class ConsoleCommands : MonoBehaviour
    {
        #region References";

const string ScriptNote =
@"
        // Note: 1 or more of your commands implement interfaces
        // It's essential to include [using Utility.Tools;] in the classes that will use them

        // Example of a Player class implementing damage_player:

        // using UnityEngine;
        // using Utility.Tools;
        // public class Player : MonoBehaviour, IDamageable<float>
        // {
        //      public void Damage(float amount) => Debug.Log(""Damaged by "" + amount);
        // }
";

const string ScriptSecond =
@"
        #endregion

        #region Commands";

const string ScriptThird =
@"  
        #endregion

        #region Methods";

const string ScriptFourth =
@"
        void Awake()
        {";

const string ScriptFifth =
@"
        }";

const string ScriptSixth =
@"
        #endregion
    }
}
#region Credits
/// Script created by Tyler Nichols
#endregion
";

const string Help =
@"
        [Command(""Displays available commands"")]
        public static void Help() => Console.instance.DisplayHelp();
";

const string Clear =
@"
        [Command(""Clears console history"")]
        public static void Clear() => Console.instance.ClearConsole();
";

const string Player_Ref =
@"
        GameObject _player;  // The player";

const string Player_Init =
@"
            _player = GameObject.FindGameObjectWithTag(""Player"");";

const string Player_Kill =
@"
        [Command(""Kills the player"")]
        public void Player_Kill() => _player.GetComponent<IKillable>().Kill();
";

const string Player_Damage =
@"
        [Command(""Damages the player by <amount>"")]
        public void Player_Damage(float amount) => _player.GetComponent<IDamageable<float>>().Damage(amount);
";

const string Player_Heal =
@"
        [Command(""Heals the player by <amount>"")]
        public void Player_Heal(float amount) => _player.GetComponent<IHealable<float>>().Heal(amount);
";

const string Kill =
@"
        [Command(""Kills <entity>"")]
        public void Kill(GameObject entity) => entity.GetComponent<IKillable>().Kill();
";

const string Damage =
@"
        [Command(""Damages<entity> by<amount>"")]
        public void Damage(GameObject entity, float amount) => entity.GetComponent<IDamageable<float>>().Damage(amount);
";

const string Heal =
@"
        [Command(""Heals<entity> by<amount>"")]
        public void Heal(GameObject entity, float amount) => entity.GetComponent<IHealable<float>>().Heal(amount);
";
        #endregion
    }
}

#region Credits
/// Script created by Tyler Nichols
#endregion