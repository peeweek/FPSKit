using System;
using UnityEngine;

namespace FPSKit
{ 
    /// <summary>
    /// The abstract representation of a projectile that can be spawned by any weapon
    /// </summary>
    public abstract class ProjectileSpawnerBase : MonoBehaviour
    {
        public Transform weaponSource;
        public float maxDistance = 50f;

        public Action OnShoot;

        protected FirstPersonController controller;
        protected BasicWeaponAttachment attachment;

        public abstract void OnActive();
        public abstract void OnInactive(); 
        public abstract void OnUpdate();

        public virtual void OnAttach(FirstPersonController controller, BasicWeaponAttachment attachment) 
        { 
            this.controller = controller;
            this.attachment = attachment;
        }

        public virtual void OnDetach() { }
    }
}
