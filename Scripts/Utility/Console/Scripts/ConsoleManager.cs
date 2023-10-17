#region Namespaces
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using Utility.Tools;
#endregion

namespace Utility.Console
{
    /// <summary> Draws Debug Console & Handles Commands </summary>
    public class ConsoleManager : Singleton<ConsoleManager>
    {
        #region Attributes
            // Stores created commands
            Dictionary<string, (string, MethodInfo)> _consoleMethods;

            // States
            bool _showConsole;
            bool _showHelp;

            // User Input
            string _input;
            string _exactInput;
            Vector2 _helpScroll;
            Vector2 _scroll;

            // Other
            const string ConsoleCommandsRef = "Utility.Console.ConsoleCommands";
        #endregion
        #region GUI Attributes
            // Console
            float _verticalSpacing = 30;

            // GUI Style
            GUIStyle _style = new GUIStyle();
            int _fontSize = 16;
            Color _fontColor = Color.white;

            // Console GUI Styles
            GUIStyle _helpStyle = new GUIStyle();
            GUIStyle _mainStyle = new GUIStyle();
            GUIStyle _inputStyle = new GUIStyle();

            // History
            float _historyLength = 150;
            List<string> _historyInput;

            // History Iteration
            Stack<string> _oldStack;
            Stack<string> _newStack;
            string _curInput = null;

            // Other
            float _lineHeight = 30;
            float _inputFieldOffset = 10;
            float _padding = 5;
            string Separator = new string('-', (int)(Screen.width * 0.05f));

            // GUI Help Cache
            string[] _commandsHelp;
            Rect _helpBox, _helpViewPort, _helpScrollView, _helpLabelOne, _helpLabelTwo;
            Rect _histBox, _histViewPort, _histScrollView;
            Rect _consBox, _consTextBox;
        #endregion

        #region Getters
            public bool Active { get => _showConsole; }
        #endregion

        #region Unity Methods
            override protected void Awake()
            {
                base.Awake();

                // Attach ConsoleCommands component to this gameobject
                Type consoleCommands = Type.GetType(ConsoleCommandsRef);
                if (consoleCommands is null) { Debug.LogError("Console failed to initialize. Check namespace of ConsoleCommands.cs"); }
                else { gameObject.AddComponent(consoleCommands); }
            }
            void Start()
            {
                // Initialize Data Structures
                _consoleMethods = new Dictionary<string, (string, MethodInfo)>();
                _historyInput = new List<string>();
                _oldStack = new Stack<string>();
                _newStack = new Stack<string>();

                InitConsoleCommands();
                CacheGUI();
                CacheHelpUI();
                SetGUIStyle();

                enabled = false;
            }
            void OnGUI()
            {
                // Only draw the GUI if console is enabled
                if (!_showConsole) return;

                // Help GUI
                if (_showHelp)
                {
                    GUI.Box(_helpBox, "", _helpStyle);

                    _helpScroll = GUI.BeginScrollView(_helpScrollView, _helpScroll, _helpViewPort);

                    Rect labelRect = _helpLabelOne;
                    GUI.Label(labelRect, "Debug Console Commands", _style);
                    labelRect = _helpLabelTwo;
                    GUI.Label(labelRect, Separator, _style);

                    int j = 2;
                    foreach(var label in _commandsHelp)
                    {
                        labelRect = new Rect(_padding, _lineHeight * j, _helpViewPort.width - _historyLength, _lineHeight);
                        GUI.Label(labelRect, label, _style);
                        j++;
                    }
                    GUI.EndScrollView();
                    _histBox.y = _historyLength;
                    _histScrollView.y = _historyLength + (_padding * 2);
                    _consBox.y = _historyLength * 2;
                    _consTextBox.y = _historyLength * 2 + _padding;
                }
                else
                {
                    _histBox.y = 0;
                    _histScrollView.y = _padding * 2;
                    _consBox.y = _historyLength;
                    _consTextBox.y = _historyLength + _padding;
                }

                // Chat History
                GUI.Box(_histBox, "", _mainStyle);

                _histViewPort.height = _lineHeight * _historyInput.Count;
                _scroll = GUI.BeginScrollView(_histScrollView, _scroll, _histViewPort);

                int i = 0;
                foreach (var cmd in _historyInput)
                {
                    Rect labelRect = new Rect(_padding, _lineHeight * i, _histViewPort.width - _historyLength, _lineHeight);
                    GUI.Label(labelRect, cmd, _style);
                    i++;
                }

                GUI.EndScrollView();

                // Draws console box at the top of the screen
                GUI.Box(_consBox, "", _inputStyle);

                // Allows user to type in console
                GUI.SetNextControlName("Input");
                _input = GUI.TextField(_consTextBox, _input, _style);
                GUI.FocusControl("Input");
            }
        #endregion
        #region Public Methods
            /// <summary> Displays help in the console </summary>
            public void DisplayHelp() => _showHelp = !_showHelp;

