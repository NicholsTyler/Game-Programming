#region Namespaces
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
using System.Collections.Generic;
using Utility.Tools;
#endregion
namespace Utility.Systems
{
    /// <summary> Draws Debug Console & Handles Commands </summary>
    public class Console : MonoBehaviour
    {
        #region Attributes
        // Actions
        InputAction toggleAction;
        InputAction returnAction;
        InputAction completeAction;
        InputAction iterateAction;

        // Singleton
        public static Console instance;
        public static Component cmdInstance;

        // Stores created commands
        Dictionary<string, (string, MethodInfo)> consoleMethods;

        // States
        bool showConsole;
        bool showHelp;

        // User Input
        string input;
        string exactInput;
        Vector2 helpScroll;
        Vector2 scroll;
        #endregion

        #region GUI Attributes
        // Console
        float verticalSpacing = 30;

        // History
        float historyLength = 150;
        List<string> historyInput;

        // History Iteration
        Stack<string> oldStack;
        Stack<string> newStack;
        string curInput = null;

        // Other
        float lineHeight = 25;
        float inputFieldOffset = 10;
        float padding = 5;

        // GUI Help Cache
        Rect helpBox, helpViewPort, helpScrollView, helpLabelOne, helpLabelTwo;
        Rect histBox, histViewPort, histScrollView;
        Rect consBox, consTextBox;
        Color consBG;
        #endregion

        #region Unity Methods
        void Awake()
        {
            // Create singleton
            if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
            else { Debug.LogError("Multiple instances of DebugController"); }

            // Create Actions
            InitActions();

            // Initialize Data Structures
            consoleMethods = new Dictionary<string, (string, MethodInfo)>();
            historyInput = new List<string>();
            oldStack = new Stack<string>();
            newStack = new Stack<string>();

            // Attach ConsoleCommands component to this gameobject
            Type consoleCommands = Type.GetType("Utility.Systems.ConsoleCommands");
            if (consoleCommands is null) { Debug.LogError("Console failed to initialize. Generate the ConsoleCommands script using the inspector"); }
            else { gameObject.AddComponent(consoleCommands); }
        }
        void Start()
        {
            InitConsoleCommands();
            CacheGUI();
        }
        void OnGUI()
        {
            // Only draw the GUI if console is enabled
            if (!showConsole) return;

            // Help GUI
            if (showHelp)
            {
                GUI.Box(helpBox, "");

                helpScroll = GUI.BeginScrollView(helpScrollView, helpScroll, helpViewPort);

                Rect labelRect = helpLabelOne;
                GUI.Label(labelRect, "Debug Console Commands");
                labelRect = helpLabelTwo;
                GUI.Label(labelRect, new string('-', (int)(Screen.width * 0.05f)));

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

                    labelRect = new Rect(padding, lineHeight * j, helpViewPort.width - historyLength, lineHeight);
                    GUI.Label(labelRect, label);
                    j++;
                }

                GUI.EndScrollView();
                histBox.y = historyLength;
                histScrollView.y = historyLength;
                consBox.y = historyLength * 2;
                consTextBox.y = historyLength * 2;
            }
            else
            {
                histBox.y = 0;
                histScrollView.y = 0;
                consBox.y = historyLength;
                consTextBox.y = historyLength;
            }

            // Chat History
            GUI.Box(histBox, "");

            histViewPort.height = lineHeight * historyInput.Count;
            scroll = GUI.BeginScrollView(histScrollView, scroll, histViewPort);

            int i = 0;
            foreach (var cmd in historyInput)
            {
                Rect labelRect = new Rect(padding, lineHeight * i, histViewPort.width - historyLength, lineHeight);
                GUI.Label(labelRect, cmd);
                i++;
            }

            GUI.EndScrollView();

            // Draws console box at the top of the screen
            GUI.Box(consBox, "");
            GUI.backgroundColor = consBG;

            // Allows user to type in console
            GUI.SetNextControlName("Input");
            input = GUI.TextField(consTextBox, input);
            GUI.FocusControl("Input");
        }
        void OnEnable()
        {
            toggleAction.Enable();
            returnAction.Enable();
            completeAction.Enable();
            iterateAction.Enable();
        }
        void OnDisable()
        {
            toggleAction.Disable();
            returnAction.Disable();
            completeAction.Disable();
            iterateAction.Disable();
        }
        #endregion

