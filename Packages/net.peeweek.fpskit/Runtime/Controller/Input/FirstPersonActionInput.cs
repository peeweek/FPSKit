#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace FPSKit
{
    public class FirstPersonActionInput : FirstPersonInput
    {
        [Header("Axis")]
        public InputAction moveAxis;

        public InputAction lookAxis;

        [Header("Buttons")]

        public InputAction jumpButton;

        public InputAction dashButton;

        public InputAction crouchButton;

        public InputAction aimButton;

        public InputAction primaryActionButton;

        public InputAction secondaryActionButton;

        public InputAction interactButton;

        public InputAction nextAttachmentButton;

        public InputAction previousAttachmentButton;


        public override Vector2 move => GetAxis(moveAxis);

        public override Vector2 look => GetAxis(lookAxis);

        public override ButtonState jump => GetButton(jumpButton);

        public override ButtonState dash => GetButton(dashButton);

        public override ButtonState crouch => GetButton(crouchButton);

        public override ButtonState aim => GetButton(aimButton);

        public override ButtonState primaryAction => GetButton(primaryActionButton);

        public override ButtonState secondaryAction => GetButton(secondaryActionButton);

        public override ButtonState interact => GetButton(interactButton);

        public override ButtonState nextAttachment => GetButton(nextAttachmentButton);

        public override ButtonState previousAttachment => GetButton(previousAttachmentButton);


        Vector2 GetAxis(InputAction axisAction)
        {
            return axisAction.ReadValue<Vector2>();
        }

        ButtonState GetButton(InputAction buttonAction)
        {
            if (!enabled)
                return ButtonState.Released;

            if (!buttonAction.enabled)
                buttonAction.Enable();

            var control = buttonAction.activeControl as ButtonControl;

            if(control == null)
            {
                return ButtonState.Released;
            }


            if (control.isPressed)
            {
                return (control.wasPressedThisFrame ? ButtonState.JustPressed : ButtonState.Pressed);
            }
            else
            {
                return (control.wasReleasedThisFrame ? ButtonState.JustReleased : ButtonState.Released);
            }


        }
        private void OnEnable()
        {
            moveAxis.Enable();
            lookAxis.Enable();
            jumpButton.Enable();
            dashButton.Enable();
            crouchButton.Enable();
            primaryActionButton.Enable();
            secondaryActionButton.Enable();
            interactButton.Enable();
        }
        private void OnDisable()
        {
            moveAxis.Disable();
            lookAxis.Disable();
            jumpButton.Disable();
            dashButton.Disable();
            crouchButton.Disable();
            primaryActionButton.Disable();
            secondaryActionButton.Disable();
            interactButton.Disable();
        }
    }
}

#endif