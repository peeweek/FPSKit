using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FPSKit
{
    [CustomEditor(typeof(FirstPersonController))]
    public class FirstPersonControllerEditor : Editor
    {
        Dictionary<string, SerializedProperty> m_Props;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI(); return;
            serializedObject.Update();

            DoTabs();

            using(new GUILayout.VerticalScope(Styles.helpBox))
            {
                switch (currentTab)
                {
                    case TabGroup.Body:
                        DoProperty("skinWidth");
                        DoProperty("bodyRadius");
                        DoProperty("bodyHeight");
                        break;
                    case TabGroup.Momentum:
                        DoProperty("terminalSpeed");
                        DoProperty("gravityScale");
                        DoProperty("groundFriction");
                        DoProperty("slopeSlideAngleThredhold");
                        DoProperty("slopeSlideScale");
                        break;
                    case TabGroup.Movement:
                        DoProperty("CanMove");
                        DoProperty("moveSpeed");
                        DoProperty("minMoveDistance");
                        DoProperty("slopeLimit");
                        DoProperty("stepOffset");
                        break;
                    case TabGroup.Look:
                        DoProperty("CanLook");
                        DoProperty("turnSpeed");
                        DoProperty("pitchSpeed");
                        DoProperty("minMaxPitch");
                        DoProperty("baseFieldOfView");
                        DoProperty("viewHeightOffset");
                        DoProperty("cameraPrefab");
                        DoProperty("viewBobbing");
                        DoProperty("viewBobbingCurve");
                        break;
                    case TabGroup.Aim:
                        DoProperty("toggleAim");
                        DoProperty("aimFOV");
                        DoProperty("aimTransitionSpeed");
                        DoProperty("recoilSmoothSpeed");
                        break;
                    case TabGroup.Jump:
                        DoProperty("CanJump");
                        DoProperty("jumpImpulseSpeed");
                        DoProperty("maxJumps");
                        DoProperty("minDelayBetweenJumps");
                        break;
                    case TabGroup.Dash:
                        DoProperty("CanDash");
                        DoProperty("dashSpeed");
                        DoProperty("dashTransitionSpeed");
                        DoProperty("dashForwardThreshold");
                        DoProperty("dashFOV");
                        DoProperty("dashDuration");
                        break;
                    case TabGroup.Crouch:
                        DoProperty("CanCrouch");
                        DoProperty("toggleCrouch");
                        DoProperty("crouchHeight");
                        DoProperty("crouchMoveSpeed");
                        DoProperty("crouchTransitionSpeed");
                        break;
                    case TabGroup.Interaction:
                        DoProperty("CanInteract");
                        DoProperty("interactionLayerMask");
                        DoProperty("interactMaxDistance");
                        DoProperty("interactions");
                        break;
                    case TabGroup.Audio:
                        DoProperty("foleyEffect");
                        DoProperty("foleyMinMaxStepDistance");
                        DoProperty("foleyFirstStepDistance");
                        DoProperty("jumpEffect");
                        DoProperty("jumpLandEffect");
                        break;
                    case TabGroup.Attachments:
                        DoProperty("initialAttachment");
                        DoProperty("initialAttachmentIndex");
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.Space();
            DoProperty("input");
            DoProperty("Paused");
            DoProperty("handleCursorWhilePaused");
            DoProperty("drawDebug");

            serializedObject.ApplyModifiedProperties();
        }

        enum TabGroup
        {
            Body,
            Momentum,
            Movement,
            Look,
            Aim,
            Jump,
            Dash,
            Crouch,
            Interaction,
            Audio,
            Attachments
        }

        TabGroup currentTab = TabGroup.Body;

        void DoTab(TabGroup tabGroup, GUIStyle style, Texture texture = null)
        {
            if (GUILayout.Toggle(currentTab == tabGroup, new GUIContent(tabGroup.ToString(),texture), style))
                currentTab = tabGroup;
        }

        void DoTabs()
        {
            using(new GUILayout.HorizontalScope())
            {
                DoTab(TabGroup.Body, Styles.miniButtonLeft);
                DoTab(TabGroup.Momentum, Styles.miniButtonMid);
                DoTab(TabGroup.Movement, Styles.miniButtonMid);
                DoTab(TabGroup.Look, Styles.miniButtonMid);
                DoTab(TabGroup.Jump, Styles.miniButtonRight);
            }
            using (new GUILayout.HorizontalScope())
            {
                DoTab(TabGroup.Aim, Styles.miniButtonLeft);
                DoTab(TabGroup.Dash, Styles.miniButtonMid);
                DoTab(TabGroup.Crouch, Styles.miniButtonMid);
                DoTab(TabGroup.Interaction, Styles.miniButtonMid);
                DoTab(TabGroup.Audio, Styles.miniButtonMid);
                DoTab(TabGroup.Attachments, Styles.miniButtonRight);
            }
            GUILayout.Space(8);

        }

        void DoProperty(string name)
        {
            if (m_Props == null)
                m_Props = new Dictionary<string, SerializedProperty>();

            if(!m_Props.ContainsKey(name))
            {
                var prop = serializedObject.FindProperty(name);
                if (prop != null)
                    m_Props.Add(name, prop);
                else
                {
                    EditorGUILayout.HelpBox($"Could not find property of name {name}", MessageType.Error);
                    return;
                }
            }

            EditorGUILayout.PropertyField(m_Props[name], true);

        }
    }

    static class Styles
    {
        public static GUIStyle miniButtonLeft;
        public static GUIStyle miniButtonMid;
        public static GUIStyle miniButtonRight;
        public static GUIStyle helpBox;

        static Styles()
        {
            miniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            miniButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
            miniButtonRight = new GUIStyle(EditorStyles.miniButtonRight);

            int height = 24;
            miniButtonLeft.fixedHeight = height;
            miniButtonMid.fixedHeight = height;
            miniButtonRight.fixedHeight = height;

            helpBox = new GUIStyle(EditorStyles.helpBox);
            helpBox.padding = new RectOffset(8, 8, 8, 8);

        }
    }
}

