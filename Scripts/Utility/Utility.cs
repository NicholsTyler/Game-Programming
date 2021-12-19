#region Namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion
namespace Utility
{

    /// <summary> Useful Global Methods </summary>
    public static class Utility
    {

    }

    /// <summary> Useful Extension Methods </summary>
    public static class Extensions
    {
        #region Type

        /// <returns> A random element </returns>
        public static T Rand<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

        #endregion

        #region GameObject

        /// <summary> Sets the layer of this GameObject and ALL children </summary>
        /// <param name="layer"> The desired layer </param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            if (gameObject.layer.ToString() == "") { Debug.LogWarning(gameObject.name + " was set to an unnamed layer"); }
            foreach (Transform t in gameObject.transform) { t.gameObject.SetLayersRecursively(layer); }
        }

        /// <returns> This GameObject's Position </returns>
        public static Vector3 Pos(this GameObject gameObject) => gameObject.transform.position;

        /// <returns> This GameObject's Rotation </returns>
        public static Quaternion Rot(this GameObject gameObject) => gameObject.transform.rotation;

        /// <returns> This GameObject's Scale </returns>
        public static Vector3 Scale(this GameObject gameObject) => gameObject.transform.localScale;

        #endregion

        #region Transform

        /// <summary> Destroys all child objects of this Transform </summary>
        public static void DestroyChildren(this Transform t)
        {
            foreach(Transform child in t) { Object.Destroy(child.gameObject); }
        }

        #endregion

        #region Vector3

        /// <returns> This Vector3 as a Vector2 </returns>
        public static Vector2 ToV2(this Vector3 vector) => new Vector2(vector.x, vector.y);

        /// <returns> This Vector3 as a Vector3Int </returns>
        public static Vector3Int ToVector3Int(this Vector3 vector) => new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);

        /// <returns> This Vector3 with y = 0 </returns>
        public static Vector3 Flat(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

        #endregion

        #region SpriteRenderer

        /// <summary> Adjust the transparency of this SpriteRenderer </summary>
        /// <param name="a"> The alpha (Between 0-255) </param>
        public static void Fade(this SpriteRenderer rend, float a)
        {
            if (a > 255 || a < 0) { Debug.LogError("Alpha must be between 0-255"); }
            var color = rend.color;
            color.a = a;
            rend.color = color;
        }

        #endregion
    }

    #region Design Pattern Classes

    /// <summary> Base Class for implementing the Command Design Pattern </summary>
    public abstract class Command
    {
        /// <summary> Executes this Command </summary>
        public abstract void Execute();

        /// <summary> Undoes this Command </summary>
        public abstract void Undo();
    }

    #endregion

    #region Interfaces

    /// <summary> This GameObject can take damage </summary>
    public interface IDamageable<T>
    {
        T Health { get; set; }  // This GameObject's current health
        void Damage(T amount);  // Deal damage to this GameObject
    }

    /// <summary> This GameObject can be healed </summary>
    public interface IHealable<T>
    {
        void Heal(T amount);    // Heal this GameObject
    }

    /// <summary> This GameObject can be killed </summary>
    public interface IKillable
    {
        void Kill();    // Kill this GameObject
    }

    /// <summary> This GameObject can move </summary>
    public interface IMoveable<T>
    {
        T MoveSpeed { get; set; }   // This GameObject's movement speed
        void Move(T direction);     // Move this GameObject
    }

    /// <summary> This GameObject can jump </summary>
    public interface IJumpable<T>
    {
        void Jump();    // This Gameobject will jump
    }

    /// <summary> This GameObject can be picked up </summary>
    public interface IPickupable
    {
        void Pickup();    // Pickup this GameObject
    }

    /// <summary> This GameObject can be saved </summary>
    public interface IPersistable
    {
        void Save();    // Save this GameObject
    }

    /// <summary> This GameObject can shoot </summary>
    public interface IShootable
    {
        void Fire();    // This GameObject will shoot
    }

    /// <summary> This GameObject can be driven </summary>
    public interface IDrivable
    {
        void Drive();    // Drive this GameObject
    }

    #endregion

}
#region Credits
/// Script created by Tyler Nichols
#endregion