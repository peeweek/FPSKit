using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public class PickupDispenser : MonoBehaviour
    {
        [Header("Pickup Configuration")]
        [SerializeField]
        Pickup pickup;
        [SerializeField]
        float respawnDelay = 5f;

        [Header("Pickup Display")]
        [SerializeField]
        GameObject pickupDisplay;
        [SerializeField]
        Vector3 rotationAxis = Vector3.up;
        [SerializeField]
        float roatationSpeed = 3f;
        [SerializeField]
        float floatAmplitude = .5f;
        [SerializeField]
        float floatFrequency = 2f;

        [Header("Effects")]
        public Effect[] onPickup;
        public Effect[] onRespawn;

        private void OnEnable()
        {
            if(pickup != null)
                pickup.onPickup += Pickup_onPickup;

            m_RespawnTTL = -1;
        }

        private void Pickup_onPickup(Pickup pickup, FirstPersonController controller)
        {
            if(onPickup != null)
            {
                foreach (var effect in onPickup)
                    effect?.ApplyEffect(pickup.transform.position, controller.transform.position);

            }

            m_RespawnTTL = respawnDelay;

            pickup.gameObject.SetActive(false);
            pickupDisplay.SetActive(false);
        }

        float m_RespawnTTL;

        private void Update()
        {
            if (pickup.gameObject.activeSelf) // Update Pickup Display
            {
                pickupDisplay.transform.Rotate(rotationAxis, roatationSpeed * Time.deltaTime);
                pickupDisplay.transform.localPosition = rotationAxis * Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            }
            else // Pickup in respawn
            {
                m_RespawnTTL -= Time.deltaTime;

                if(m_RespawnTTL < 0)
                {
                    pickupDisplay.SetActive(true);
                    pickup.gameObject.SetActive(true);

                    if (onRespawn != null)
                    {
                        foreach (var effect in onRespawn)
                        {
                            effect?.ApplyEffect(transform.position, transform.up);
                        }
                    }
                }
            }
        }
    }
}


