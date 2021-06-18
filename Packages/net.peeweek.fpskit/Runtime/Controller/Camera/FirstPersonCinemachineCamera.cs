#if USE_CINEMACHINE
using UnityEngine;
using Cinemachine;

namespace FPSKit
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class FirstPersonCinemachineCamera : FirstPersonCamera
    {
        public override float fov { get => cinemachineCamera.m_Lens.FieldOfView; set => cinemachineCamera.m_Lens.FieldOfView = value; }

        public CinemachineVirtualCamera cinemachineCamera 
        {
            get 
            {   if (m_Camera == null)
                    m_Camera = GetComponent<CinemachineVirtualCamera>();
                return m_Camera; 
            } 
        }

        CinemachineVirtualCamera m_Camera;

    }

}

#endif

