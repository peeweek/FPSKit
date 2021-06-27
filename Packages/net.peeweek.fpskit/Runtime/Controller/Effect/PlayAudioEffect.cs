using UnityEngine;

namespace FPSKit
{
    public class PlayAudioEffect : Effect
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        AudioClip[] audioClips;

        public override void ApplyEffect(Vector3 position, Vector3 normal)
        {
            if (audioSource == null || audioClips == null)
                return;

            AudioClip a = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.Stop();
            audioSource.clip = a;
            audioSource.Play();
        }
    }
}



