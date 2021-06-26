using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace FPSKit
{
    public class BasicWeaponAttachment : LocomotionAttachment
    {
        [Header("Basic Weapon Attachment")]
        [SerializeField]
        protected InputAction shootButton;

        [SerializeField]
        protected string animatorBoolShootProperty = "Shoot";
        [SerializeField]
        protected bool continousShoot;
        [SerializeField]
        protected float shootDelay = 0.17f;

        [SerializeField]
        protected bool canShootWhileDash = false;
        [SerializeField]
        protected bool canShootWhileJump = true;

        [Header("Shoot Rumble")]
        [SerializeField]
        protected bool rumble;
        [SerializeField]
        protected AnimationCurve rumbleCurve = AnimationCurve.Linear(0, 1, 0.1f, 0);
        [SerializeField]
        protected Vector2 rumbleLowHighScale = Vector2.one;

        [Header("Projectile")]
        [SerializeField]
        protected Transform projectileSource;
        [SerializeField]
        protected Projectile projectile;

        // Private Fields
        float m_TTL;
        ButtonControl m_ShootButtonControl;

        public override void OnActive(FirstPersonController controller)
        {
            base.OnActive(controller);
            shootButton.Enable();
            m_TTL = shootDelay;
        }

        public override void OnInactive(FirstPersonController controller)
        {
            base.OnInactive(controller);
            shootButton.Disable();
        }

        public override void OnUpdate(FirstPersonController controller)
        {
            base.OnUpdate(controller);

            m_TTL += Time.deltaTime;

            m_ShootButtonControl = shootButton.controls[0] as ButtonControl;

            bool shoot = false;

            if (m_ShootButtonControl != null
                && m_TTL > shootDelay
                && (controller.jump ? canShootWhileJump : true)
                && (controller.dash ? canShootWhileDash : true)
                )
            {
                if (continousShoot)
                    shoot = m_ShootButtonControl.isPressed;
                else
                    shoot = m_ShootButtonControl.wasPressedThisFrame;
            }

            if (shoot)
            {
                m_TTL = 0;
                animator.SetBool(animatorBoolShootProperty, true);
                if (projectile != null && projectileSource != null)
                    projectile.SpawnProjectile(this);
            }
            else
                animator.SetBool(animatorBoolShootProperty, false);

            if (rumble)
            {
                var gamepad = m_ShootButtonControl.device as Gamepad;
                if (gamepad != null)
                {
                    if (m_TTL > rumbleCurve.keys[rumbleCurve.length - 1].time)
                    {
                        gamepad.SetMotorSpeeds(0, 0);
                    }
                    else if (m_TTL >= 0)
                    {
                        var f = rumbleCurve.Evaluate(m_TTL);
                        gamepad.SetMotorSpeeds(f * rumbleLowHighScale.x, f * rumbleLowHighScale.y);
                    }
                }
            }
        }
    }
}
