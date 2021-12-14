#region Namespaces
using UnityEngine;
#endregion

public class CommandMoveForward : Command
{
    // The Object this Command will move
    BasicMove moveObject;

    // Constructor
    public CommandMoveForward(BasicMove moveObject) { this.moveObject = moveObject; }

    // Methods
    public override void Execute() { moveObject.Move(Vector3.forward); }
    public override void Undo() { moveObject.Move(Vector3.back); }
}
