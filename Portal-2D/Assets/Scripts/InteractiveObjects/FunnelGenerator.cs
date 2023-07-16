using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FunnelGenerator : MonoBehaviour
{
    [SerializeField] GameObject funnelPrefab;
    [SerializeField] Material funnelMaterial;
    [SerializeField] bool dupa = false;
    [SerializeField] bool isEnabled = false;

    public float Health { get => this._health ; set => this._health = value * 2; }
    [SerializeField]
    private float _health = 0;


    static float raycastDistance = 500f;

    List<GameObject> funnelsList = new List<GameObject>();


    public void Awake()
    {
        PortalManager.OnPortalChange += Refresh;

        if(isEnabled)
            Generate();
    }

    private void Refresh()
    {
        if(!isEnabled)
            return;
        Generate();
    }

    public void EnableGenerator()
    {
        if(isEnabled)
            return;

        isEnabled = true;
        Generate();
    }

    public void DisableGenerator()
    {
        if(!isEnabled)
            return;
        isEnabled = false;
        DestroyFunnels();
    }

    private void DestroyFunnels()
    {
        foreach(var funnel in funnelsList)
            Destroy(funnel);

        funnelsList.Clear();
    }

    public void Generate()
    {
        DestroyFunnels();
        DoGenerate(gameObject,funnelPrefab, funnelsList,true);
    }

    static void DoGenerate(GameObject originObject,GameObject funnelPrefab, List<GameObject> funnelsList, bool recursionAllowed)
    {
        var startPoint = originObject.transform;
        BoxCollider2D boxCollider = originObject.GetComponent<BoxCollider2D>();
        Vector3 raycastStart = new Vector3(startPoint.position.x, startPoint.position.y, 1f);
        Vector3 raycastEnd = raycastStart + startPoint.right * 500;

        RaycastHit2D hit = Physics2D.Raycast(raycastStart + startPoint.right*5, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN | Common.eLayerType.PORTAL));
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

        var portal = hit.collider.GetComponent<PortalBehaviour>();
        if (portal != null && portal.otherEnd != null)
        {
            DoGenerate( portal.otherEnd.gameObject , funnelPrefab, funnelsList , false);
        }
    }

    private void Update()
    {
        if( dupa != isEnabled )
        {
            if( dupa )
                EnableGenerator();
            else
                DisableGenerator();
        }
    //    Vector3 raycastStart = new Vector3(transform.position.x, transform.position.y, 1f);
    //    Vector3 raycastEnd = raycastStart + transform.right * 500;
    //    RaycastHit2D hit = Physics2D.Raycast(raycastStart, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN));
    //    Debug.DrawLine(raycastStart, hit.point, Color.cyan, 200f);

    //    if (hit.collider != null && hit.collider.gameObject.GetComponent<PortalBehaviour>() != null)
    //    {
    //        Generate(otherPotal);
    //    }
    }
}
