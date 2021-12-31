/* NOTE: THIS SCRIPT USES UNITY'S NEW INPUT SYSTEM
 * Create an Input Actions Asset with the following actions:
 * ToggleDebug [I prefer the keybind as (`)]
 * Return [I prefer the keybind as (Enter)]
 */
#region Namespaces
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
#endregion

/// <summary> Implements custom debug console commands </summary>
public class DebugCommands
{
    #region Default Commands
    [ConsoleCommand("Displays available commands")]
    public static void HELP() => DebugController.instance.DisplayHelp();

    [ConsoleCommand("Clears console history")]
    public static void CLEAR() => DebugController.instance.ClearConsole();

    [ConsoleCommand("Spams a <WORD> <X> times in console")]
    public static void SPAM(string WORD, int X)
    {
        for (int i=0; i<X; i++)
            DebugController.instance.AddToHistory(WORD);
    }
    #endregion

    #region Other Commands
    /* CREATE COMMANDS HERE!! */
    #endregion
}






/* -------------------- IGNORE BELOW HERE -------------------- */




#region Implementation Classes
/// <summary> Draws Debug Console & Handles Commands </summary>
[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
public class DebugController : MonoBehaviour
{
    #region Attributes
    // Singleton
    public static DebugController instance;
    public static DebugCommands cmdInstance;

    Dictionary<string, (string, MethodInfo)> consoleMethods;

    bool showConsole;
    bool showHelp;

    // User Input
    string input;
    Vector2 helpScroll;
    Vector2 scroll;
    #endregion

    #region GUI Attributes
    // Console
    float verticalSpacing = 30;

    float historyLength = 150;
    List<string> historyInput;

    float lineHeight = 25;
    float inputFieldOffset = 10;
    float padding = 5;
    #endregion

    #region Unity Methods
    void Awake()
    {
        // Create singleton
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Debug.LogError("Multiple instances of DebugController"); }

        // Initialize Data Structures
        consoleMethods = new Dictionary<string, (string, MethodInfo)>();
        historyInput = new List<string>();
    }

    void Start()
    {
        InitConsoleCommands();
    }
    void OnGUI()
    {
        // Only draw the GUI if console is enabled
        if (!showConsole) return;
        float y = 0f;

        // Help GUI
        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, historyLength), "");

            Rect viewportHelp = new Rect(0, 0, Screen.width - verticalSpacing, lineHeight * consoleMethods.Count);
            helpScroll = GUI.BeginScrollView(new Rect(0, y + padding, Screen.width, historyLength - inputFieldOffset), helpScroll, viewportHelp);

            Rect labelRect = new Rect(padding, 0, viewportHelp.width - historyLength, lineHeight);
            GUI.Label(labelRect, "Debug Console Commands");
            labelRect = new Rect(padding, lineHeight, viewportHelp.width - historyLength, lineHeight);
            GUI.Label(labelRect, "--------------------------------------------------------------------------------------------");

            int j = 2;
            foreach (var pair in consoleMethods)
            {
                ParameterInfo[] parameters = pair.Value.Item2.GetParameters();

                // Create the help text to display
                string key = pair.Key;
                if (parameters.Length > 0)
                {
                    for (int k = 0; k < parameters.Length; k++)
                    {
                        key += " <" + parameters[k].Name + "> ";
                    }
                }
                string label = $"{key} - {pair.Value.Item1}";

                labelRect = new Rect(padding, lineHeight * j, viewportHelp.width - historyLength, lineHeight);
                GUI.Label(labelRect, label);
                j++;
            }

            GUI.EndScrollView();
            y += historyLength;
        }

        // Chat History
        GUI.Box(new Rect(0, y, Screen.width, historyLength), "");

        Rect viewport = new Rect(0, 0, Screen.width - verticalSpacing, lineHeight * historyInput.Count);
        scroll = GUI.BeginScrollView(new Rect(0, y + padding, Screen.width, historyLength - inputFieldOffset), scroll, viewport);

        int i = 0;
        foreach (var cmd in historyInput)
        {
            Rect labelRect = new Rect(padding, lineHeight * i, viewport.width - historyLength, lineHeight);
            GUI.Label(labelRect, cmd);
            i++;
        }

        GUI.EndScrollView();
        y += historyLength;

        // Draws console box at the top of the screen
        GUI.Box(new Rect(0, y, Screen.width, verticalSpacing), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0.25f);

        // Allows user to type in console
        GUI.SetNextControlName("Input");
        input = GUI.TextField(new Rect(inputFieldOffset, y + padding, Screen.width - lineHeight, lineHeight), input);
        GUI.FocusControl("Input");
    }
    #endregion

    #region Custom Methods
    public void OnToggleDebug(UnityEngine.InputSystem.InputValue value)
    {
        showConsole = !showConsole;
        input = "";
    }
    public void OnReturn(UnityEngine.InputSystem.InputValue value)
    {
        if (!showConsole) { return; }
        HandleInput();
        input = "";
    }
    public void DisplayHelp() => showHelp = !showHelp;
    public void ClearConsole()
    {
        historyInput.Clear();
        showHelp = false;
    }
    public void AddToHistory(string input)
    {
        historyInput.Add(input);
        scroll = new Vector2(0, historyInput.Count * lineHeight);
    }

    void HandleInput()
    {
        // Verify input was not null or whitespace
        if (string.IsNullOrWhiteSpace(input)) { return; }

        // Add this input to console history
        AddToHistory(input);

        // Make the input all upper case
        input = input.ToUpper();

        // Separate id from parameters
        string[] properties = input.Split(' ');
            
        // Loop through all id / method pairs
        foreach(var pair in consoleMethods)
        {
            if (properties[0] != pair.Key.ToUpper()) { continue; }

            MethodInfo mInfo = pair.Value.Item2;
            ParameterInfo[] parameters = mInfo.GetParameters();
                
            // Invoke method if no parameters are needed
            if (parameters.Length == 0) { mInfo.Invoke(cmdInstance, null); break; }

            // Check if correct number of parameters were entered
            if (properties.Length-1 != parameters.Length) { break; }

            // Attempt to convert input to expected types
            object[] methodParams = new object[parameters.Length];
            bool isValid = true;
            for (int i=1; i< properties.Length; i++)
            {
                // Get the type of the method parameter
                System.Type paramType = parameters[i - 1].ParameterType;

                // If parameter is an int
                if(paramType == typeof(int))
                {
                    int output;
                    bool isInt = int.TryParse(properties[i], out output);

                    if (isInt) { methodParams[i - 1] = output; }
                    else { isValid = false; break; }
                }

                // If parameter is a float
                else if (paramType == typeof(float))
                {
                    float output;
                    bool isFloat = float.TryParse(properties[i], out output);

                    if (isFloat) { methodParams[i - 1] = output; }
                    else { isValid = false; break; }
                }

                // Parameter is a string
                else { methodParams[i - 1] = properties[i]; }
            }

            if (isValid) { mInfo.Invoke(cmdInstance, methodParams); }
                break;
        }
    }

    void InitConsoleCommands()
    {
        cmdInstance = new DebugCommands();
        System.Type debugType = cmdInstance.GetType();
        foreach (MethodInfo mInfo in debugType.GetMethods())
        {
            foreach (System.Attribute attr in System.Attribute.GetCustomAttributes(mInfo))
            {
                if (attr.GetType() == typeof(ConsoleCommandAttribute))
                {
                    string key = mInfo.Name.ToUpper();
                    (string, MethodInfo) value = (((ConsoleCommandAttribute)attr).Description, mInfo);
                    consoleMethods[key] = value;
                }
            }
        }
    }
    #endregion
}

/// <summary> Attribute for Console Commands </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class ConsoleCommandAttribute : System.Attribute
{
    #region Variables
    string _description;
    #endregion

    #region Getters
    public string Description { get { return _description; } }
    #endregion

    /// <summary> Constructor </summary>
    /// <param name="description"> Describes what this command does </param>
    public ConsoleCommandAttribute(string description)
    {
        _description = description;
    }
}
#endregion

#region Credits
/// Script idea from https://www.youtube.com/channel/UCR35rzd4LLomtQout93gi0w
/// Script created by Tyler Nichols
#endregion