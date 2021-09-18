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
            DoProperty("Paused");

            DoTabs();
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
                    break;
                case TabGroup.Jump:
                    break;
                case TabGroup.Crouch:
                    break;
                case TabGroup.Interaction:
                    break;
                case TabGroup.Audio:
                    break;
                case TabGroup.Attachments:
                    break;
                default:
                    break;
            }

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
            Crouch,
            Interaction,
            Audio,
            Attachments
        
        }

        TabGroup currentTab = TabGroup.Body;

        void DoTab(TabGroup tabGroup, GUIStyle style)
        {
            if (GUILayout.Toggle(currentTab == tabGroup, tabGroup.ToString(), style))
                currentTab = tabGroup;
        }
        void DoTabs()
        {
            using(new GUILayout.HorizontalScope())
            {
                DoTab(TabGroup.Body, EditorStyles.miniButtonLeft);
                DoTab(TabGroup.Momentum, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Movement, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Look, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Aim, EditorStyles.miniButtonRight);
            }
            using (new GUILayout.HorizontalScope())
            {
                DoTab(TabGroup.Jump, EditorStyles.miniButtonLeft);
                DoTab(TabGroup.Crouch, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Interaction, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Audio, EditorStyles.miniButtonMid);
                DoTab(TabGroup.Attachments, EditorStyles.miniButtonRight);
            }

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

            EditorGUILayout.PropertyField(m_Props[name]);

        }
    }
}

