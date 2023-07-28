using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PortalBehaviour : MonoBehaviour
{
    public PortalBehaviour otherEnd;

    void Start()
    {
        int angle = ((int)(transform.rotation.eulerAngles.z+0.5f))%360;
        if ( angle >= 180  )
        {
            var interior = GetComponentInChildren<PortalCloner>().GetOwnInterior();
            interior.transform.Rotate(0, 0, 180);
            interior.transform.localScale = new Vector3(interior.transform.localScale.x * -1, interior.transform.localScale.y, interior.transform.localScale.z);
        }

        GetComponentInChildren<PortalCloner>().MakeTilesBehindPortalNonCollidable();

        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("OpenPortal");
    }

    public void InitDestroyment()
    {
        GetComponentInChildren<PortalCloner>().MakeTilesBehindPortalCollidable();


        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("ClosePortal");
    }

    // wywo³ane na koniec animacji zamykania portalu
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public static void Link(PortalBehaviour a, PortalBehaviour b)
    {
        if (a==null || b==null)
            return;

        a.otherEnd = b;
        b.otherEnd = a;

        var ac = a.GetComponentInChildren<PortalCloner>();
        var bc = b.GetComponentInChildren<PortalCloner>();

        ac.SetDestination(bc);
        bc.SetDestination(ac);
    }
}