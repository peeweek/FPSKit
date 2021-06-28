using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public class PlayAudioEffect : Effect
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        bool instantiateWhenOverlapping;

        [SerializeField]
        AudioClip[] audioClips;


        List<AudioSource> m_AudioSources;

        int idx = 0;

        public override void ApplyEffect(Vector3 position, Vector3 normal)
        {
            if (audioSource == null || audioClips == null || audioClips.Length == 0)
                return;

            // Compute new Idx not to be equal to previous
            idx = (idx + Random.Range(1, audioClips.Length - 1)) % audioClips.Length;

            AudioClip newClip = audioClips[idx];

            if(instantiateWhenOverlapping)
            {
                if(m_AudioSources == null)
                {
                    m_AudioSources = new List<AudioSource>();
                    m_AudioSources.Add(audioSource);
                }
                AudioSource freeSource = null;

                foreach (var source in m_AudioSources)
                {
                    if (!source.isPlaying)
                    {
                        freeSource = source;
                        break;
                    }
                }

                if(freeSource == null)
                { 
                    freeSource = DuplicateAudioSource();
                    m_AudioSources.Add(freeSource);
                }

                freeSource.Stop();
                freeSource.clip = newClip;
                freeSource.Play();
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = newClip;
                audioSource.Play();
            }
        }

        AudioSource DuplicateAudioSource()
        {
            GameObject go = Instantiate(audioSource.gameObject);
            go.name = audioSource.name;
            go.transform.parent = audioSource.transform.parent;
            go.transform.localPosition = audioSource.transform.localPosition;
            go.transform.localRotation = audioSource.transform.localRotation;
            go.transform.localScale = audioSource.transform.localScale;
            AudioSource source;
            if(!go.TryGetComponent(out source))
            {
                source = go.AddComponent<AudioSource>();
            }
            source.clip = null;
            source.loop = false;
            source.Stop();
            return source;
        }

    }
}



