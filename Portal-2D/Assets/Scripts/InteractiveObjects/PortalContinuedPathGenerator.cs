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
        var startPoint = originObject.transform;
        BoxCollider2D boxCollider = originObject.GetComponent<BoxCollider2D>();
        Vector3 raycastStart = new Vector3(startPoint.position.x, startPoint.position.y, 1f);
        Vector3 raycastEnd = raycastStart + startPoint.right * 500;

        RaycastHit2D hit = Physics2D.Raycast(raycastStart + startPoint.right*5, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN | Common.eLayerType.PORTAL | (Common.eLayerType.NON_PORTAL)));
        Debug.DrawLine(raycastStart, hit.point, Color.cyan, 200f);
        //Debug.DrawLine(transform.position, hit.point, Color.magenta, 200f);

        var funnel = Instantiate(funnelPrefab, startPoint.position, Quaternion.identity);
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
        Vector3 funnelPos = funnel.transform.position;
        funnel.transform.localPosition = new Vector3( funnel.GetComponent<BoxCollider2D>().bounds.size.x / 2 * funnelScale.x, 0, 0);


        if(hit.collider == null || !recursionAllowed)
            return;

        var portallogic = hit.collider.GetComponent<PortalLogic>();
        if ( portallogic == null )
            return;

        //var portal = portalCloner.GetPortalBehaviour();
        if (portallogic != null && portallogic.GetDestinationOutput() != null)
        {
            DoGenerate( portallogic.GetDestinationOutput().gameObject , funnelPrefab, funnelsList , false);
        }
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
