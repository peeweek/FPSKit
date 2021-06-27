using UnityEngine;

namespace FPSKit
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyProjectile : MonoBehaviour
    {
        [SerializeField]
        protected int explodeOnCollide = 0;
        [SerializeField]
        protected float explodeAfterDelay = 0;

        RigidBodyProjectileSpawner parent;
        int m_Collisions;
        float m_TTL;

        public void SetParent(RigidBodyProjectileSpawner parent)
        {
            if (parent != null)
                this.parent = parent;
        }

        public void Recycle()
        {
            parent?.Reap(this);
        }

        public void Explode()
        {
            Recycle();
        }

        private void OnEnable()
        {
            m_Collisions = 0;
            m_TTL = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            m_Collisions++;
        }

        void Update()
        {
            if (explodeOnCollide > 0 && m_Collisions >= explodeOnCollide)
                Explode();

            if (explodeAfterDelay > 0 && m_TTL > explodeAfterDelay)
                Explode();

            m_TTL += Time.deltaTime;
        }
    }
}
