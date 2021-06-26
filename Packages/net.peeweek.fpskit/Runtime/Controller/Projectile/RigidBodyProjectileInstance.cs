using UnityEngine;

namespace FPSKit
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyProjectileInstance : MonoBehaviour
    {
        public RigidBodyProjectile parent;

        public void Recycle()
        {
            parent?.Reap(gameObject);
        }
    }
}
