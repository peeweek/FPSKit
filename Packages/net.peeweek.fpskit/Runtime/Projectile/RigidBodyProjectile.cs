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

        [SerializeField]
        Effect[] onCollideEffects;


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
            foreach(var effect in onCollideEffects)
            {
                if (effect == null)
                    continue;

                var contact = collision.contacts[0];

                effect.ApplyEffect(contact.point, contact.point + contact.normal);
            }
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
