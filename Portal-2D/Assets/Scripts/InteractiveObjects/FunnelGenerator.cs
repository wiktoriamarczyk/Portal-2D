using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FunnelGenerator : MonoBehaviour
{
    [SerializeField] GameObject funnelPrefab;
    [SerializeField] Material funnelMaterial;
    GameObject funnel;
     Vector4 funnelScale;
    float raycastDistance = 500f;

    public void Awake()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector3 raycastStart = new Vector3(transform.position.x, transform.position.y, 1f);
        Vector3 raycastEnd = raycastStart + transform.right * 500;

        RaycastHit2D hit = Physics2D.Raycast(raycastStart, raycastEnd, raycastDistance, (int)(Common.eLayerType.TERRAIN));
        Debug.DrawLine(raycastStart, hit.point, Color.cyan, 200f);
        //Debug.DrawLine(transform.position, hit.point, Color.magenta, 200f);

        funnel = Instantiate(funnelPrefab, transform.position, Quaternion.identity);
        funnel.transform.SetParent(transform);
        funnel.transform.localRotation = Quaternion.identity;

        if (hit.collider != null)
        {
            funnelScale = new Vector4(hit.distance / funnel.GetComponent<BoxCollider2D>().bounds.size.x + 0.5f, 1, 1, 1);
        }
        else
        {
            float distance = Vector2.Distance(raycastStart, raycastEnd);
            funnelScale = new Vector4(distance / funnel.GetComponent<BoxCollider2D>().bounds.size.x, 1, 1, 1);
        }
        funnelMaterial.SetVector("_Scale", funnelScale);
        funnel.transform.localScale = funnelScale;
        Vector3 funnelPos = funnel.transform.position;
        funnel.transform.localPosition = new Vector3( funnel.GetComponent<BoxCollider2D>().bounds.size.x / 2 * funnelScale.x, 0, 0);
    }
}
