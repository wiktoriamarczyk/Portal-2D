using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PortalContinuedPathGenerator : MonoBehaviour
{
    [SerializeField] GameObject pathPrefab;
    [SerializeField] Material pathMaterial;
    [SerializeField] bool enable = false;
    [SerializeField] bool isEnabledOnStart = false;

    static float raycastDistance = 500f;
    List<GameObject> funnelsList = new List<GameObject>();

    void Awake()
    {
        PortalManager.OnPortalChange += Refresh;

        if(isEnabledOnStart)
            Generate();
    }

    void Refresh()
    {
        if(!isEnabledOnStart)
            return;
        Generate();
    }

    public void EnableGenerator()
    {
        if(isEnabledOnStart)
            return;

        isEnabledOnStart = true;
        Generate();
    }

    public void DisableGenerator()
    {
        if(!isEnabledOnStart)
            return;
        isEnabledOnStart = false;
        DestroyFunnels();
    }

    void DestroyFunnels()
    {
        foreach(var funnel in funnelsList)
            Destroy(funnel);

        funnelsList.Clear();
    }

    public void Generate()
    {
        Invoke("DoGenerate", 0.1f);
    }
    public void DoGenerate()
    {
        DestroyFunnels();
        DoGenerate(gameObject, pathPrefab, funnelsList, true);
    }

    static void DoGenerate(GameObject originObject,GameObject funnelPrefab, List<GameObject> funnelsList, bool recursionAllowed)
    {
        var directionVectorMultiplier = Vector3.one;

        {
            var portalLogic = originObject.GetComponent<PortalLogic>();
            if (portalLogic!=null)
            {
                originObject = portalLogic.GetDestinationOutput().gameObject;
                directionVectorMultiplier = portalLogic.GetObjectXFlipFactor();
                directionVectorMultiplier.x *= -1;
            }
        }

        Vector3 pathDirectionVector = CommonFunctions.VectorLocalToWorld(originObject.transform, Vector3.Scale(Vector3.right, directionVectorMultiplier)).normalized;
        Vector3 startPoint   = originObject.transform.position;
        Vector3 raycastStart = new Vector3(startPoint.x, startPoint.y, 0f);
        Vector3 raycastEnd   = raycastStart + pathDirectionVector * 500;

        RaycastHit2D hit = Physics2D.Raycast(raycastStart + pathDirectionVector*5, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN | Common.eLayerType.PORTAL | (Common.eLayerType.NON_PORTAL)));
        Debug.DrawLine(raycastStart, hit.point, Color.cyan, 200f);
        //Debug.DrawLine(transform.position, hit.point, Color.magenta, 200f);

        var funnel = Instantiate(funnelPrefab, startPoint, Quaternion.identity);
        funnel.transform.SetParent(originObject.transform);
        funnel.transform.localRotation = Quaternion.identity;
        funnelsList.Add(funnel);

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

        DoGenerate(portallogic.gameObject, funnelPrefab, funnelsList, false);
    }

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
}
