using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace FPSKit
{
    public class BasicWeaponAttachment : LocomotionAttachment
    {
        [Header("Basic Weapon Attachment")]

        [SerializeField]
        protected string animatorBoolShootProperty = "Shoot";

        [SerializeField]
        protected bool canShootWhileDash = false;

        [field: SerializeField]
        protected bool canShootWhileJump = true;

        [Header("Recoil")]
        [SerializeField]
        bool recoil = false;
        [SerializeField]
        Vector2 recoilPitchMinMax = new Vector2(1.0f,1.5f);
        [SerializeField]
        float recoilYawJitter = 1.0f;


        [Header("Projectile")]
        [field:SerializeField]
        public Transform spawnerSource {get; protected set; }
        [SerializeField]
        protected ProjectileSpawnerBase projectileSpawner;

        // Accessors
        public Transform source { get => spawnerSource; }

        public bool CanShoot()
        {
            return (this.controller.jump ? this.canShootWhileJump : true)
                && (this.controller.dash ? this.canShootWhileDash : true);
        }

        public override void OnActive()
        {
            base.OnActive();
            projectileSpawner?.OnActive();
        }

        public override void OnInactive()
        {
            base.OnInactive();
            projectileSpawner?.OnInactive();
        }

        public override void OnAttach(FirstPersonController controller)
        {
            base.OnAttach(controller);
            projectileSpawner?.OnAttach(controller, this);
            projectileSpawner.OnShoot += OnShoot;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            projectileSpawner?.OnDetach();
            projectileSpawner.OnShoot -= OnShoot;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            animator.SetBool(animatorBoolShootProperty, false);
            projectileSpawner?.OnUpdate();
        }

        protected virtual void OnShoot() 
        {
            animator.SetBool(animatorBoolShootProperty, true);
            if(recoil)
            {
                Vector2 recoil = new Vector2(
                    -Random.Range(recoilPitchMinMax.x, recoilPitchMinMax.y),
                    Random.Range(-.5f, .5f) * recoilYawJitter);
                controller.Recoil(recoil);
            }
        }
    }
}