            /// <summary> Processes the current user input </summary>
            public void ProcessCommand()
            {
                if (!_showConsole) { return; }
                HandleInput();
                _input = "";
            }

            /// <summary> Attempts to finish the user input </summary>
            public void AutoComplete()
            {
                if (!_showConsole) { return; }
                if (_input.Length == 0) { _input = "help"; return; }

                foreach (var key in _consoleMethods.Keys)
                {
                    _input = _input.ToLower();
                    if (key.Contains(_input)) { _input = key; break; }
                }
            }

            /// <summary> Iterates through console history </summary>
            public void IterateHistory(float val)
            {
                if (!_showConsole) { return; }

                // Up Arrow Pressed
                if (val > 0 && _oldStack.Count > 0)
                {
                    if (_curInput != null) { _newStack.Push(_curInput); }
                    var cmd = _oldStack.Pop();
                    _curInput = cmd;
                    _input = cmd;
                }

                // Down Arrow Pressed
                else if (val < 0)
                {
                    // Down Arrow Pressed but already at newest command
                    if (_newStack.Count == 0)
                    {
                        if (_curInput != null && (_oldStack.Count == 0 || _oldStack.Peek() != _curInput)) { _oldStack.Push(_curInput); }
                        _curInput = null;
                        _input = "";
                        return;
                    }
                    if (_curInput != null) { _oldStack.Push(_curInput); }
                    var cmd = _newStack.Pop();
                    _curInput = cmd;
                    _input = cmd;
                }
            }

            /// <summary> Enables / Disables the console </summary>
            /// <returns> Is the console active after execution? </returns>
            public void ToggleConsole()
            {
                _showConsole = !_showConsole;

                if (_showConsole) Cursor.lockState = CursorLockMode.Confined;
                else Cursor.lockState = CursorLockMode.Locked;

                _input = "";
                enabled = _showConsole;
            }

            /// <summary> Clears the console history </summary>
            public void ClearConsole()
            {
                _historyInput.Clear();
                _oldStack.Clear();
                _newStack.Clear();
                _showHelp = false;
            }

            /// <summary> Adds a string to the console history </summary>
            public void AddToHistory(string input)
        {
            _historyInput.Add(input);
            _scroll = new Vector2(0, _historyInput.Count * _lineHeight);
        }
        #endregion
        #region Private Methods
            void AddToCommandHistory(string input)
            {
                if (_curInput != null) { _oldStack.Push(_curInput); }
                while (_newStack.Count > 0) { _oldStack.Push(_newStack.Pop()); }
                _oldStack.Push(input);
                _curInput = null;
            }
            void HandleInput()
            {
                // Verify input was not null or whitespace
                if (string.IsNullOrWhiteSpace(_input)) { return; }

                // Make the input all lower case
                _exactInput = _input;
                _input = _input.ToLower();

                // Add the input to the command history
                AddToCommandHistory(_exactInput);

                // Separate id from parameters
                string[] properties = _input.Split(' ');
                string[] exactProperties = _exactInput.Split(' ');

                // Determine if Dictionary contains the id
                string keyInput = properties[0];
                if (!_consoleMethods.ContainsKey(keyInput)) { AddToHistory(exactProperties[0] + " is not a valid command."); return; }

                MethodInfo mInfo = _consoleMethods[keyInput].Item2;
                ParameterInfo[] parameters = mInfo.GetParameters();

                // Invoke method if no parameters are needed
                if (parameters.Length == 0) { mInfo.Invoke(ConsoleCommands.Instance, null); AddToHistory(_exactInput); return; }

                // Checks for number of default values
                int defVals = 0;
                foreach (var param in parameters) { if (param.IsOptional) defVals++; }

                // Check if correct number of parameters were entered
                if (properties.Length - 1 < parameters.Length-defVals)
                {
                    AddToHistory("You entered: " + _exactInput);
                    AddToHistory(keyInput + " requires " + (parameters.Length-defVals) + " parameters. You entered " + (properties.Length - 1));
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
                            AddToHistory("You entered: " + _exactInput);
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
                            AddToHistory("You entered: " + _exactInput);
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
                            AddToHistory("You entered: " + _exactInput);
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
                            AddToHistory("You entered: " + _exactInput);
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
                            AddToHistory("You entered: " + _exactInput);
                            AddToHistory("Parameter #" + i + " <" + exactProperties[i] + "> is not a valid color.");
                            break;
                        }
                    }

