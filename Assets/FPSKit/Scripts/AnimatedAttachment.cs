using FPSKit;
using UnityEngine;

public class AnimatedAttachment : Attachment
{
    [Header("Animated Attachment")]
    [SerializeField]
    protected Animator animator;

    public override void OnActive(FirstPersonController controller)
    {
        base.OnActive(controller);
    }
}
