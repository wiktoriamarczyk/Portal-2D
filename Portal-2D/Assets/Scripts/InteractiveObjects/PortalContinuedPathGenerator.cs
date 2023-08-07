using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for generating the path which can be continued by the portal
/// </summary>
public class PortalContinuedPathGenerator : MonoBehaviour
{
    /// <summary>
    /// Prefab of the path
    /// </summary>
    [SerializeField] GameObject pathPrefab;
    /// <summary>
    /// Material of the path
    /// </summary>
    [SerializeField] Material pathMaterial;
    /// <summary>
    /// Is the generator enabled?
    /// </summary>
    [SerializeField] bool enable = false;
    /// <summary>
    /// Is the generator enabled on start?
    /// </summary>
    [SerializeField] bool isEnabledOnStart = false;
    /// <summary>
    /// Distance of the raycast
    /// </summary>
    static float raycastDistance = 500f;
    /// <summary>
    /// List of funnels
    /// </summary>
    List<GameObject> pathList = new List<GameObject>();

    /// <summary>
    /// Awake is called when the script instance is being loaded. Here we subscribe to the portal change event
    /// </summary>
    void Awake()
    {
        PortalManager.OnPortalChange += Refresh;

        if(isEnabledOnStart)
            Generate();
    }

    /// <summary>
    /// Deinitialize
    /// </summary>
    private void OnDestroy()
    {
        PortalManager.OnPortalChange -= Refresh;
    }

    /// <summary>
    /// Method called when portal was spawned, regenerates the path
    /// </summary>
    void Refresh()
    {
        if(!isEnabledOnStart)
            return;
        Generate();
    }
    /// <summary>
    /// Method which activates the generator
    /// </summary>
    public void EnableGenerator()
    {
        if(isEnabledOnStart)
            return;

        isEnabledOnStart = true;
        Generate();
    }
    /// <summary>
    /// Method which deactivates the generator
    /// </summary>
    public void DisableGenerator()
    {
        if(!isEnabledOnStart)
            return;
        isEnabledOnStart = false;
        DestroyPaths();
    }
    /// <summary>
    /// Method which destroys all generated paths
    /// </summary>
    void DestroyPaths()
    {
        foreach(var funnel in pathList)
            Destroy(funnel);

        pathList.Clear();
    }
    /// <summary>
    /// Method which invokes generatation of the path
    /// </summary>
    public void Generate()
    {
        Invoke("DoGenerate", 0.1f);
    }
    /// <summary>
    /// Method which regenerates the path
    /// </summary>
    public void DoGenerate()
    {
        DestroyPaths();
        DoGenerate(gameObject, pathPrefab, pathList, true);
    }
    /// <summary>
    ///  Generates the path
    /// </summary>
    /// <param name="originObject">generator</param>
    /// <param name="pathPrefab">prefab of the path</param>
    /// <param name="paths">list of paths</param>
    /// <param name="recursionAllowed">is continued by the portal</param>
    static void DoGenerate(GameObject originObject,GameObject pathPrefab, List<GameObject> paths, bool recursionAllowed)
    {
        var directionVectorMultiplier = Vector3.one;

        {
            var portalLogic = originObject.GetComponent<PortalLogic>();
            if (portalLogic!=null)
            {
                originObject = portalLogic.GetDestinationOutput().gameObject;
                directionVectorMultiplier = portalLogic.GetObjectXFlipFactor();
                //directionVectorMultiplier.x *= -1;
            }
        }

        Vector3 pathDirectionVector = CommonFunctions.VectorLocalToWorld(originObject.transform, Vector3.Scale(Vector3.right, directionVectorMultiplier)).normalized;
        Vector3 startPoint   = originObject.transform.position;
        Vector3 raycastStart = new Vector3(startPoint.x, startPoint.y, 0f);
        Vector3 raycastEnd   = raycastStart + pathDirectionVector * 500;

        RaycastHit2D hit = Physics2D.Raycast(raycastStart + pathDirectionVector*5, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN | Common.eLayerType.PORTAL | (Common.eLayerType.NON_PORTAL)));
        Debug.DrawLine(raycastStart, hit.point, Color.cyan, 200f);
        //Debug.DrawLine(transform.position, hit.point, Color.magenta, 200f);

        var funnel = Instantiate(pathPrefab, startPoint, Quaternion.identity);
        funnel.transform.SetParent(originObject.transform);
        funnel.transform.localRotation = Quaternion.identity;
        paths.Add(funnel);

        Vector4 funnelScale;
        if (hit.collider != null)
        {
            funnelScale = new Vector4((hit.distance+5) / funnel.GetComponent<BoxCollider2D>().bounds.size.x + 0.5f, 1, 1, 1);
        }
        else
        {
            float distance = Vector2.Distance(raycastStart, raycastEnd);
            funnelScale = new Vector4(distance / funnel.GetComponent<BoxCollider2D>().bounds.size.x, 1, 1, 1);
        }
        funnel.GetComponent<SpriteRenderer>().material.SetVector("_Scale", funnelScale);
        funnel.transform.localScale = funnelScale;
        funnel.transform.localPosition = new Vector3( funnel.GetComponent<BoxCollider2D>().bounds.size.x / 2 * funnelScale.x, 0, 0);

        if(hit.collider == null || !recursionAllowed)
            return;

        var portallogic = hit.collider.GetComponent<PortalLogic>();
        if (portallogic == null || portallogic.IsDying() || portallogic.GetDestinationOutput() == null)
            return;

        DoGenerate(portallogic.gameObject, pathPrefab, paths, false);
    }
    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled. Here we check if the generator should be enabled or disabled
    /// </summary>
    void Update()
    {
        if (enable != isEnabledOnStart)
        {
            if (enable)
                EnableGenerator();
            else
                DisableGenerator();
        }
    }

    /// <summary>
    /// Toggles funnel accordingly with user interactions
    /// </summary>

    public void ToggleEnable()
    {
        if (enable)
        {
            DisableGenerator();
            enable = false;
        }
        else
        {
            EnableGenerator();
            enable = true;
        }


    }
}
