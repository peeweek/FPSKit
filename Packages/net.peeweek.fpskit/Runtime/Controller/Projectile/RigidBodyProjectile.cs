using UnityEngine;

namespace FPSKit
{
    public class RigidBodyProjectile : Projectile
    {
        [SerializeField]
        GameObject prefab;
        [SerializeField]
        int maxInstanceCount;
        [SerializeField]
        bool reapOldestInstance;


        // Private Fields
        GameObject instances;

        public override Projectile CreateInstance()
        {
            RigidBodyProjectile projectile = MemberwiseClone() as RigidBodyProjectile;
            projectile.InitializePool();
            return projectile;
        }

        void InitializePool()
        {

        }

        public override bool SpawnProjectile(BasicWeaponAttachment source)
        {
            if(instances == null)
            {
                Debug.LogError("Cannot Spawn Projectile : Projectile asset must be instantiated at runtime using CreateInstance() method");
                return false;
            }
            return true;
        }
    }
}

