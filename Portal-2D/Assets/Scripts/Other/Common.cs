using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
