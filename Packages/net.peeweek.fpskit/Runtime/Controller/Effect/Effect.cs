using UnityEngine;

namespace FPSKit
{
    /// <summary>
    /// Represents an effect (Camera, VFX, SFX, Lighting) triggered by FPSKit
    /// </summary>
    public abstract class Effect : MonoBehaviour
    {
        public abstract void ApplyEffect(Vector3 position, Vector3 normal);
    }
}