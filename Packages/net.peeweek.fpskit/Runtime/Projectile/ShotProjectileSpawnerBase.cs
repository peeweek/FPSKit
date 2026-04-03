using UnityEngine;
using UnityEngine.InputSystem;

namespace FPSKit
{
    public abstract class ShotProjectileSpawnerBase : ProjectileSpawnerBase
    {
        [Header("Shot")]

        [SerializeField]
        protected bool continousShoot;
        [SerializeField]
        protected float shootDelay = 0.17f;

        [SerializeField]
        Effect[] effects;

        [Header("Rumble")]
        [SerializeField]
        protected bool rumble;
        [SerializeField]
        protected AnimationCurve rumbleCurve = AnimationCurve.Linear(0, 1, 0.1f, 0);
        [SerializeField]
        protected Vector2 rumbleLowHighScale = Vector2.one;        

        /// <summary>
        /// Called upon shooting, game logic that spawns a projectile based on settings.
        /// Implement this method to actually do something when needing to spawn
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected abstract bool Spawn(Ray ray, RaycastHit hit, bool hitTarget);

        public override void OnActive()
        {
            m_TTL = shootDelay;
        }

        public override void OnInactive()
        {

        }
        
        // Private Fields
        private float m_TTL;
        private Ray m_Ray;
        private RaycastHit m_RaycastHit;

        public override void OnUpdate()
        {
            ButtonState buttonState = controller.input.primaryAction;

            m_TTL += Time.deltaTime;

            bool shoot = false;

            if (m_TTL > shootDelay && attachment.CanShoot())
            {
                if (continousShoot)
                    shoot = buttonState == ButtonState.JustPressed || buttonState == ButtonState.Pressed;
                else
                    shoot = buttonState == ButtonState.JustPressed;
            }

            if (shoot)
            {
                m_TTL = 0;
                if (attachment.spawnerSource != null)
                {
                    Vector3 source = Vector3.zero;
                    Vector3 target = Vector3.one;
                    bool hasHit = false;

                    m_Ray.origin = transform.position;
                    m_Ray.direction = transform.forward;
                    if (Physics.Raycast(m_Ray, out m_RaycastHit, maxDistance))
                    {
                        hasHit = true;
                    }
                    else
                    {
                        m_RaycastHit.point = m_Ray.origin + m_Ray.direction * maxDistance;
                        m_RaycastHit.normal = -m_Ray.direction;
                        hasHit = false;
                    }

                    Spawn(m_Ray, m_RaycastHit, hasHit);

                    foreach (var effect in effects)
                    {
                        if(effect == null) continue;
                        effect.ApplyEffect(this.transform.position, this.transform.position);
                    }

                    if(OnShoot != null) OnShoot();
                }
            }


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


