using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FunnelGenerator : MonoBehaviour
{
    [SerializeField] GameObject funnelPrefab;
    float raycastDistance = 500f;

    public void Awake()
    {
        GameObject funnel = Instantiate(funnelPrefab, transform.position, Quaternion.identity);
        funnel.transform.SetParent(transform.parent);
        funnel.transform.localScale = transform.localScale;
        funnel.transform.localRotation = transform.localRotation;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward, raycastDistance, (int)(Common.eLayerType.TERRAIN));
        Debug.DrawLine(transform.position, transform.position+ this.transform.right*100, Color.cyan, 200f);
        //Debug.DrawLine(transform.position, hit.point, Color.magenta, 200f);

        if (hit.collider != null)
        {
            Vector3 funnelPosition = hit.point;
            funnelPosition.z = 0f;
            funnel.transform.position = funnelPosition;
        }
    }
}
