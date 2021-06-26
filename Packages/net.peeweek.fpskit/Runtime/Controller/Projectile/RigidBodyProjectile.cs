using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public class RigidBodyProjectile : Projectile
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
        LinkedList<GameObject> m_Instances;
        LinkedList<GameObject> m_AvailableInstances;

        void InitializePool()
        {
            m_Instances = new LinkedList<GameObject>();
            m_AvailableInstances = new LinkedList<GameObject>();

            referencePrefab.SetActive(false);
            for(int i = 0; i<poolSize; i++)
            {
                m_AvailableInstances.AddFirst(Instantiate(referencePrefab));
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

        public override bool Spawn(BasicWeaponAttachment source)
        {
            if(m_AvailableInstances.Count == 0 && reapOldestInstance)
                Reap();

            if (m_AvailableInstances.Count == 0) // Still can't spawn
                return false;
            else
            {
                // Here we spawn!
                GameObject go = m_AvailableInstances.First.Value;
                m_AvailableInstances.RemoveFirst();
                m_Instances.AddFirst(go);
                RigidBodyProjectileInstance instance;

                if(!go.TryGetComponent(out instance))
                {
                    instance = go.AddComponent<RigidBodyProjectileInstance>();
                }

                instance.parent = this;
                go.SetActive(true);
                go.transform.position = source.source.position;
                go.GetComponent<Rigidbody>().velocity = source.source.forward * initialSpeed;

            }
            return true;
        }

        public void Reap(GameObject instance = null)
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
                    throw new System.Exception($"Could not find instance '{instance.name}' in projectile pool '{name}'");
                }
                m_Instances.Remove(node);
            }

            m_AvailableInstances.AddFirst(instance);
        }
    }
}

