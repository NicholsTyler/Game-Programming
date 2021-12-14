#region Namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion


/// <summary> Base Class for implementing the Command Design Pattern </summary>
public abstract class Command
{
    /// <summary> Executes this Command </summary>
    public abstract void Execute();

    /// <summary> Undoes this Command </summary>
    public abstract void Undo();
}
