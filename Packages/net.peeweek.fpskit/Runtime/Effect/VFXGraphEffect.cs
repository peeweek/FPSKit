using UnityEngine;
using UnityEngine.VFX;

namespace FPSKit
{
    public class VFXGraphEffect : Effect
    {
        [SerializeField]
        string eventName = "OnPlay";

        [SerializeField]
        VisualEffect visualEffect;

        VFXEventAttribute m_EventAttribute;
        public override void ApplyEffect(Vector3 position, Vector3 target)
        {
            if (m_EventAttribute == null)
                m_EventAttribute = visualEffect.CreateVFXEventAttribute();

            m_EventAttribute.SetVector3("position", position);
            m_EventAttribute.SetVector3("targetPosition", target);

            visualEffect.SendEvent(eventName, m_EventAttribute);
        }
    }
}

