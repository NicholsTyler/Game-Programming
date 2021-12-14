#region Namespaces
using UnityEngine;
#endregion

public class CommandMoveForward : Command
{
    // The Object this Command will move
    BasicMove _moveObject;

    // Constructor
    public CommandMoveForward(BasicMove moveObject) { _moveObject = moveObject; }

    // Methods
    public override void Execute() { _moveObject.Move(Vector3.forward); }
    public override void Undo() { _moveObject.Move(Vector3.back); }
}
