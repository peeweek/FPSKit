using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    [RequireComponent(typeof(SphereCollider))]
    public class FirstPersonPickup : MonoBehaviour
    {
        [SerializeField]
        Attachment referenceAttachmentPrefab;
        [SerializeField]
        bool switchToOnPickUp = false;

        public Attachment attachment { get => referenceAttachmentPrefab; }
        SphereCollider m_Collider;



        private void Awake()
        {
            m_Collider = GetComponent<SphereCollider>();
            m_Collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out FirstPersonController controller))
            {
                controller.PickUp(this, switchToOnPickUp);
            }
        }
    }
}


