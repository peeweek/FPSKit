using UnityEngine;

namespace FPSKit
{
    public abstract class SimpleInteractionTarget : MonoBehaviour
    {
        /// <summary>
        /// Performs an interaction response to a First Person Controller and its Simple interaction
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="interaction"></param>
        /// <returns>whether the interaction was successful</returns>
        public abstract bool Interact(FirstPersonController controller, SimpleInteraction interaction);
    }
}

