using UnityEngine;

namespace FPSKit
{
    public class LocomotionAttachment : AnimatedAttachment
    {
        [SerializeField]
        string AnimatorDashProperty = "Dash";
        [SerializeField]
        string AnimatorMoveProperty = "Move";
        [SerializeField]
        string AnimatorJumpProperty = "Jump";

        public override void OnUpdate(FirstPersonController controller)
        {
            base.OnUpdate(controller);

            animator.SetBool(AnimatorDashProperty, controller.dash);
            animator.SetFloat(AnimatorMoveProperty, controller.speed);
            animator.SetBool(AnimatorJumpProperty, controller.jump);

        }
    }

}
