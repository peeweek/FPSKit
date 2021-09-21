using UnityEngine;

namespace FPSKit
{
    public class SimpleInteraction : FirstPersonInteraction
    {
        public override bool OnInteract(ButtonState interactState, FirstPersonController controller, RaycastHit raycastHit)
        {
            var go = raycastHit.collider.gameObject;
            var targets = go.GetComponents<SimpleInteractionTarget>();
            foreach(var target in targets)
            {
                if(target.Interact(controller, this))
                {
                    return true;
                }
            }
            return false;
        }
    }
}


