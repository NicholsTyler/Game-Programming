#region Namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class GameController : MonoBehaviour
{
    [Tooltip("The movable object")]
    public BasicMove moveObject;

    #region Private Attributes
    // Commands
    Command buttonW;
    Command buttonA;
    Command buttonS;
    Command buttonD;

    // Undo / Redo Stack Data Structures
    Stack<Command> undoCommands = new Stack<Command>();
    Stack<Command> redoCommands = new Stack<Command>();

    // True if we are currently replaying
    bool isReplaying = false;

    // The object's starting position
    Vector3 startPos;

    //The time between each command execution when we replay so we can see what's going on
    const float REPLAY_PAUSE_TIMER = 0.5f;
    #endregion

    #region Unity Methods
    void Start()
    {
        //Bind the keys to default commands
        buttonW = new CommandMoveForward(moveObject);
        //buttonA = new CommandMoveLeft(moveObject);
        //buttonS = new CommandMoveBack(moveObject);
        //buttonD = new CommandMoveRight(moveObject);

        startPos = moveObject.transform.position;
    }

    void Update()
    {
        //We can check for input while we are replaying
        if (isReplaying) { return; }

        CheckInput();
    }
    #endregion

    #region Custom Methods
    /// <summary> Checks input for W, A, S, D, U, R, Spacebar, Return </summary>
    void CheckInput()
    {
        // If using Time.deltaTime to move, it will need to be saved in the stack
        // Could potentially create a stack of a custom class
        if (Input.GetKeyDown(KeyCode.W)) { ExecuteNewCommand(buttonW); }
        if (Input.GetKeyDown(KeyCode.A)) { ExecuteNewCommand(buttonA); }
        if (Input.GetKeyDown(KeyCode.S)) { ExecuteNewCommand(buttonS); }
        if (Input.GetKeyDown(KeyCode.D)) { ExecuteNewCommand(buttonD); }

        //Undo with u
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (undoCommands.Count == 0) { Debug.Log("Nothing to undo."); }
            else
            {
                Command lastCommand = undoCommands.Pop();

                lastCommand.Undo();

                //Add this to redo if we want to redo the undo
                redoCommands.Push(lastCommand);
            }
        }

        //Redo with r
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (redoCommands.Count == 0) { Debug.Log("Nothing to redo."); }
            else
            {
                Command nextCommand = redoCommands.Pop();

                nextCommand.Execute();

                //Add to undo if we want to undo the redo
                undoCommands.Push(nextCommand);
            }
        }

        // To test Input Swapping, press Spacebar to swap A and D
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Ref is important or the keys will not be swapped
            SwapKeys(ref buttonA, ref buttonD);
        }

        //Start replay by pressing Return
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Replay());

            isReplaying = true;
        }
    }

    // Replay
    IEnumerator Replay()
    {
        // Move object to intial position
        moveObject.transform.position = startPos;

        // Pause so we can see that it has started at the start position
        yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);

        //Convert the undo stack to an array
        Command[] oldCommands = undoCommands.ToArray();

        //This array is inverted so we iterate from the back
        for (int i = oldCommands.Length - 1; i >= 0; i--)
        {
            Command currentCommand = oldCommands[i];

            currentCommand.Execute();

            yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);
        }

        isReplaying = false;
    }

    // Executes the command and do stuff to the list to make the replay, undo, redo system work
    void ExecuteNewCommand(Command commandButton)
    {
        commandButton.Execute();

        // Add the new command to the last position in the list
        undoCommands.Push(commandButton);

        // Remove all redo commands because redo is not defined when we have add a new command
        redoCommands.Clear();
    }

    // Swap the pointers to two commands
    void SwapKeys(ref Command key1, ref Command key2)
    {
        Command temp = key1;

        key1 = key2;

        key2 = temp;
    }
    #endregion
}
