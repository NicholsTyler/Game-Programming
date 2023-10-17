#region Namespaces
using UnityEngine;
#endregion

namespace Utility.Tools
{
    #region Entities
        /// <summary> This GameObject can become invincible </summary>
        public interface IInvincable
        {
            bool Invincible { get; set; }
        }

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

        /// <summary> This GameObject can be thrown </summary>
        public interface IThrowable
        {
            void Throw();
        }

        /// <summary> This GameObject can be equipped </summary>
        public interface IEquipable
        {
            void Equip();
            void Unequip();
        }

        /// <summary> This GameObject can be interacted with </summary>
        public interface IInteractable
        {
            void Interact();
        }

        /// <summary> This GameObject can be saved </summary>
        public interface ISaveable
        {
            void Save();
        }
    #endregion
}
#region Credits
    /// Script created by Tyler Nichols
#endregion