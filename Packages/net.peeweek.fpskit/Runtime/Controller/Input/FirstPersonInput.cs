using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSKit
{
    public enum ButtonState
    {
        Released,
        JustPressed,
        Pressed,
        JustReleased,
    }

    public abstract class FirstPersonInput : MonoBehaviour
    {
        public abstract Vector2 move { get; }
        public abstract Vector2 look { get; }

        public abstract ButtonState jump { get; }
        public abstract ButtonState dash { get; }
        public abstract ButtonState crouch { get; }
        public abstract ButtonState primaryAction { get; }
        public abstract ButtonState secondaryAction { get; }
        public abstract ButtonState tertiaryAction { get; }

    }
}


