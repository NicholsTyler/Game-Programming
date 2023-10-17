#region Namespaces
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
#endregion

namespace Utility.Tools
{
    public static class Extensions
    {
        #region Type
            /// <returns> A random element </returns>
            public static T Rand<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

            /// <summary> Saves the contents of a class as a Json file </summary>
            /// <typeparam name="T"> The Class Type </typeparam>
            /// <param name="fileName"> The file name WITHOUT .json </param>
            /// <param name="classInstance"> An instance of a class </param>
            public static void SaveJson<T>(this T classInstance, string fileName) where T : MonoBehaviour
            {
                string text = JsonUtility.ToJson(classInstance);
                System.IO.File.WriteAllText("Assets/Resources/Json/" + fileName + ".json", text);
            }

            /// <summary> Loads a Json file to a specified class </summary>
            /// <typeparam name="T"> The Class Type </typeparam>
            /// <param name="fileName"> The file name WITHOUT .json </param>
            public static T LoadJson<T>(this string fileName) where T : MonoBehaviour
            {
                TextAsset textAsset = Resources.Load<TextAsset>("Json/" + fileName); 
                return JsonUtility.FromJson<T>(textAsset.text);
            }
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
            public static Vector3 GetPos(this GameObject gameObject) => gameObject.transform.position;

            /// <summary> Sets this GameObject's Position </summary>
            public static void SetPos(this GameObject gameObject, float x = 0, float y = 0, float z = 0) => gameObject.transform.position = new Vector3(x, y, z);

            /// <returns> This GameObject's Rotation </returns>
            public static Quaternion GetRot(this GameObject gameObject) => gameObject.transform.rotation;

            /// <summary> Sets this GameObject's Rotation </summary>
            public static void SetRot(this GameObject gameObject, float x = 0, float y = 0, float z = 0) => gameObject.transform.rotation = Quaternion.Euler(x, y, z);

            /// <returns> This GameObject's Scale </returns>
            public static Vector3 GetScale(this GameObject gameObject) => gameObject.transform.localScale;

            /// <summary> Sets this GameObject's Scale </summary>
            public static void SetScale(this GameObject gameObject, float x = 0, float y = 0, float z = 0) => gameObject.transform.localScale = new Vector3(x, y, z);
        #endregion
        #region Transform
            /// <summary> Destroys all child objects of this Transform </summary>
            public static void DestroyChildren(this Transform t)
            {
                foreach(Transform child in t) { Object.Destroy(child.gameObject); }
            }

            /// <summary> Gets nearest transform from a list of transforms </summary>
            /// <param name="transforms"> List of transforms to search through </param>
            public static Transform GetNearest (this Transform curTransform, Transform[] transforms)
            {
                Transform bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = curTransform.position;
                foreach(Transform potentialTarget in transforms)
                {
                    Vector3 directionToTarget = potentialTarget.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if(dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                }
        
                return bestTarget;
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
        #region String
            /// <summary> Converts a string to a color </summary>
            public static Color ToColor(this string color)
            {
                bool valid = ColorUtility.TryParseHtmlString(color, out Color newColor);
                return valid ? newColor : Color.clear;
            }
        #endregion
        #region DateTime
            /// <returns> The current time as a time stamp </returns>
            static public string ToTimeStamp(this System.DateTime time)
            {
                string stamp = $"{time.Year:0000}-{time.Month:00}-{time.Day:00}_"
                            + $"{time.Hour:00}{time.Minute:00}";
                return stamp;
            }
        #endregion
        #region SceneManagement
            static public void ReloadScene(this SceneManager sceneManager)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        #endregion
    }
}
#region Credits
    /// Script created by Tyler Nichols
#endregion