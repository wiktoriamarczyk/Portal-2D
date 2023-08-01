using UnityEngine;

/// <summary>
/// Helper class for objects which can be cloned
/// </summary>
public class Cloner : MonoBehaviour
{
    /// <summary>
    /// Target of the cloner
    /// </summary>
    [SerializeField] GameObject target;
    /// <summary>
    /// Object to clone
    /// </summary>
    [SerializeField] GameObject objectToClone;
}
