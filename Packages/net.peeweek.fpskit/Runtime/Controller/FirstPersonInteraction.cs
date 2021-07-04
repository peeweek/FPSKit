using UnityEngine;

namespace FPSKit
{
    public abstract class FirstPersonInteraction : MonoBehaviour
    {
        public abstract bool OnInteract(ButtonState interactState, FirstPersonController controller, RaycastHit raycastHit);
    }
}



