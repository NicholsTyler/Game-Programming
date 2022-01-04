namespace Utility.Tools
{
    #region Entities

    /// <summary> This GameObject can take damage </summary>
    public interface IDamageable<T>
    {
        T Health { get; set; }
        void Damage(T amount);
    }

    /// <summary> This GameObject can be healed </summary>
    public interface IHealable<T>
    {
        T Health { get; set; }
        void Heal(T amount);
    }

    /// <summary> This GameObject can be killed </summary>
    public interface IKillable
    {
        void Kill();
    }

    /// <summary> This GameObject can enter God Mode </summary>
    public interface IGodable
    {
        void God(bool active);
    }

    /// <summary> This GameObject can move </summary>
    public interface IMoveable<T>
    {
        T MoveSpeed { get; set; }
        void Move();
    }

    /// <summary> This GameObject can shoot </summary>
    public interface IShootable
    {
        void Fire();
    }

    #endregion

    #region Objects

    /// <summary> This GameObject can be picked up </summary>
    public interface IPickupable
    {
        void Pickup();
    }

    /// <summary> This GameObject can be consumed </summary>
    public interface IConsumable
    {
        void Consume();
    }

    /// <summary> This GameObject can be saved </summary>
    public interface IPersistable
    {
        void Save();
    }

    /// <summary> This GameObject can be driven </summary>
    public interface IDrivable
    {
        void Drive();
    }

    #endregion
}
#region Credits
/// Script created by Tyler Nichols
#endregion