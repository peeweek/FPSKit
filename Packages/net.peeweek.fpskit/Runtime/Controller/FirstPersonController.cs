using System;
using System.Collections.Generic;
using System.Linq;
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
        AnimationCurve viewBobbingCurve;

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

        [Header("Audio")]
        [SerializeField]
        PlayAudioEffect foleyEffect;
        [SerializeField]
        Vector2 foleyMinMaxStepDistance = new Vector2(2.5f,3.1f);
        [SerializeField]
        float foleyFirstStepDistance = 1.0f;
        [SerializeField]
        PlayAudioEffect jumpEffect;
        [SerializeField]
        PlayAudioEffect jumpLandEffect;

        [Header("Attachments")]
        [SerializeField]
        Attachment initialAttachment;
        [SerializeField]
        int initialAttachmentIndex = 0;

        [Header("Debug")]
        [SerializeField]
        bool drawDebug = false;

        // Private Fields
        Transform m_CameraRoot;
        CharacterController m_Character;
        FirstPersonCamera m_Camera;

        Dictionary<int, Attachment> m_Attachments;
        Attachment m_CurrentAttachment;

        private void Awake()
        {
            SyncComponents();

            if (!CreateCamera())
            {
                this.enabled = false;
                Debug.LogError("FirstPersonController : cannot create camera, aborting");
                return;
            }

            m_Attachments = new Dictionary<int, Attachment>();

            if (initialAttachment != null && TryAttach(initialAttachment, out Attachment attachment))
            {
                SetAttachment(attachment.index);
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
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            UpdateCrouch();
            UpdateGravityAndMomentum();
            UpdateCameraRotation();
            UpdateJump();
            UpdateDash();
            UpdateMovement();
            UpdateCameraFov();
            UpdateViewBobbing();

            // If Attachment present, update it
            m_CurrentAttachment?.OnUpdate(this);

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
                // Check if nothing blocks above our head, so we can get up
                if (Physics.Raycast(transform.position + (transform.up * crouchHeight), transform.up, (bodyHeight - crouchHeight)))
                {
                    // If not enough room, Forbid getting up
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

        /// <summary>
        /// Applies an impulse to the Character, and optionally replaces current speed;
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="replace"></param>
        void Impulse(Vector3 vector, bool replace)
        {
            if (replace)
                m_Forces = vector;
            else
                m_Forces += vector;
        }

        #endregion

        #region MOVEMENT

        public float speed { get => m_Movement.magnitude; }

        Vector3 m_Movement;
        float m_ForwardDot;
        float m_NextFoley;

        void UpdateMovement()
        {
            Vector2 move = input.move;
            m_ForwardDot = Vector2.Dot(move, Vector2.up);

            m_Movement = transform.forward * move.y;
            m_Movement += transform.right * move.x;
            m_Movement *= Mathf.Lerp(Mathf.Lerp(moveSpeed, crouchMoveSpeed, m_Crouch), dashSpeed, m_Dash);

            if (foleyEffect != null)
            {
                if (m_Character.isGrounded && m_Movement.sqrMagnitude > 0) // If moving
                {
                    // Update Foley
                    if (m_NextFoley <= 0)
                    {
                        m_NextFoley = UnityEngine.Random.Range(foleyMinMaxStepDistance.x, foleyMinMaxStepDistance.y);
                        foleyEffect?.ApplyEffect(transform.position, Vector3.up);
                    }

                    m_NextFoley -= m_Movement.magnitude * Time.deltaTime;
                }
                else // Standing Still
                {
                    m_NextFoley = foleyFirstStepDistance;
                }
            }
        }

        #endregion

        #region JUMP

        public bool jump { get => m_Jump > 0; }

        int m_Jump;
        float m_JumpTTL;

        void UpdateJump()
        {
            if (m_Character.isGrounded)
            {
                if(m_Jump > 0) // Handle Landing
                {
                    m_Jump = 0;
                    jumpLandEffect?.ApplyEffect(transform.position, Vector3.up);
                }
            }

            m_JumpTTL += Time.deltaTime;

            ButtonState jumpBtn = input.jump;

            bool jump = (CanJump && jumpBtn == ButtonState.JustPressed && (m_Jump < maxJumps || maxJumps <= 0) && m_JumpTTL >= minDelayBetweenJumps);

            if(jump)
            {
                m_Jump++;
                m_JumpTTL = 0;
                Impulse(new Vector3(0,jumpImpulseSpeed,0), true);
                jumpEffect?.ApplyEffect(transform.position, Vector3.up);
            }
        }

        #endregion

        #region DASH
        public bool dash { get => m_Dash > 0.1f; }

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
        Vector2 m_Recoil;

        public void Recoil(Vector2 recoil)
        {
            m_Recoil += recoil;
        }

        /// <summary>
        /// Updates player Rotation for Yaw , Camera Root pitch, and applies optional recoil
        /// </summary>
        void UpdateCameraRotation()
        {
            Vector2 look = input.look;

            // Turn : Applied to body
            m_Yaw = transform.localEulerAngles.y;
            float yawOffset = look.x * turnSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, yawOffset + m_Recoil.y, 0));

            // Pitch : Applied to Camera Root
            m_Pitch = m_CameraRoot.localEulerAngles.x;
            m_Pitch = m_Pitch < 180 ? m_Pitch : m_Pitch - 360;
            m_Pitch -= look.y * pitchSpeed * Time.deltaTime;
            m_Pitch = Mathf.Clamp(m_Pitch, minMaxPitch.x, minMaxPitch.y);
            m_CameraRoot.localEulerAngles = new Vector3(m_Pitch + m_Recoil.x, 0, 0);

            if (m_Recoil.sqrMagnitude > 0)
                m_Recoil = Vector3.zero;
        }

        float m_ViewBob;
        float m_ViewBobTime;

        void UpdateViewBobbing()
        {
            if (viewBobbing)
            {
                if (speed > 0)
                {
                    m_ViewBob = Mathf.Clamp01(m_ViewBob + Time.deltaTime * 4);
                    m_ViewBobTime +=  speed * Time.deltaTime;
                }
                else
                {
                    m_ViewBob = Mathf.Clamp01(m_ViewBob - Time.deltaTime * 12);
                }

                if (m_ViewBob == 0)
                    m_ViewBobTime = 0;

                float b = viewBobbingCurve.Evaluate(m_ViewBobTime);
                m_CameraRoot.localPosition += new Vector3(0, b * m_ViewBob, 0);
            }
        }

        void UpdateCameraFov()
        {
            m_Camera.fov = Mathf.Lerp(baseFieldOfView, dashFOV, m_Dash);
        }

        #endregion

        #region ATTACHMENTS

        /// <summary>
        /// Sets next attachment available, active (cycling)
        /// </summary>
        /// <returns>The success of the operation</returns>
        public bool NextAttachment()
        {
            if (m_CurrentAttachment == null && m_Attachments != null && m_Attachments.Keys.Count > 1)
                return false;

            var keys = m_Attachments.Keys.OrderBy(index => index).ToList();
            int index = (keys.IndexOf(m_CurrentAttachment.index) + 1) % keys.Count;
            return SetAttachment(index);
        }

        /// <summary>
        /// Sets previous attachment available, active (cycling)
        /// </summary>
        /// <returns>The success of the operation</returns>
        public bool PreviousAttachment()
        {
            if (m_CurrentAttachment == null && m_Attachments != null && m_Attachments.Keys.Count > 1)
                return false;

            var keys = m_Attachments.Keys.OrderBy(index => index).ToList();
            int index = (keys.IndexOf(m_CurrentAttachment.index) - 1) % keys.Count;
            return SetAttachment(index);
        }

        /// <summary>
        /// Enables the attachment at given index (if present)
        /// </summary>
        /// <param name="index"></param>
        public bool SetAttachment(int index)
        {
            if (m_Attachments == null || !m_Attachments.ContainsKey(index))
                return false;

            foreach(var kvp in m_Attachments)
            {
                bool active = kvp.Value.gameObject.activeSelf;

                if (kvp.Key == index)
                {
                    if(!active)
                    {
                        kvp.Value.gameObject.SetActive(true);
                        kvp.Value.OnActive(this);
                        m_CurrentAttachment = kvp.Value;
                    }
                }
                else
                {
                    if(active)
                    {
                        kvp.Value.OnInactive(this);
                        kvp.Value.gameObject.SetActive(false);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public bool Remove(int index)
        {
            if(m_Attachments != null && m_Attachments.ContainsKey(index))
            {
                if (m_Attachments[index] != null)
                {
                    var attachment = m_Attachments[index];
                    m_Attachments.Remove(index);
                    attachment.OnInactive(this);
                    attachment.OnDetach(this);
                    Destroy(attachment.gameObject);
                    return true;
                }      
            }
            return false;
        }

        /// <summary>
        /// Tries attaching a referenceAttachment, and returns if it was successful
        /// </summary>
        /// <param name="referenceAttachment"></param>
        /// <param name="newAttachment"></param>
        /// <returns></returns>
        internal bool TryAttach(Attachment referenceAttachment, out Attachment newAttachment, bool setActive = true)
        {
            newAttachment = null;

            if (referenceAttachment == null)
                return false;

            // If attachment already present at index
            if (m_Attachments.ContainsKey(referenceAttachment.index))
            {
                // Check if we can replace it
                if(referenceAttachment.replacesWhenAttached)
                    Remove(referenceAttachment.index);
                else
                    return false; // Or just fail
            }

            try
            {
                var go = Instantiate(referenceAttachment.gameObject);
                go.SetActive(false);
                go.name = referenceAttachment.gameObject.name;
                go.transform.parent = m_CameraRoot;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                newAttachment = go.GetComponent<Attachment>();
                m_Attachments[newAttachment.index] = newAttachment;
                newAttachment.OnAttach(this);

                if (setActive)
                    SetAttachment(newAttachment.index);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        #endregion

        #region DEBUG

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(new Vector3(0,bodyHeight-viewHeightOffset,0), 40, 5, .01f, 1);
        }

        void OnGUI()
        {
            if (!drawDebug)
                return;

            System.Text.StringBuilder debugString = new System.Text.StringBuilder();
            debugString.AppendLine($"Position {transform.position} | Direction {transform.forward} ");
            debugString.AppendLine($"Grounded : {m_Character.isGrounded} | Jumps : {m_Jump} | Crouch : {m_Crouch.ToString("F2")}");
            debugString.AppendLine($"Dash {m_Dash.ToString("F2")}| ForwardDot {m_ForwardDot.ToString("F2")} | TTL ({m_DashTTL.ToString("F2")}s.)");
            debugString.AppendLine($"Movement {m_Movement} | Speed : {m_Movement.magnitude.ToString("F2")} ");
            debugString.AppendLine($"Forces {m_Forces} | Forces Mag : {m_Forces.magnitude.ToString("F2")} ");
            debugString.AppendLine($"Character Velocity {m_Character.velocity} | Character Speed : {m_Character.velocity.magnitude.ToString("F2")} ");
            debugString.AppendLine($"Camera Pitch {m_Pitch.ToString("F2")} | Yaw {m_Yaw.ToString("F2")} | FOV {m_Camera.fov.ToString("F2")}");

            GUI.Box(new Rect(0, 0, 640, 300), "<b>Character Debug</b>");
            GUI.Label(new Rect(10, 24, 620, 364), debugString.ToString());
        }
        #endregion


    }
}