                    // Parameter is a string
                    else { methodParams[i - 1] = properties[i]; }
                }

                if (!isValid) return;

                int idx = methodParams.Length;
                while (defVals > 0)
                {
                    idx -= defVals;
                    if (methodParams[idx] is null) { methodParams[idx] = parameters[idx].DefaultValue; }
                    defVals--;
                }

                AddToHistory(_exactInput); 
                mInfo.Invoke(ConsoleCommands.Instance, methodParams);
            }
            void InitConsoleCommands()
            {
                Type debugType = ConsoleCommands.Instance.GetType();
                foreach (MethodInfo mInfo in debugType.GetMethods())
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                    {
                        if (attr.GetType() == typeof(CommandAttribute))
                        {
                            string key = mInfo.Name.ToLower();
                            (string, MethodInfo) value = (((CommandAttribute)attr).Description, mInfo);
                            _consoleMethods[key] = value;
                        }
                    }
                }

                AddToHistory("Welcome to the Debug Console!");
                AddToHistory("Type \'help\' to view all available commands.");
            }
            void CacheGUI()
            {
                float y = 0f;

                // Help GUI
                _helpBox = new Rect(0, y, Screen.width, _historyLength);
                _helpViewPort = new Rect(0, 0, Screen.width - _verticalSpacing, _lineHeight * (_consoleMethods.Count + 2));
                _helpScrollView = new Rect(0, y + (_padding * 2), Screen.width, _historyLength - _inputFieldOffset);
                _helpLabelOne = new Rect(_padding, 0, _helpViewPort.width - _historyLength, _lineHeight);
                _helpLabelTwo = new Rect(_padding, _lineHeight, _helpViewPort.width - _historyLength, _lineHeight);

                // History GUI
                _histBox = new Rect(0, y, Screen.width, _historyLength);
                _histViewPort = new Rect(0, 0, Screen.width - _verticalSpacing, _lineHeight * _historyInput.Count);
                _histScrollView = new Rect(0, y + _padding, Screen.width, _historyLength - _inputFieldOffset);

                y += _historyLength;

                // General Console GUI
                _consBox = new Rect(0, y, Screen.width, _verticalSpacing);
                _consTextBox = new Rect(_inputFieldOffset, y + _padding, Screen.width - _lineHeight, _lineHeight);
            }
            void CacheHelpUI()
            {
                _commandsHelp = new string[_consoleMethods.Count];

                int i = 0;
                foreach (var pair in _consoleMethods)
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
                    _commandsHelp[i] = label;
                    i++;
                }
            }
            void SetGUIStyle()
        {
            // Set GUI Style
            _style.fontSize = _fontSize;
            _style.normal.textColor = _fontColor;

            // Set Help GUI Style
            Texture2D helpBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            helpBackground.SetPixel(0, 0, new Color32(100, 100, 100, 255));
            helpBackground.Apply();
            _helpStyle.normal.background = helpBackground;

            // Set Main GUI Style
            Texture2D mainBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            mainBackground.SetPixel(0, 0, new Color32(75, 75, 75, 255));
            mainBackground.Apply();
            _mainStyle.normal.background = mainBackground;

            // Set Input GUI Style
            Texture2D inputBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            inputBackground.SetPixel(0, 0, new Color32(40, 40, 40, 255));
            inputBackground.Apply();
            _inputStyle.normal.background = inputBackground;
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