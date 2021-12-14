#region Namespaces
using UnityEngine;
#endregion

/// <summary> Moves the Object this is attached to </summary>
public class BasicMove : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _speed = 1f;

    public void Move(Vector3 dir) { transform.Translate(dir * _speed); }
}
