using FPSKit;
using UnityEngine;

public class LocomotionAttachment : AnimatedAttachment
{
    [SerializeField]
    string AnimatorDashProperty = "Dash";

    public override void OnUpdate(FirstPersonController controller)
    {
        base.OnUpdate(controller);

        animator.SetBool(AnimatorDashProperty, controller.dash);
    }
}
