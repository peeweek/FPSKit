
using UnityEngine;
using UnityEngine.Events;

namespace FPSKit
{
    public class SimpleInteractionTarget_UnityEvent : SimpleInteractionTarget
    {
        public UnityEvent onInteract;

        public override bool Interact(FirstPersonController controller, SimpleInteraction interaction)
        {
            onInteract.Invoke();
            return true;
        }
    }
}

