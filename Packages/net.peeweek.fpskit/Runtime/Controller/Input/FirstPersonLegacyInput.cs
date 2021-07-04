#if ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;

namespace FPSKit
{
    public class FirstPersonLegacyInput : FirstPersonInput
    {
        [SerializeField]
        string moveAxisX = "Horizontal";
        [SerializeField]
        string moveAxisY = "Vertical";

        [SerializeField]
        string lookAxisX = "Mouse X";
        [SerializeField]
        string lookAxisY = "Mouse Y";

        [SerializeField]
        string jumpButton = "Jump";
        [SerializeField]
        string dashButton = "Fire2";
        [SerializeField]
        string crouchButton = "Fire3";
        [SerializeField]
        string aimButton = "";
        [SerializeField]
        string primaryActionButton = "Fire1";
        [SerializeField]
        string secondaryActionButton = "";
        [SerializeField]
        string interactButton = "Submit";
        [SerializeField]
        string nextAttachmentButton = "";
        [SerializeField]
        string prevAttachmentButton = "";

        public override Vector2 move => new Vector2(Input.GetAxis(moveAxisX), Input.GetAxis(moveAxisY));

        public override Vector2 look => new Vector2(Input.GetAxis(lookAxisX), Input.GetAxis(lookAxisY));

        ButtonState m_Jump;
        public override ButtonState jump => m_Jump;

        ButtonState m_Dash;
        public override ButtonState dash => m_Dash;

        ButtonState m_Crouch;
        public override ButtonState crouch => m_Crouch;

        ButtonState m_Aim;
        public override ButtonState aim => m_Aim;

        ButtonState m_PrimaryAction;
        public override ButtonState primaryAction => m_PrimaryAction;

        ButtonState m_SecondaryAction;
        public override ButtonState secondaryAction => m_SecondaryAction;

        ButtonState m_TertiaryAction;
        public override ButtonState interact => m_TertiaryAction;

        ButtonState m_NextAttachment;
        public override ButtonState nextAttachment => m_NextAttachment;

        ButtonState m_PreviousAttachment;
        public override ButtonState previousAttachment => m_PreviousAttachment;

        private void Update()
        {
            UpdateButton(jumpButton , ref m_Jump);
            UpdateButton(dashButton , ref m_Dash);
            UpdateButton(crouchButton , ref m_Crouch);
            UpdateButton(aimButton , ref m_Aim);
            UpdateButton(primaryActionButton , ref m_PrimaryAction);
            UpdateButton(secondaryActionButton , ref m_SecondaryAction);
            UpdateButton(interactButton, ref m_TertiaryAction);
            UpdateButton(nextAttachmentButton, ref m_NextAttachment);
            UpdateButton(prevAttachmentButton, ref m_PreviousAttachment);
        }

        void UpdateButton(string name, ref ButtonState state)
        {
            if (string.IsNullOrEmpty(name))
                return;

            bool pressed = Input.GetButton(name);
            switch (state)
            {
                default:
                case ButtonState.Released:
                    state = pressed ? ButtonState.JustPressed : ButtonState.Released;
                    break;
                case ButtonState.JustPressed:
                    state = pressed ? ButtonState.Pressed : ButtonState.JustReleased;
                    break;
                case ButtonState.Pressed:
                    state = pressed ? ButtonState.Pressed : ButtonState.JustReleased;
                    break;
                case ButtonState.JustReleased:
                    state = pressed ? ButtonState.JustPressed : ButtonState.Released;
                    break;
            }

        }

    }

}
#endif