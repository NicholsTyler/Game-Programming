#region Namespaces
    using UnityEngine;
    using UnityEngine.SceneManagement;
#endregion

namespace Utility.Tools
{
    /// <summary> Global helper methods </summary>
    public static class Global
    {
        /// <summary> Takes a screenshot</summary>
        /// <param name="superSize"></param>
        /// <param name="filePath"> Where to save the screenshot </param>
        static public void Capture(int superSize = 1, string filePath = "")
        {
            string fileName = $"Capture-{System.DateTime.Now.ToTimeStamp()}.png";
            ScreenCapture.CaptureScreenshot(fileName, superSize);

            Vector2 gameViewSize;
            #if UNITY_EDITOR
                gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
                Debug.LogWarning( $"Saved \"{fileName}\" "
                                + $"at {gameViewSize.x * superSize}x{gameViewSize.y * superSize}"
                                + " to project folder (the parent folder of Assets) !" );
            #else
                gameViewSize = new Vector2( Screen.width, Screen.height );
                Debug.LogWarning( $"Saved \"{fileName}\" "
                                + $"at {gameViewSize.x * superSize}x{gameViewSize.y * superSize}"
                                + $" to path: \"{filePath}\" !" );
            #endif
        }

        /// <summary> Finds what percentage a number is at between 2 bounds </summary>
        /// <param name="curNumber"> The number to determine percentage for </param>
        /// <param name="min"> Lower bound </param>
        /// <param name="max"> Upper bound </param>
        static public float RangePercentage(float curNumber, float min=0, float max=1)
        {
            return ((curNumber - min) * 100) / (max - min);
        }
    }
}

#region Credits
    /// Script created by Tyler Nichols
#endregion