using UnityEngine;

namespace FPSKit
{
    [RequireComponent(typeof(Camera))]
    public class FirstPersonSimpleCamera : FirstPersonCamera
    {
        new Camera camera
        {
            get
            {
                if (m_Camera == null)
                    m_Camera = gameObject.GetComponent<Camera>();
                return m_Camera;
            }
        }

        Camera m_Camera;

        public override float fov { get => camera.fieldOfView; set => camera.fieldOfView = value; }
    }

}


