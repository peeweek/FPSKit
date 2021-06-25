using UnityEngine;

namespace FPSKit
{
    /// <summary>
    /// Represents a rig attached to the camera root. For instance: FPS Player Hands, or a Weapon.
    /// This script needs to be used on a Game Object and optionally used in a prefab. 
    /// 
    /// Upon attaching, a Clone of this game object is created for attachment
    /// 
    /// </summary>
    public class Attachment : MonoBehaviour
    {

        public int index { get => m_AttachmentIndex; }
        public bool replacesWhenAttached { get => m_ReplacesWhenAttached; }
        public FirstPersonController owner { get => m_Owner; }
        [Header("Global Settings")]
        [SerializeField, Tooltip("Replaces any already attachment present at given index when attached")]
        private bool m_ReplacesWhenAttached;
        [SerializeField, Tooltip("The index in the player's attachment list at which the attachment should attach")]
        private int m_AttachmentIndex;

        [SerializeField, HideInInspector]
        private FirstPersonController m_Owner;

        /// <summary>
        /// Method called when the attachment is created and attached to a player
        /// </summary>
        /// <param name="controller">The First Person Controller this Attachment is attached to</param>
        public virtual void OnAttach(FirstPersonController controller)
        {
            m_Owner = controller;
        }

        /// <summary>
        /// Method called when the attachment is removed from a player, just before removal
        /// </summary>
        /// <param name="controller">The First Person Controller this Attachment is attached to</param>
        public virtual void OnDetach(FirstPersonController controller) { }

        /// <summary>
        /// Method Called just after attachment becomes active (for instance when changing attachments)
        /// </summary>
        /// <param name="controller">The First Person Controller this Attachment is attached to</param>
        public virtual void OnActive(FirstPersonController controller) { }

        /// <summary>
        /// Method Called just before attachment becomes inactive (for instance when changing attachments)
        /// </summary>
        /// <param name="controller">The First Person Controller this Attachment is attached to</param>
        public virtual void OnInactive(FirstPersonController controller) { }


        /// <summary>
        /// OnUpdate is called by the FirstPersonController whem the attachment is active
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnUpdate(FirstPersonController controller) { }
    }

}

