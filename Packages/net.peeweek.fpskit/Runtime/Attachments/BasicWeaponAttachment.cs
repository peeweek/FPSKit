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
        protected bool continousShoot;
        [SerializeField]
        protected float shootDelay = 0.17f;

        [SerializeField]
        protected bool canShootWhileDash = false;
        [SerializeField]
        protected bool canShootWhileJump = true;

        [SerializeField]
        Effect[] effects;

        [Header("Recoil")]
        [SerializeField]
        bool recoil = false;
        [SerializeField]
        Vector2 recoilPitchMinMax = new Vector2(1.0f,1.5f);
        [SerializeField]
        float recoilYawJitter = 1.0f;

        [Header("Shoot Rumble")]
        [SerializeField]
        protected bool rumble;
        [SerializeField]
        protected AnimationCurve rumbleCurve = AnimationCurve.Linear(0, 1, 0.1f, 0);
        [SerializeField]
        protected Vector2 rumbleLowHighScale = Vector2.one;

        [Header("Projectile")]
        [SerializeField]
        protected Transform spawnerSource;
        [SerializeField]
        protected ProjectileSpawner projectileSpawner;

        // Accessors
        public Transform source { get => spawnerSource; }

        // Private Fields
        float m_TTL;

        public override void OnActive(FirstPersonController controller)
        {
            base.OnActive(controller);
            m_TTL = shootDelay;
        }

        public override void OnInactive(FirstPersonController controller)
        {
            base.OnInactive(controller);
        }

        Ray m_Ray;
        RaycastHit m_RaycastHit;

        public override void OnUpdate(FirstPersonController controller)
        {
            base.OnUpdate(controller);

            m_TTL += Time.deltaTime;

            ButtonState buttonState = controller.input.primaryAction;

            bool shoot = false;

            if (m_TTL > shootDelay
                && (controller.jump ? canShootWhileJump : true)
                && (controller.dash ? canShootWhileDash : true)
                )
            {
                if (continousShoot)
                    shoot = buttonState == ButtonState.JustPressed || buttonState == ButtonState.Pressed;
                else
                    shoot = buttonState == ButtonState.JustPressed;
            }

            if (shoot)
            {
                m_TTL = 0;
                animator.SetBool(animatorBoolShootProperty, true);
                if (projectileSpawner != null && spawnerSource != null)
                {
                    Vector3 source = Vector3.zero;
                    Vector3 target = Vector3.one;
                    bool hasHit = false;

                    m_Ray.origin = transform.position;
                    m_Ray.direction = transform.forward;
                    if (Physics.Raycast(m_Ray, out m_RaycastHit, projectileSpawner.maxDistance))
                    {
                        hasHit = true;
                    }
                    else
                    {
                        m_RaycastHit.point = m_Ray.origin + m_Ray.direction * projectileSpawner.maxDistance;
                        m_RaycastHit.normal = -m_Ray.direction;
                        hasHit = false;
                    }

                    projectileSpawner.Spawn(m_Ray, m_RaycastHit, hasHit);

                    // Play Effects
                    foreach(var effect in effects)
                    {
                        if (effect == null)
                            continue;

                        effect.ApplyEffect(source, target);
                    }

                    if(recoil)
                    {
                        Vector2 recoil = new Vector2(
                            -Random.Range(recoilPitchMinMax.x, recoilPitchMinMax.y),
                            Random.Range(-.5f, .5f) * recoilYawJitter);
                        controller.Recoil(recoil);
                    }
                }
                    
            }
            else
                animator.SetBool(animatorBoolShootProperty, false);

            if (rumble)
            {
                var gamepad = Gamepad.current;

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
