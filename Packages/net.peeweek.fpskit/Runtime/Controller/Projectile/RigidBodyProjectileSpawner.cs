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

        [Header("Instance Pool")]
        [Min(12)]
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

        private void Awake()
        {
            InitializePool();
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

        public override bool Spawn(Vector3 source, Vector3 target, bool hitTarget)
        {
            if(m_AvailableInstances.Count == 0 && reapOldestInstance)
                Reap();

            if (m_AvailableInstances.Count == 0) // Still can't spawn
                return false;
            else
            {
                // Here we spawn!
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
                go.transform.position = source;
                go.GetComponent<Rigidbody>().velocity = (target-source).normalized * initialSpeed;
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

