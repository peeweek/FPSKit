using UnityEngine;

namespace FPSKit
{ 
    /// <summary>
    /// The abstract representation of a projectile that can be spawned by any weapon
    /// </summary>
    public abstract class ProjectileSpawner : MonoBehaviour
    {
        public Transform weaponSource;
        public float maxDistance = 50f;

        /// <summary>
        /// Called upon shooting, game logic that spawns a projectile based on settings.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public abstract bool Spawn(Ray ray, RaycastHit hit, bool hitTarget);
    }
}
