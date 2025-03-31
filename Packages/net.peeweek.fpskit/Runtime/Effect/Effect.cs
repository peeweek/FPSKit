using UnityEngine;

namespace FPSKit
{
    /// <summary>
    /// Represents an effect (Camera, VFX, SFX, Lighting) triggered by FPSKit
    /// </summary>
    public abstract class ContinuousEffect : MonoBehaviour
    {
        public abstract void OnEffectApply(Vector3 position, Vector3 target);

        public abstract void OnEffectUpdate(Vector3 position, Vector3 target);

        public abstract void OnEffectStop();

        public Vector3 effectPosition;
        public Vector3 effectTarget;

        public void ApplyEffect(Vector3 position, Vector3 target)
        {
            this.effectPosition = position;
            this.effectTarget = target;
            OnEffectApply(position, target);
            this.gameObject.SetActive(true);
        }

        public void Update()
        {
            OnEffectUpdate(this.effectPosition, this.effectTarget);
        }

        public void StopEffect()
        {
            this.gameObject.SetActive(false);
        }


    }
}