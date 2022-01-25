using System.Collections;
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
        protected float recycleAfterExplodeDelay = 0;


        [SerializeField]
        Effect[] onCollideEffects;

        [SerializeField]
        Effect[] onExplodeEffects;


        protected RigidBodyProjectileSpawner m_Parent;
        protected int m_Collisions;
        protected float m_TTL;

        protected bool m_Exploding { get; private set; }

        public void SetParent(RigidBodyProjectileSpawner parent)
        {
            if (parent != null)
                m_Parent = parent;
        }

        /// <summary>
        /// Immediately recycles this projectile
        /// </summary>
        public void Recycle()
        {
            m_Parent?.Reap(this);
        }

        public void BeginExplode()
        {
            if (!m_Exploding)
                StartCoroutine(Explode());
        }


        /// <summary>
        /// Coroutine: Triggers an explosion, and after a delay, the recycling of the projectile
        /// </summary>
        /// <returns></returns>
        private IEnumerator Explode()
        {
            foreach (var effect in onExplodeEffects)
            {
                if (effect == null)
                    continue;

                effect.ApplyEffect(transform.position, transform.position + transform.up);
            }

            yield return new WaitForSeconds(recycleAfterExplodeDelay);
            Recycle();
        }

        protected virtual void OnEnable()
        {
            m_Collisions = 0;
            m_TTL = 0;
            m_Exploding = false;
        }

        protected virtual void OnCollisionEnter(Collision collision)
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

        protected virtual void Update()
        {
            if (m_Exploding)
                return;

            if (explodeOnCollide > 0 && m_Collisions >= explodeOnCollide)
                StartCoroutine(Explode());
            else if (explodeAfterDelay > 0 && m_TTL > explodeAfterDelay)
                StartCoroutine(Explode());
            else
                m_TTL += Time.deltaTime;
        }
    }
}
