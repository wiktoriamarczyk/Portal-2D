using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper struct for connecting the source and clone of the animation
/// </summary>
public struct CloneAnimPair
{
    /// <summary>
    /// Constructor of the struct
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    public CloneAnimPair(GameObject s, GameObject c)
    {
        source = s;
        clone = c;
    }

    public GameObject source;
    public GameObject clone;
}

/// <summary>
/// Class responsible for synchronizing the animation of the source object to the destination object
/// </summary>
public class CloneAnimSync : MonoBehaviour
{
    /// <summary>
    /// Source object
    /// </summary>
    [SerializeField] GameObject animSourceObject;
    /// <summary>
    /// Destination object
    /// </summary>
    [SerializeField] GameObject animDestRoot;
    /// <summary>
    /// List of pairs of source and destination objects
    /// </summary>
    List<CloneAnimPair> cloneAnimList;


    /// <summary>
    /// Method which starts the animation
    /// </summary>
    /// <param name="source">animation source</param>
    public void StartAnim(GameObject source)
    {
        animSourceObject = source;
        cloneAnimList = new List<CloneAnimPair>();
        if( animSourceObject == null || animDestRoot == null )
            return;

        Transform[] srcComponents = animSourceObject.GetComponentsInChildren<Transform>();
        Transform[] dstComponents = animDestRoot.GetComponentsInChildren<Transform>();
        foreach( Transform dstComponent in dstComponents )
        {
            foreach( Transform sourceComponent in srcComponents )
            {
                if( sourceComponent.name == dstComponent.name )
                {
                    cloneAnimList.Add(new CloneAnimPair(sourceComponent.gameObject, dstComponent.gameObject));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled. Here responsible for synchronizing the animation
    /// </summary>
    void Update()
    {
        foreach( CloneAnimPair pair in cloneAnimList )
        {
            pair.source.transform.GetLocalPositionAndRotation(out var localPos, out var localRot);
            // Clone local pose to destination transform in one operation.
            pair.clone.transform.SetLocalPositionAndRotation(localPos, localRot);
        }
    }
}
