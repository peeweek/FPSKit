using UnityEngine;

namespace FPSKit
{ 
    public abstract class Projectile : ScriptableObject
    {
        public abstract Projectile CreateInstance();

        public abstract bool SpawnProjectile(BasicWeaponAttachment source);
    }
}
