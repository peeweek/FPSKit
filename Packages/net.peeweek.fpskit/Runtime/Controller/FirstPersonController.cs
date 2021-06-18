using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Features")]
        public bool Paused = false;
        public bool CanMove = true;
        public bool CanLook = true;
        public bool CanJump = true;
        public bool CanDash = true;
        public bool CanCrouch = true;


        [Header("Input")]
        public FirstPersonInput input;

        [Header("Body Configuration")]
        [SerializeField]
        float skinWidth = 0.08f;
        [SerializeField]
        float bodyRadius = 0.4f;
        [SerializeField]
        float bodyHeight = 1.95f;

        [Header("Movement")]
        [SerializeField]
        float moveSpeed = 5f;
        [SerializeField]
        float minMoveDistance = 0.001f;
        [SerializeField]
        float slopeLimit = 45;
        [SerializeField]
        float stepOffset = 0.3f;

        [Header("Momentum")]
        [SerializeField]
        float terminalSpeed = 52.7f;
        [SerializeField]
        float gravityScale = 1.95f;
        [SerializeField]
        float groundFriction = 5.7f;
        [SerializeField]
        float slopeSlide = 16.1f;

        [Header("Look")]
        [SerializeField]
        float turnSpeed = 360;
        [SerializeField]
        float pitchSpeed = 250;
        [SerializeField]
        Vector2 minMaxPitch = new Vector2(-80, 80);
        [SerializeField]
        float baseFieldOfView = 65; 
        [SerializeField, Min(0)]
        float viewHeightOffset = 0.25f;
        [SerializeField]
        FirstPersonCamera cameraPrefab;
        [SerializeField]
        bool viewBobbing = false;
        [SerializeField]
        float viewBobbingScale = .05f;

        [Header("Jump")]
        [SerializeField]
        float jumpImpulseSpeed = 8.5f;
        [SerializeField]
        int maxJumps = 1;
        [SerializeField]
        float minDelayBetweenJumps = 0.35f;

        [Header("Dash")]
        [SerializeField]
        float dashSpeed = 10.5f;
        [SerializeField]
        float dashTransitionSpeed = 7f;
        [SerializeField]
        float dashForwardThreshold = 0.8f;
        [SerializeField]
        float dashFOV = 80f;
        [SerializeField]
        float dashDuration = -1f;

        [Header("Crouch")]
        [SerializeField]
        bool toggleCrouch = true;
        [SerializeField]
        float crouchHeight = 0.9f;
        [SerializeField]
        float crouchMoveSpeed = 2.9f;
        [SerializeField]
        float crouchTransitionSpeed = 15f;


        [Header("Debug")]
        [SerializeField]
        bool drawDebug = false;

        // Private Fields
        Transform m_CameraRoot;
        CharacterController m_Character;
        FirstPersonCamera m_Camera;

        private void Awake()
        {
            SyncComponents();

            if (!CreateCamera())
            {
                this.enabled = false;
                Debug.LogError("FirstPersonController : cannot create camera, aborting");
                return;
            }
        }

        private void OnValidate()
        {
            SyncComponents();
        }

        void SyncComponents()
        {
            m_Character = GetComponent<CharacterController>();
            m_Character.hideFlags = HideFlags.NotEditable;
            m_Character.slopeLimit = slopeLimit;
            m_Character.stepOffset = stepOffset;
            m_Character.skinWidth = skinWidth;
            m_Character.minMoveDistance = minMoveDistance;
            m_Character.radius = bodyRadius;
            m_Character.height = bodyHeight;
            m_Character.center = new Vector3(0, bodyHeight / 2, 0);
        }

        #region UPDATE
        private void LateUpdate()
        {
            if (Paused)
                return;

            UpdateCrouch();
            UpdateGravityAndMomentum();
            UpdateCameraRotation();
            UpdateJump();
            UpdateDash();
            UpdateMovement();
            UpdateCameraFov();

            // Apply to character   
            m_Move = m_Movement + m_Forces;

            m_Character.Move(m_Move * Time.deltaTime);

        }

        #endregion

        #region CROUCH

        bool crouch;
        float m_Crouch = 0f;

        void UpdateCrouch()
        {
            if (!CanCrouch)
                crouch = false;
            else
            {
                ButtonState crouchBtn = input.crouch;
                if (toggleCrouch)
                {
                    if (crouchBtn == ButtonState.JustPressed)
                        crouch = !crouch;
                }
                else
                    crouch = (crouchBtn == ButtonState.JustPressed || crouchBtn == ButtonState.Pressed);
            }


            // If crouched but wants to get up...
            if (m_Crouch > 0.0f && !crouch)
            {
                // Check if nothing blocks above our head
                if (Physics.Raycast(transform.position + (transform.up * bodyHeight), transform.up, (bodyHeight - crouchHeight)))
                {
                    // If blocked, Forbid getting up.
                    crouch = true;
                }
            }

            // Interpolate Crouch
            m_Crouch = Mathf.Clamp01(m_Crouch + (crouchTransitionSpeed * Time.deltaTime * (crouch ? 1 : -1)));

            // Update Player height
            m_Character.height = Mathf.Lerp(bodyHeight, crouchHeight, m_Crouch);
            m_Character.center = new Vector3(0, m_Character.height / 2, 0);
            m_CameraRoot.localPosition = new Vector3(0, m_Character.height - viewHeightOffset, 0);

        }
        #endregion

        #region BODY AND MOTION

        Vector3 m_Move;

        Vector3 m_Forces;
        float m_ForceMag;

        void UpdateGravityAndMomentum()
        {
            // Apply Gravity
            m_Forces += Physics.gravity * gravityScale * Time.deltaTime;

            
            // Apply Friction
            if (m_Character.isGrounded)
            {
                m_Forces *= (1.0f - (groundFriction * Time.deltaTime));
            }

            m_ForceMag = m_Forces.magnitude;

            // Apply Terminal Velocity Clamp
            if (m_ForceMag > terminalSpeed)
            {
                m_ForceMag = terminalSpeed;
                m_Forces = m_Forces.normalized * m_ForceMag;
            }
        }

        void Impulse(Vector3 vector, bool replace)
        {
            if (replace)
                m_Forces = vector;
            else
                m_Forces += vector;
        }

        #endregion

        #region MOVEMENT

        Vector3 m_Movement;
        float m_ForwardDot;

        void UpdateMovement()
        {
            Vector2 move = input.move;
            m_ForwardDot = Vector2.Dot(move, Vector2.up);

            m_Movement = transform.forward * move.y;
            m_Movement += transform.right * move.x;
            m_Movement *= Mathf.Lerp(Mathf.Lerp(moveSpeed, crouchMoveSpeed, m_Crouch), dashSpeed, m_Dash);
        }

        #endregion

        #region JUMP

        int m_Jump;
        float m_JumpTTL;

        void UpdateJump()
        {
            if (m_Character.isGrounded)
                m_Jump = 0;

            m_JumpTTL += Time.deltaTime;

            ButtonState jumpBtn = input.jump;

            bool jump = (CanJump && jumpBtn == ButtonState.JustPressed && (m_Jump < maxJumps || maxJumps <= 0) && m_JumpTTL >= minDelayBetweenJumps);

            if(jump)
            {
                m_Jump++;
                m_JumpTTL = 0;
                Impulse(new Vector3(0,jumpImpulseSpeed,0), false);
            }
        }

        #endregion

        #region DASH

        float m_Dash;
        float m_DashTTL;

        void UpdateDash()
        {
            ButtonState dashBtn = input.dash;
            bool dash = CanDash && m_Crouch == 0 && m_ForwardDot > dashForwardThreshold && (dashBtn == ButtonState.JustPressed || dashBtn == ButtonState.Pressed);

            if(dash)
            {
                m_DashTTL += Time.deltaTime;
                m_Dash += ((dashDuration > 0 && m_DashTTL > dashDuration) ? -1 : 1) * dashTransitionSpeed * Time.deltaTime;
            }
            else
            {
                m_Dash -= dashTransitionSpeed * Time.deltaTime;
            }

            m_Dash = Mathf.Clamp01(m_Dash);

            if (m_Dash == 0)
                m_DashTTL = 0;

        }

        #endregion

        #region CAMERA

        /// <summary>
        /// Attempts to create the camera object, and returns whether it was successful or not
        /// </summary>
        /// <returns></returns>
        bool CreateCamera()
        {
            if (cameraPrefab == null)
                return false;

            try
            {
                var root = new GameObject("CameraRoot");
                root.transform.parent = this.transform;
                root.transform.localPosition = new Vector3(0, bodyHeight - viewHeightOffset, 0);
                root.transform.localRotation = Quaternion.identity;
                root.transform.localScale = Vector3.one;

                m_CameraRoot = root.transform;

                var camera = GameObject.Instantiate(cameraPrefab.gameObject, m_CameraRoot);
                camera.name = cameraPrefab.gameObject.name;
                camera.transform.localPosition = Vector3.zero;
                camera.transform.localRotation = Quaternion.identity;
                camera.transform.localScale = Vector3.zero;
                m_Camera = camera.GetComponent<FirstPersonCamera>();
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }


        float m_Yaw;
        float m_Pitch;

        void UpdateCameraRotation()
        {
            Vector2 look = input.look;

            // Turn : Applied to body
            m_Yaw = transform.localEulerAngles.y;
            float yawOffset = look.x * turnSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, yawOffset, 0));

            // Pitch : Applied to Camera Root
            m_Pitch = m_CameraRoot.localEulerAngles.x;
            m_Pitch = m_Pitch < 180 ? m_Pitch : m_Pitch - 360;
            m_Pitch -= look.y * pitchSpeed * Time.deltaTime;
            m_Pitch = Mathf.Clamp(m_Pitch, minMaxPitch.x, minMaxPitch.y);
            m_CameraRoot.localEulerAngles = new Vector3(m_Pitch, 0, 0);
        }

        void UpdateCameraFov()
        {
            m_Camera.fov = Mathf.Lerp(baseFieldOfView, dashFOV, m_Dash);
        }

        #endregion

        #region DEBUG

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(new Vector3(0,bodyHeight-viewHeightOffset,0), 40, 5, .01f, 1);
        }

        #endregion

        void OnGUI()
        {
            if (!drawDebug)
                return;

            System.Text.StringBuilder debugString = new System.Text.StringBuilder();
            debugString.AppendLine($"Position {transform.position} | Direction {transform.forward} ");
            debugString.AppendLine($"Grounded : {m_Character.isGrounded} | Jumps : {m_Jump} | Crouch : {m_Crouch.ToString("F2")}" );
            debugString.AppendLine($"Dash {m_Dash.ToString("F2")}| ForwardDot {m_ForwardDot.ToString("F2")} | TTL ({m_DashTTL.ToString("F2")}s.)" );
            debugString.AppendLine($"Movement {m_Movement} | Speed : {m_Movement.magnitude.ToString("F2")} "); 
            debugString.AppendLine($"Forces {m_Forces} | Forces Mag : {m_Forces.magnitude.ToString("F2")} "); 
            debugString.AppendLine($"Character Velocity {m_Character.velocity} | Character Speed : {m_Character.velocity.magnitude.ToString("F2")} ");
            debugString.AppendLine($"Camera Pitch {m_Pitch.ToString("F2")} | Yaw {m_Yaw.ToString("F2")} | FOV {m_Camera.fov.ToString("F2")}");

            GUI.Box(new Rect(0, 0, 640, 300), "<b>Character Debug</b>");
            GUI.Label(new Rect(10, 24, 620, 364), debugString.ToString());
        }

    }
}