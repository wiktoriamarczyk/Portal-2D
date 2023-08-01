using UnityEngine;

/// <summary>
/// Class responsible for managing portal behaviour
/// </summary>
public class PortalBehaviour : MonoBehaviour
{
    /// <summary>
    /// Opposite portal
    /// </summary>
    public PortalBehaviour otherEnd;
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        int angle = ((int)(transform.rotation.eulerAngles.z+0.5f))%360;
        if ( angle >= 180  )
        {
            var interior = GetComponentInChildren<PortalLogic>().GetOwnInterior();
            interior.transform.Rotate(0, 0, 180);
            interior.transform.localScale = new Vector3(interior.transform.localScale.x * -1, interior.transform.localScale.y, interior.transform.localScale.z);
        }

        GetComponentInChildren<PortalLogic>().MakeTilesBehindPortalNonCollidable();

        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("OpenPortal");
    }
    /// <summary>
    /// Method responsible for initialize portal destroyment
    /// </summary>
    public void InitDestroyment()
    {
        GetComponentInChildren<PortalLogic>().OnDestroyBegin();

        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("ClosePortal");
    }
    /// <summary>
    /// Method responsible for destroying portal
    /// </summary>
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Method responsible for linking two portals
    /// </summary>
    /// <param name="a">first portal</param>
    /// <param name="b">second portal</param>
    public static void Link(PortalBehaviour a, PortalBehaviour b)
    {
        if (a==null || b==null)
            return;

        a.otherEnd = b;
        b.otherEnd = a;

        var ac = a.GetComponentInChildren<PortalLogic>();
        var bc = b.GetComponentInChildren<PortalLogic>();

        ac.SetDestination(bc);
        bc.SetDestination(ac);
    }
}