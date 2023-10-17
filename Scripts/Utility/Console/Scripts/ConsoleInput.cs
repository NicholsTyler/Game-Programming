#region Namespaces
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Utility.Tools;
#endregion

namespace Utility.Console
{
    /// <summary> Handles all user input </summary>
    public class ConsoleInput : Singleton<ConsoleInput>
    {
        #region Attributes
            /* InputActions */
            InputActionMap _consoleMap;
            InputAction _consoleToggle;
            InputAction _consoleReturn;
            InputAction _consoleComplete;
            InputAction _consoleIterate;

            /* References */
            ConsoleManager _console;
        #endregion

        #region Binding
            /* Console */
            const string ConsoleToggleKey = "<Keyboard>/backquote";
            const string ConsoleToggleBtn = "";

            const string ConsoleReturnKey = "<Keyboard>/enter";
            const string ConsoleReturnBtn = "";

            const string ConsoleAutoKey = "<Keyboard>/tab";
            const string ConsoleAutoBtn = "";

            const string ConsoleIteratePosKey = "<Keyboard>/upArrow";
            const string ConsoleIteratePosBtn = "";

            const string ConsoleIterateNegKey = "<Keyboard>/downArrow";
            const string ConsoleIterateNegBtn = "";
        #endregion

        #region Methods
            /* Unity */
            override protected void Awake()
            {
                base.Awake();
                CreateConsoleActions();
            }
            void OnEnable()
            {
                if (_console is null)
                {
                    _console = ConsoleManager.Instance;
                }
                _consoleMap.Enable();
            }
            void OnDisable()
            {
                _consoleMap.Disable();
            }

            /* Private */
            void CreateConsoleActions()
            {
                // Create Action Map
                _consoleMap = new InputActionMap("ConsoleMap");

                // Toggle
                _consoleToggle = _consoleMap.AddAction("ConsoleToggle", binding: ConsoleToggleKey);
                _consoleToggle.performed += ctx => _console.ToggleConsole();

                // Return
                _consoleReturn = _consoleMap.AddAction("ConsoleReturn", binding: ConsoleReturnKey);
                _consoleReturn.performed += ctx => _console.ProcessCommand();

                // Auto Complete
                _consoleComplete = _consoleMap.AddAction("ConsoleAuto", binding: ConsoleAutoKey);
                _consoleComplete.performed += ctx => _console.AutoComplete();

                // History Iteration
                _consoleIterate = _consoleMap.AddAction("ConsoleIterate");
                _consoleIterate.AddCompositeBinding("Axis(minValue=0,maxValue=1)")
                    .With("Positive", ConsoleIteratePosKey)
                    .With("Negative", ConsoleIterateNegKey);
                _consoleIterate.performed += ctx => _console.IterateHistory(ctx.ReadValue<float>());
            }
        #endregion
    }
}

#region Credits
    /// Script created by Tyler Nichols
#endregion