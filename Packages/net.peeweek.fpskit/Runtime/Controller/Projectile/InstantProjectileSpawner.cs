using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public class InstantProjectileSpawner : ProjectileSpawner
    {
        public Transform weaponSource;

        public Effect[] traceEffects;
        public Effect[] impactEffects;

        public override bool Spawn(Ray ray, RaycastHit hit, bool hitTarget)
        {
            foreach(var effect in traceEffects)
            {
                effect.ApplyEffect(weaponSource.position, hit.point);
            }

            foreach(var effect in impactEffects)
            {
                effect.ApplyEffect(hit.point, hit.point + hit.normal);
            }

            return true;
        }
    }
}


