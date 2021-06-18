using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public abstract class FirstPersonCamera : MonoBehaviour
    {
        public abstract float fov { get; set; }

        public Vector2Int screenSplit = Vector2Int.one;
        public int screenIndex;
    }
}


