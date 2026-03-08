#if USE_CINEMACHINE
using UnityEngine;
using Unity.Cinemachine;

namespace FPSKit
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class FirstPersonCinemachineCamera : FirstPersonCamera
    {
        public override float fov { get => cinemachineCamera.Lens.FieldOfView; set => cinemachineCamera.Lens.FieldOfView = value; }

        public CinemachineCamera cinemachineCamera 
        {
            get 
            {   if (m_Camera == null)
                    m_Camera = GetComponent<CinemachineCamera>();
                return m_Camera; 
            } 
        }

        CinemachineCamera m_Camera;
    }
}

#endif

