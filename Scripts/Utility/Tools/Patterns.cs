namespace Utility.Tools
{
    /// <summary> Base Class for implementing the Command Design Pattern </summary>
    public abstract class Command
    {
        /// <summary> Executes this Command </summary>
        public abstract void Execute();

        /// <summary> Undoes this Command </summary>
        public abstract void Undo();
    }
}
#region Credits
/// Script created by Tyler Nichols
#endregion