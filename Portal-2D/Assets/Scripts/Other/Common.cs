using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;


namespace Common {

    enum eLayerType
    {
        DEFAULT                 = 1 << 0, // 000000001
        FOREGROUND              = 1 << 7, // 010000000
        TERRAIN                 = 1 << 8, // 100000000
        NON_PORTAL              = 1 << 9,
        UNITS                   = 1 << 10,
        BACKGROUND              = 1 << 12,
        PLAYER                  = 1 << 11,
        NON_COLLIDABLE_UNITS    = 1 << 14,
        PORTAL                  = 1 << 15
    }
}
/// <summary>
/// Class containing common functions
/// </summary>
public class CommonFunctions
{
    static public Vector3 TransformPosBetweenPortals(Vector3 absPos, GameObject srcPortal, GameObject dstPortal)
    {
        if (srcPortal==null || dstPortal==null)
            return absPos;

        Vector3 objectSrcPortalLocPos = srcPortal.transform.worldToLocalMatrix.MultiplyPoint(absPos);
        Vector3 objectDstPortalLocPos = objectSrcPortalLocPos;
        objectDstPortalLocPos.x *= -1;

        return dstPortal.transform.localToWorldMatrix.MultiplyPoint( objectDstPortalLocPos );
    }

    static public Vector3 PointLocalToWorld(Transform parent, Vector3 pos)
    {
        return parent.localToWorldMatrix.MultiplyPoint(pos);
    }
    static public Vector2 PointLocalToWorld(Transform parent, Vector2 pos)
    {
        var p = PointLocalToWorld(parent, new Vector3(pos.x, pos.y, 0));
        return new Vector2(p.x, p.y);
    }
    static public Vector3 PointWorldToLocal(Transform parent, Vector3 pos)
    {
        var result = parent.worldToLocalMatrix.MultiplyPoint(pos);
        return result;
    }
    static public Vector2 PointWorldToLocal(Transform parent, Vector2 pos)
    {
        var p = PointWorldToLocal(parent, new Vector3(pos.x, pos.y, 0));
        return new Vector2(p.x, p.y);
    }

    static public Vector3 VectorLocalToWorld(Transform parent, Vector3 vec)
    {
        var A = PointLocalToWorld(parent,vec);
        var B = PointLocalToWorld(parent,Vector3.zero);
        return A-B;
    }
    static public Vector2 VectorLocalToWorld(Transform parent, Vector2 vec)
    {
        var A = PointLocalToWorld(parent,vec);
        var B = PointLocalToWorld(parent,Vector2.zero);
        return A-B;
    }
    static public Vector3 VectorWorldToLocal(Transform parent, Vector3 vec)
    {
        var A = PointWorldToLocal(parent,vec);
        var B = PointWorldToLocal(parent,Vector3.zero);
        return A-B;
    }
    static public Vector2 VectorWorldToLocal(Transform parent, Vector2 vec)
    {
        var A = PointWorldToLocal(parent,vec);
        var B = PointWorldToLocal(parent,Vector2.zero);
        return A-B;
    }

    /// <summary>
    /// Finds the nearest point on the line
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        Vector3 lhs = point - origin;

        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
    }
}