        #region Custom Methods
        /* Input Methods */
        void OnToggleDebug(InputAction.CallbackContext context)
        {
            showConsole = !showConsole;
            input = "";
        }
        void OnReturn(InputAction.CallbackContext context)
        {
            if (!showConsole) { return; }
            HandleInput();
            input = "";
        }
        void OnAutoComplete(InputAction.CallbackContext context)
        {
            if (!showConsole) { return; }
            if(input.Length == 0) { input = "help"; return; }

            foreach (var key in consoleMethods.Keys)
            {
                input = input.ToLower();
                if (key.Contains(input)) { input = key; break; }
            }
        }
        void OnIterateHistory(InputAction.CallbackContext context)
        {
            if (!showConsole) { return; }
            float val = context.ReadValue<float>();

            // Up Arrow Pressed
            if (val > 0 && oldStack.Count > 0)
            {
                if (curInput != null) { newStack.Push(curInput); }
                var cmd = oldStack.Pop();
                curInput = cmd;
                input = cmd;
            }

            // Down Arrow Pressed
            else if (val < 0)
            {
                // Down Arrow Pressed but already at newest command
                if (newStack.Count == 0)
                {
                    if (curInput != null && (oldStack.Count == 0 || oldStack.Peek() != curInput)) { oldStack.Push(curInput); }
                    curInput = null;
                    input = ""; 
                    return; 
                }
                if (curInput != null) { oldStack.Push(curInput); }
                var cmd = newStack.Pop(); 
                curInput = cmd;
                input = cmd;
            }
        }

        /* Public Methods */
        public void DisplayHelp() => showHelp = !showHelp;
        public void ClearConsole()
        {
            historyInput.Clear();
            oldStack.Clear();
            newStack.Clear();
            showHelp = false;
        }
        public void AddToHistory(string input)
        {
            historyInput.Add(input);
            scroll = new Vector2(0, historyInput.Count * lineHeight);
        }

