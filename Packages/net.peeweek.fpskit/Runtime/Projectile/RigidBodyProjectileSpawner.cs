using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public class RigidBodyProjectileSpawner : ProjectileSpawner
    {
        [SerializeField]
        GameObject referencePrefab;
        [SerializeField]
        float initialSpeed = 12;
        [SerializeField]
        float upwardsModifier = 0.025f;

        [Header("Instance Pool")]
        [Min(1)]
        [SerializeField]
        int poolSize = 12;
        [SerializeField]
        bool reapOldestInstance = false;

        // Private Fields
        LinkedList<RigidBodyProjectile> m_Instances;
        LinkedList<RigidBodyProjectile> m_AvailableInstances;

        void InitializePool()
        {
            m_Instances = new LinkedList<RigidBodyProjectile>();
            m_AvailableInstances = new LinkedList<RigidBodyProjectile>();

            referencePrefab.SetActive(false);
            for(int i = 0; i<poolSize; i++)
            {
                var go = Instantiate(referencePrefab);
                if (go.TryGetComponent(out RigidBodyProjectile projectile))
                    m_AvailableInstances.AddFirst(projectile);
                else
                    Debug.LogError($"Projectile prefab {referencePrefab} is missing a RigidBodyProjectile component", referencePrefab);
            }
        }

        List<GameObject> todelete;

        void ClearPool()
        {
            if (todelete == null)
                todelete = new List<GameObject>();
            else
                todelete.Clear();

            foreach (var proj in m_Instances)
                todelete.Add(proj.gameObject);

            foreach (var proj in m_AvailableInstances)
                todelete.Add(proj.gameObject);

            for (int i = 0; i < todelete.Count; i++)
                Destroy(todelete[i]);

            m_Instances.Clear();
            m_AvailableInstances.Clear();
        }

        public override void OnAttach(FirstPersonController controller)
        {
            base.OnAttach(controller);
            InitializePool();
        }

        public override void OnDetach(FirstPersonController controller)
        {
            base.OnDetach(controller);
            ClearPool();
        }

        private void OnDestroy()
        {
            if(m_Instances != null)
                while(m_Instances.Count > 0)
                {
                    Destroy(m_Instances.First.Value);
                    m_Instances.RemoveFirst();
                }

            if(m_AvailableInstances != null)
                while (m_AvailableInstances.Count > 0)
                {
                    Destroy(m_AvailableInstances.First.Value);
                    m_AvailableInstances.RemoveFirst();
                }
        }

        public override bool Spawn(Ray ray, RaycastHit hit, bool hitTarget)
        {
            if(m_AvailableInstances.Count == 0 && reapOldestInstance)
                Reap();

            if (m_AvailableInstances.Count == 0) // Still can't spawn
                return false;
            else
            {
                var source = weaponSource.position;
                var target = hit.point;

                // Apply Upwards Modifier;
                target.y += (target - source).magnitude * upwardsModifier;

                // Here we Spawn !
                GameObject go = m_AvailableInstances.First.Value.gameObject;
                m_AvailableInstances.RemoveFirst();
                m_Instances.AddFirst(go.GetComponent<RigidBodyProjectile>());
                RigidBodyProjectile instance;

                if(!go.TryGetComponent(out instance))
                {
                    instance = go.AddComponent<RigidBodyProjectile>();
                }

                instance.SetParent(this);
                go.SetActive(true);
                var rb = go.GetComponent<Rigidbody>();
                rb.Move(source, Quaternion.identity);
                rb.linearVelocity = (target-source).normalized * initialSpeed;
            }
            return true;
        }

        public void Reap(RigidBodyProjectile instance = null)
        {
            if(instance == null)
            {
                instance = m_Instances.Last.Value;
                m_Instances.RemoveLast();
            }
            else
            {
                var node = m_Instances.Find(instance);
                if (node == null)
                {
                    throw new System.Exception($"Could not find instance '{instance.gameObject.name}' in projectile pool '{name}'");
                }
                m_Instances.Remove(node);
            }
            instance.gameObject.SetActive(false);
            m_AvailableInstances.AddFirst(instance);
        }
    }
}

