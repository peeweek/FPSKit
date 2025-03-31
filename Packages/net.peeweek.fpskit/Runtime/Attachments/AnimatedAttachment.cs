using UnityEngine;

namespace FPSKit
{
    public class AnimatedAttachment : Attachment
    {
        [Header("Animated Attachment")]
        [SerializeField]
        protected Animator animator;

        public override void OnActive()
        {
            base.OnActive();
        }
    }
}