        /* Private Methods */
        void AddToCommandHistory(string input)
        {
            if (curInput != null) { oldStack.Push(curInput); }
            foreach (var cmd in newStack) oldStack.Push(cmd);
            if (newStack.Count != 0) { newStack.Clear(); }
            oldStack.Push(input);
            curInput = null;
        }
        void HandleInput()
        {
            // Verify input was not null or whitespace
            if (string.IsNullOrWhiteSpace(input)) { return; }

            // Make the input all lower case
            exactInput = input;
            input = input.ToLower();

            // Add the input to the command history
            AddToCommandHistory(exactInput);

            // Separate id from parameters
            string[] properties = input.Split(' ');
            string[] exactProperties = exactInput.Split(' ');

            // Determine if Dictionary contains the id
            string keyInput = properties[0];
            if (!consoleMethods.ContainsKey(keyInput)) { AddToHistory(exactProperties[0] + " is not a valid command."); return; }

            MethodInfo mInfo = consoleMethods[keyInput].Item2;
            ParameterInfo[] parameters = mInfo.GetParameters();

            // Invoke method if no parameters are needed
            if (parameters.Length == 0) { mInfo.Invoke(cmdInstance, null); AddToHistory(exactInput); return; }

            // Check if correct number of parameters were entered
            if (properties.Length - 1 != parameters.Length)
            {
                AddToHistory("You entered: " + exactInput);
                AddToHistory(keyInput + " requires " + parameters.Length + " parameters. You entered " + (properties.Length - 1));
                return;
            }

            // Attempt to convert input to expected types
            object[] methodParams = new object[parameters.Length];
            bool isValid = true;
            for (int i = 1; i < properties.Length; i++)
            {
                // Get the type of the method parameter
                Type paramType = parameters[i - 1].ParameterType;

                // If parameter is an int
                if (paramType == typeof(int))
                {
                    bool isInt = int.TryParse(properties[i], out int output);

                    if (isInt) { methodParams[i - 1] = output; }
                    else
                    {
                        isValid = false;
                        AddToHistory("You entered: " + exactInput);
                        AddToHistory("Parameter #" + i + " (" + exactProperties[i] + ") is not a valid integer.");
                        break;
                    }
                }

                // If parameter is a float
                else if (paramType == typeof(float))
                {
                    bool isFloat = float.TryParse(properties[i], out float output);

                    if (isFloat) { methodParams[i - 1] = output; }
                    else
                    {
                        isValid = false;
                        AddToHistory("You entered: " + exactInput);
                        AddToHistory("Parameter #" + i + " <" + exactProperties[i] + "> is not a valid integer or decimal");
                        break;
                    }
                }

                // If parameter is a boolean
                else if (paramType == typeof(bool))
                {
                    bool isBool = bool.TryParse(properties[i], out bool output);

                    if (isBool) { methodParams[i - 1] = output; }
                    else
                    {
                        isValid = false;
                        AddToHistory("You entered: " + exactInput);
                        AddToHistory("Parameter #" + i + " <" + exactProperties[i] + "> should be true or false.");
                        break;
                    }
                }

                // If parameter is a GameObject
                else if (paramType == typeof(GameObject))
                {
                    GameObject foundGO = GameObject.Find(exactProperties[i]);
                    if (foundGO != null) { methodParams[i - 1] = foundGO; }
                    else
                    {
                        isValid = false;
                        AddToHistory("You entered: " + exactInput);
                        AddToHistory("The GameObject <" + exactProperties[i] + "> was not found. Check your spelling and capitalization.");
                        break;
                    }
                }

                // If parameter is a color
                else if (paramType == typeof(Color))
                {
                    Color newColor = properties[i].ToColor();

                    if (newColor != Color.clear) { methodParams[i - 1] = newColor; }
                    else
                    {
                        isValid = false;
                        AddToHistory("You entered: " + exactInput);
                        AddToHistory("Parameter #" + i + " <" + exactProperties[i] + "> is not a valid color.");
                        break;
                    }
                }

                // Parameter is a string
                else { methodParams[i - 1] = properties[i]; }
            }

            if (isValid) { AddToHistory(exactInput); mInfo.Invoke(cmdInstance, methodParams); }
        }
        void InitConsoleCommands()
        {
            TryGetComponent(Type.GetType("Utility.Systems.ConsoleCommands"), out cmdInstance);
            Type debugType = cmdInstance.GetType();
            foreach (MethodInfo mInfo in debugType.GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(CommandAttribute))
                    {
                        string key = mInfo.Name.ToLower();
                        (string, MethodInfo) value = (((CommandAttribute)attr).Description, mInfo);
                        consoleMethods[key] = value;
                    }
                }
            }

            AddToHistory("Welcome to the Debug Console!");
            AddToHistory("Type \'help\' to view all available commands.");
        }
        void InitActions()
        {
            // Create Console Toggle Action
            toggleAction = new InputAction("ToggleDebug", binding: "<Keyboard>/backquote");
            toggleAction.performed += OnToggleDebug;

            // Create Return Action
            returnAction = new InputAction("Return", binding: "<Keyboard>/enter");
            returnAction.performed += OnReturn;

            // Create Autocomplete Action
            completeAction = new InputAction("AutoComplete", binding: "<Keyboard>/tab");
            completeAction.performed += OnAutoComplete;

            // Create History Iteration Action
            iterateAction = new InputAction("IterateHistory");
            iterateAction.AddCompositeBinding("Axis(minValue=0,maxValue=1)")
                .With("Positive", "<Keyboard>/upArrow")
                .With("Negative", "<Keyboard>/downArrow");
            iterateAction.performed += OnIterateHistory;
        }
        void CacheGUI()
        {
            float y = 0f;

            // Help GUI
            helpBox = new Rect(0, y, Screen.width, historyLength);
            helpViewPort = new Rect(0, 0, Screen.width - verticalSpacing, lineHeight * (consoleMethods.Count + 2));
            helpScrollView = new Rect(0, y + padding, Screen.width, historyLength - inputFieldOffset);
            helpLabelOne = new Rect(padding, 0, helpViewPort.width - historyLength, lineHeight);
            helpLabelTwo = new Rect(padding, lineHeight, helpViewPort.width - historyLength, lineHeight);

            // History GUI
            histBox = new Rect(0, y, Screen.width, historyLength);
            histViewPort = new Rect(0, 0, Screen.width - verticalSpacing, lineHeight * historyInput.Count);
            histScrollView = new Rect(0, y + padding, Screen.width, historyLength - inputFieldOffset);

            y += historyLength;

            // General Console GUI
            consBox = new Rect(0, y, Screen.width, verticalSpacing);
            consTextBox = new Rect(inputFieldOffset, y + padding, Screen.width - lineHeight, lineHeight);
            consBG = new Color(0, 0, 0, 0.25f);
        }
        #endregion
    }

    /// <summary> Converts a method into a console command </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        string _description;
        public string Description { get { return _description; } }

        /// <param name="description"> What will this method do when called? </param>
        public CommandAttribute(string description) => _description = description;
    }
}

#region Credits
/// NOTE: THIS SCRIPT USES UNITY'S NEW INPUT SYSTEM

/// Script created by Tyler Nichols
/// Script idea from Game Dev Guide (Youtube)
#endregion